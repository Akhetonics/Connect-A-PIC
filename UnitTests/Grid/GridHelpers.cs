using CAP_Core.Grid;
using Moq;
using UnitTests;

namespace UnitTests.Grid
{
    internal static class GridHelpers
    {
        public static GridManager InitializeGridWithComponents(int width = 10, int height = 10)
        {
            var grid = new GridManager(width, height);
            grid.ComponentMover.PlaceComponent(1, 1, TestComponentFactory.CreateComponent(TestComponentFactory.StraightWGJson));
            grid.ComponentMover.PlaceComponent(2, 2, TestComponentFactory.CreateComponent(TestComponentFactory.StraightWGJson));
            grid.ComponentMover.PlaceComponent(3, 3, TestComponentFactory.CreateComponent(TestComponentFactory.DirectionalCouplerJSON));
            return grid;
        }
    }
}
