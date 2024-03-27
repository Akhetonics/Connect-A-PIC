using CAP_Contracts;
using CAP_Core;
using CAP_Core.Components;
using CAP_Core.Components.Creation;
using CAP_Core.Grid;
using CAP_Core.Helpers;
using CAP_DataAccess.Components.ComponentDraftMapper;
using CAP_DataAccess.Components.ComponentDraftMapper.DTOs;
using Shouldly;

namespace UnitTests.Grid
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
            return new Task<bool>(() => true);
        }
    }
    public class GridPersistenceManagerTests
    {

        [Fact]
        public async Task SaveAndLoadGridManager()
        {
            // prepare
            GridManager grid = GridHelpers.InitializeGridWithComponents();

            ComponentFactory componentFactory = InitializeComponentFactory();
            GridPersistenceManager gridPersistenceManager = new(grid, new FileDataAccessor());
            var inputs = grid.ExternalPortManager.GetAllExternalInputs();
            int inputHeight = inputs.FirstOrDefault()?.TilePositionY ?? throw new Exception("there is no StandardInput defined");
            var firstComponent = TestComponentFactory.CreateComponent(TestComponentFactory.StraightWGJson);
            var firstSlider = new Slider(Guid.NewGuid(), 1, 0.1337, 1, 0.1);
            firstComponent.AddSlider(1, firstSlider);
            var sliderCountBeforeSaving = firstComponent.GetAllSliders().Count;
            grid.ComponentMover.PlaceComponent(0, inputHeight, firstComponent);
            var secondComponent = ExportNazcaTests.PlaceAndConcatenateComponent(grid, firstComponent);
            var thirdComponent = ExportNazcaTests.PlaceAndConcatenateComponent(grid, secondComponent);
            var fourthComponent = ExportNazcaTests.PlaceAndConcatenateComponent(grid, thirdComponent);
            var orphan = TestComponentFactory.CreateComponent(TestComponentFactory.StraightWGJson);
            var orphanPos = new IntVector(10, 5);
            grid.ComponentMover.PlaceComponent(orphanPos.X, orphanPos.Y, orphan);


            string tempSavePath = Path.GetTempFileName();
            await gridPersistenceManager.SaveAsync(tempSavePath);
            var firstComponentConnections = grid.ComponentMover.GetComponentAt(0, inputHeight).WaveLengthToSMatrixMap.Values.First().GetNonNullValues();
            await gridPersistenceManager.LoadAsync(tempSavePath, componentFactory);
            var firstComponentFromGrid = grid.ComponentMover.GetComponentAt(0, inputHeight);
            var sliderFromGrid = firstComponentFromGrid.GetSlider(firstSlider.Number);

            // Asserts
            firstComponentFromGrid.GetAllSliders().Count.ShouldBe(sliderCountBeforeSaving);
            firstComponentFromGrid.Rotation90CounterClock.ShouldBe(firstComponent.Rotation90CounterClock);
            sliderFromGrid.MinValue.ShouldBe(sliderFromGrid.MinValue);
            sliderFromGrid.MaxValue.ShouldBe(sliderFromGrid.MaxValue);
            sliderFromGrid.Value.ShouldBe(sliderFromGrid.Value);
            firstComponentFromGrid.Identifier.ShouldBe(firstComponent.Identifier);
            firstComponentConnections.First().Value.Magnitude.ShouldBe(1.0, 1e-4);
            grid.ComponentMover.GetComponentAt(orphanPos.X, orphanPos.Y).Rotation90CounterClock.ShouldBe(orphan.Rotation90CounterClock);
            grid.ComponentMover.GetComponentAt(orphanPos.X, orphanPos.Y).Identifier.ShouldBe(orphan.Identifier);
        }

        private static ComponentFactory InitializeComponentFactory()
        {
            var componentDrafts = new List<Component>() { TestComponentFactory.CreateComponent(TestComponentFactory.StraightWGJson) };
            var componentFactory = new ComponentFactory();
            componentFactory.InitializeComponentDrafts(componentDrafts);
            return componentFactory;
        }
    }
}
