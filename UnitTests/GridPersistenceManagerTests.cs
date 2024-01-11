using CAP_Core;
using CAP_Core.Components.Creation;
using CAP_Core.ExternalPorts;
using CAP_Core.Grid;
using CAP_DataAccess.Components.ComponentDraftMapper;
using Castle.Components.DictionaryAdapter.Xml;
using Components.ComponentDraftMapper;
using ConnectAPIC.Scripts.View.ComponentFactory;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static UnitTests.ComponentDraftFileReaderTests;

namespace UnitTests
{
    public class GridPersistenceManagerTests
    {
        private static string StraightWGJsonString = @"
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
            ""connections"": [
                {
                    ""fromPinNr"": 0,
                    ""toPinNr"": 1,
                    ""magnitude"": 1,
                    ""wireLengthNM"" : 250000.0
                },
                {
                    ""fromPinNr"": 1,
                    ""toPinNr"": 0,
                    ""magnitude"": 1,
                    ""wireLengthNM"" : 250000.0
                }
            ]
        }";
        [Fact]
        public async Task SaveAndLoadGridManager()
        {
            // prepare
            GridManager grid = new(24, 12);
            GridPersistenceManager gridPersistenceManager = LoadComponentDraftsAndInitializeFactory(grid);
            var inputs = grid.ExternalPorts.Where(p => p.GetType() == typeof(ExternalInput)).ToList();
            int inputHeight = inputs.FirstOrDefault()?.TilePositionY ?? throw new Exception("there is no StandardInput defined");
            var firstComponent = TestComponentFactory.CreateDirectionalCoupler();
            grid.PlaceComponent(0, inputHeight, firstComponent);
            var secondComponent = ExportNazcaTests.PlaceAndConcatenateComponent(grid, firstComponent);
            var thirdComponent = ExportNazcaTests.PlaceAndConcatenateComponent(grid, secondComponent);
            var fourthComponent = ExportNazcaTests.PlaceAndConcatenateComponent(grid, thirdComponent);
            var orphan = TestComponentFactory.CreateStraightWaveGuide();
            grid.PlaceComponent(10, 5, orphan);
            

            string tempSavePath = Path.GetTempFileName();
            await gridPersistenceManager.SaveAsync(tempSavePath);
            await gridPersistenceManager.LoadAsync(tempSavePath);
            
            // Asserts
            grid.GetComponentAt(0, inputHeight).Rotation90CounterClock.ShouldBe(firstComponent.Rotation90CounterClock);
            grid.GetComponentAt(0, inputHeight).Identifier.ShouldBe(firstComponent.Identifier);
            grid.GetComponentAt(orphan.GridXMainTile, orphan.GridYMainTile).Rotation90CounterClock.ShouldBe(firstComponent.Rotation90CounterClock);
            grid.GetComponentAt(orphan.GridXMainTile, orphan.GridYMainTile).Identifier.ShouldBe(firstComponent.Identifier);
        }

        private static GridPersistenceManager LoadComponentDraftsAndInitializeFactory(GridManager grid)
        {
            var dataAccessor = new FileDataAccessor();
            var directoryAccessor = new DirectoryDataAccessor();
            ComponentDraftFileReader reader = new (dataAccessor);
            ComponentJSONFinder finder = new (directoryAccessor, dataAccessor);
            var jsonFiles = finder.FindRecursively(Directory.GetCurrentDirectory()+@"..\..\..\..\..\..", "json");
            var drafts = jsonFiles.Select(f => reader.TryReadJson(f).draft).ToList();
            var draftConverter = new ComponentDraftConverter(new Logger());
            var componentDrafts = draftConverter.ToComponentModels(drafts);
            ComponentFactory.Instance.InitializeComponentDrafts(componentDrafts);
            GridPersistenceManager gridPersistenceManager = new(grid, dataAccessor, ComponentFactory.Instance);
            return gridPersistenceManager;
        }
    }
}
