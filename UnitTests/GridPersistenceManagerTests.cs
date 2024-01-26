using CAP_Contracts;
using CAP_Core;
using CAP_Core.Components.Creation;
using CAP_Core.Grid;
using CAP_Core.Helpers;
using CAP_DataAccess.Components.ComponentDraftMapper;
using CAP_DataAccess.Components.ComponentDraftMapper.DTOs;
using Shouldly;

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
            var dummyJsonDataAccessor = new DummyDataAccessor(TestComponentFactory.StraightWGJsonString);
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
