using CAP_Contracts;
using CAP_Core;
using CAP_Core.Components;
using CAP_Core.Components.Creation;
using CAP_Core.ExternalPorts;
using CAP_Core.Grid;
using CAP_Core.Helpers;
using CAP_DataAccess.Components.ComponentDraftMapper;
using Castle.Components.DictionaryAdapter.Xml;
using CAP_DataAccess.Components.ComponentDraftMapper;
using CAP_DataAccess.Components.ComponentDraftMapper.DTOs;
using ConnectAPIC.Scripts.View.ComponentFactory;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static UnitTests.ComponentDraftFileReaderTests;

namespace UnitTests
{
    class DummyDataAccessor : IDataAccessor
    {
        public DummyDataAccessor(string fileContent)
        {
            FileContent = fileContent;
        }

        public string FileContent { get; }

        public bool DoesResourceExist(string resourcePath)
        {
            return true;
        }

        public string ReadAsText(string FilePath)
        {
            return FileContent;   
        }

        public Task<bool> Write(string filePath, string text)
        {
            return new Task<bool>(()=>true);
        }
    }
    public class GridPersistenceManagerTests
    {
        public static string StraightWGJsonString { get; set; } = @"
        {
            ""fileFormatVersion"": 1,
            ""identifier"": ""Straight"",
            ""nazcaFunctionParameters"": """",
            ""nazcaFunctionName"": ""placeCell_StraightWG"",
            ""sceneResPath"": ""res://Scenes/Components/Straight/StraightWaveGuide.tscn"",
            ""deltaLength"": 0,
            ""widthInTiles"": 1,
            ""heightInTiles"": 1,
            ""pins"": [
                {
                    ""number"": 0,
                    ""name"": ""west"",
                    ""matterType"": 1,
                    ""side"": 2,
                    ""partX"": 0,
                    ""partY"": 0
                },
                {
                    ""number"": 1,
                    ""name"": ""east"",
                    ""matterType"": 1,
                    ""side"": 0,
                    ""partX"": 0,
                    ""partY"": 0
                }
            ],
            ""sMatrices"": [
                {
                    ""waveLength"" : 1550,
                    ""connections"": [
                    {
                        ""fromPinNr"": 0,
                        ""toPinNr"": 1,
                        ""real"": 1.0,
                        ""imaginary"" : 2.0
                    },
                    {
                        ""fromPinNr"": 1,
                        ""toPinNr"": 0,
                        ""real"": 1.0,
                        ""imaginary"" : 1.0
                    }
                    ]
                },
{
                    ""waveLength"" : 1310,
                    ""connections"": [
                    ]
                },
{
                    ""waveLength"" : 980,
                    ""connections"": [
                    ]
                }
            ],
            ""overlays"":[
            {
                ""overlayAnimTexturePath"" : ""res://dummyPath.png"",
                ""rectSide"" : 2,
                ""tileOffsetX"" : 0,
                ""tileOffsetY"" : 1
            }
        ]}";
        [Fact]
        public async Task SaveAndLoadGridManager()
        {
            // prepare
            GridManager grid = new(24, 12);
            ComponentFactory componentFactory = InitializeComponentFactory(grid);
            GridPersistenceManager gridPersistenceManager = new(grid, new FileDataAccessor());
            var inputs = grid.GetAllExternalInputs();
            int inputHeight = inputs.FirstOrDefault()?.TilePositionY ?? throw new Exception("there is no StandardInput defined");
            var firstComponent = TestComponentFactory.CreateStraightWaveGuide();
            grid.PlaceComponent(0, inputHeight, firstComponent);
            var secondComponent = ExportNazcaTests.PlaceAndConcatenateComponent(grid, firstComponent);
            var thirdComponent = ExportNazcaTests.PlaceAndConcatenateComponent(grid, secondComponent);
            var fourthComponent = ExportNazcaTests.PlaceAndConcatenateComponent(grid, thirdComponent);
            var orphan = TestComponentFactory.CreateStraightWaveGuide();
            var orphanPos = new IntVector(10, 5);
            grid.PlaceComponent(orphanPos.X, orphanPos.Y, orphan);
            

            string tempSavePath = Path.GetTempFileName();
            await gridPersistenceManager.SaveAsync(tempSavePath);
            var firstComponentConnections = grid.GetComponentAt(0, inputHeight).LaserWaveLengthToSMatrixMap.Values.First().GetNonNullValues();
            await gridPersistenceManager.LoadAsync(tempSavePath, componentFactory);
            
            // Asserts
            grid.GetComponentAt(0, inputHeight).Rotation90CounterClock.ShouldBe(firstComponent.Rotation90CounterClock);
            grid.GetComponentAt(0, inputHeight).Identifier.ShouldBe(firstComponent.Identifier);
            firstComponentConnections.First().Value.Real.ShouldBe(1);
            grid.GetComponentAt(orphanPos.X, orphanPos.Y).Rotation90CounterClock.ShouldBe(orphan.Rotation90CounterClock);
            grid.GetComponentAt(orphanPos.X, orphanPos.Y).Identifier.ShouldBe(orphan.Identifier);
        }

        private static ComponentFactory InitializeComponentFactory(GridManager grid)
        {
            var dummyJsonDataAccessor = new DummyDataAccessor(StraightWGJsonString);
            var straightComponentDraft = new ComponentDraftFileReader(dummyJsonDataAccessor).TryReadJson("").draft;
            if(straightComponentDraft == null)
            {
                throw new Exception("JSON could not be parsed");
            }
            var drafts = new List<ComponentDraft>() { straightComponentDraft };
            var validator = new ComponentDraftValidator(dummyJsonDataAccessor);
            string draftErrors = "";
            foreach (var item in drafts.Select(d => validator.Validate(d)).ToList())
            {
                draftErrors += item.errorMsg;
            };
            if(String.IsNullOrEmpty(draftErrors)==false)
                throw new Exception(draftErrors);

            var draftConverter = new ComponentDraftConverter(new Logger());
            var componentDrafts = draftConverter.ToComponentModels(drafts);
            var componentFactory = new ComponentFactory();
            componentFactory.InitializeComponentDrafts(componentDrafts);
            return componentFactory;
        }
    }
}
