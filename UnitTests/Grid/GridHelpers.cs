using CAP_Core.Grid;
using Moq;
using UnitTests;

namespace UnitTests.Grid
{
    internal static class GridHelpers
    {
        public static GridManager InitializeGridWithComponents(int width = 10, int height = 10)
        {
            // create Mock objects
            var mockTileManager = new Mock<ITileManager>(width, height);
            var mockComponentMover = new Mock<IComponentMover>();
            var mockExternalPortManager = new Mock<IExternalPortManager>();
            var mockComponentRotator = new Mock<IComponentRotator>();
            var mockComponentRelationshipManager = new Mock<IComponentRelationshipManager>();

            // initialize GridManager with Mocks
            var grid = new GridManager(mockTileManager.Object, mockComponentMover.Object, mockExternalPortManager.Object, mockComponentRotator.Object, mockComponentRelationshipManager.Object);


            grid.ComponentMover.PlaceComponent(1, 1, TestComponentFactory.CreateComponent(TestComponentFactory.StraightWGJson));
            grid.ComponentMover.PlaceComponent(2, 2, TestComponentFactory.CreateComponent(TestComponentFactory.StraightWGJson));
            grid.ComponentMover.PlaceComponent(3, 3, TestComponentFactory.CreateComponent(TestComponentFactory.DirectionalCouplerJSON));
            return grid;
        }
    }
}
