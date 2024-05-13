using CAP_Core.Components;
using CAP_Core.Components.ComponentHelpers;
using CAP_Core.Components.Creation;
using CAP_Core.Grid;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using Moq;

namespace UnitTests.ViewModels.Commands
{
    public class CreateComponentCommandTests
    {
        private readonly Mock<GridManager> gridManagerMock;
        private readonly CreateComponentCommand command;

        public CreateComponentCommandTests()
        {
            var tileMgr = new TileManager(24, 12);
            var lightMgr = new LightManager();
            var componentMover = new Mock<IComponentMover>();
            var externalPortMgr = new Mock<IExternalPortManager>();
            var componentRotator = new Mock<IComponentRotator>();
            var componentRelationshipMgr = new Mock<IComponentRelationshipManager>();
            gridManagerMock = new Mock<GridManager>(tileMgr, componentMover.Object, externalPortMgr.Object, componentRotator.Object, componentRelationshipMgr.Object, lightMgr);

            command = new CreateComponentCommand(gridManagerMock.Object, InitializeComponentFactory());


            componentMover.Setup(m => m.GetComponentAt(It.IsAny<int>(), It.IsAny<int>(), 1, 1))
                .Returns<int, int, int, int>((x, y, width, height)
                    => tileMgr.Tiles[x, y].Component);

            componentMover.Setup(m => m.UnregisterComponentAt(It.IsAny<int>(), It.IsAny<int>()))
                .Callback<int, int>((x, y)
                    => tileMgr.Tiles[x, y].Component = null);

            componentMover.Setup(m => m.IsColliding(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Component>()))
                .Returns<int, int, int, int, Component>((x, y, width, height, comp)
                    =>
                {
                    if (x < 0 || y < 0) return true;
                    if (tileMgr.Tiles[x, y].Component != null) return true;
                    return false;
                });

            componentMover.Setup(m => m.PlaceComponent(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Component>()))
            .Callback<int, int, Component>((x, y, comp)
            =>
            {
                if (componentMover.Object.IsColliding(x, y, comp.WidthInTiles, comp.HeightInTiles))
                {
                    var blockingComponent = componentMover.Object.GetComponentAt(x, y);
                    throw new ComponentCannotBePlacedException(comp, blockingComponent);
                }
                comp.RegisterPositionInGrid(x, y);

                for (int i = 0; i < comp.WidthInTiles; i++)
                {
                    for (int j = 0; j < comp.HeightInTiles; j++)
                    {
                        int gridX = x + i;
                        int gridY = y + j;
                        tileMgr.Tiles[gridX, gridY].Component = comp;
                    }
                }
            });
        }

        [Fact]
        public void CanExecute_ReturnsFalse_WhenArgumentIsNotCreateComponentArgs()
        {
            var result = command.CanExecute(new object());
            Assert.False(result);
        }

        [Fact]
        public void CanExecute_ReturnsFalse_WhenCreateIsInvalid()
        {
            gridManagerMock.Object.ComponentMover.PlaceComponent(1, 0, TestComponentFactory.CreateComponent(TestComponentFactory.StraightWGJson));

            // arrange
            var argsOnOtherComponent = new CreateComponentArgs(0, 1, 0, DiscreteRotation.R0, Guid.NewGuid());
            var argsOutOfGrid = new CreateComponentArgs(0, -1, 0, DiscreteRotation.R0, Guid.NewGuid());

            // act
            var canExecuteOnAnotherComponent = command.CanExecute(argsOnOtherComponent);
            var canExecuteOutOfGrid = command.CanExecute(argsOutOfGrid);

            // assert
            Assert.False(canExecuteOnAnotherComponent);
            Assert.False(canExecuteOutOfGrid);
        }

        [Fact]
        public async Task TestExecute()
        {
            // arrange
            var args = new CreateComponentArgs(0, 4, 4, DiscreteRotation.R0, Guid.NewGuid());

            // act
            await command.ExecuteAsync(args);

            bool componentPlaced = gridManagerMock.Object.TileManager.Tiles[4, 4].Component != null;

            command.Undo();

            // assert
            Assert.True(componentPlaced);
            Assert.True(gridManagerMock.Object.TileManager.Tiles[4, 4].Component == null);
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
