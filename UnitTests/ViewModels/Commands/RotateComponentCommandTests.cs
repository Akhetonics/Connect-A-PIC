using CAP_Core.Components;
using CAP_Core.Components.ComponentHelpers;
using CAP_Core.Grid;
using CAP_Core.Helpers;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using Moq;

namespace UnitTests.ViewModels.Commands
{
    public class RotateComponentCommandTests
    {
        private readonly Mock<GridManager> gridManagerMock;
        private readonly RotateComponentCommand rotateCommand;

        public RotateComponentCommandTests()
        {
            var tileMgr = new TileManager(24, 12);
            var lightMgr = new LightManager();
            var componentMover = new Mock<IComponentMover>();
            var externalPortMgr = new Mock<IExternalPortManager>();
            var componentRotator = new Mock<IComponentRotator>();
            var componentRelationshipMgr = new Mock<IComponentRelationshipManager>();
            gridManagerMock = new Mock<GridManager>(tileMgr, componentMover.Object, externalPortMgr.Object, componentRotator.Object, componentRelationshipMgr.Object, lightMgr);

            rotateCommand = new RotateComponentCommand(gridManagerMock.Object);

     

            componentMover.Setup(m => m.GetComponentAt(It.IsAny<int>(), It.IsAny<int>(), 1, 1))
               .Returns<int, int, int, int>((x, y, width, height)
                   =>
               {
                   if (x < 0 || y < 0) return null;
                   return tileMgr.Tiles[x, y].Component;
               });

            componentMover.Setup(m => m.IsColliding(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Component>()))
            .Returns<int, int, int, int, Component>((x, y, width, height, comp)
                =>
                {
                    if (x < 0 || y < 0) return true;
                    if (tileMgr.Tiles[x, y].Component != null) return true;
                    return false;
                });

            componentMover.Setup(m => m.UnregisterComponentAt(It.IsAny<int>(), It.IsAny<int>()))
                .Callback<int, int>((x, y)
                    =>
                {
                    Component? component = componentMover.Object.GetComponentAt(x,y);
                    if (component == null) return;
                    x = component.GridXMainTile;
                    y = component.GridYMainTile;
                    for (int i = 0; i < component.WidthInTiles; i++)
                    {
                        for (int j = 0; j < component.HeightInTiles; j++)
                        {
                            tileMgr.Tiles[x + i, y + j].Component = null;
                        }
                    }
                    component.ClearGridData();

                });

            componentMover.Setup(m => m.PlaceComponent(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Component>()))
            .Callback<int, int, Component>((x, y, comp)
            =>
            {
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


            componentRotator.Setup(m => m.RotateComponentBy90CounterClockwise(It.IsAny<int>(), It.IsAny<int>()))
                .Returns<int, int>((tileX, tileY)
                => {
                    Component? component = componentMover.Object.GetComponentAt(tileX, tileY);
                    if (component == null) return false;
                    var tile = tileMgr.Tiles[tileX, tileY];
                    if (tile == null || tile.Component == null) return false;

                    var rotatedComponent = tile.Component;
                    int x = tile.Component.GridXMainTile;
                    int y = tile.Component.GridYMainTile;
                    componentMover.Object.UnregisterComponentAt(tile.GridX, tile.GridY);
                    rotatedComponent.RotateBy90CounterClockwise();
                    try
                    {
                        componentMover.Object.PlaceComponent(x, y, rotatedComponent);
                        return true;
                    }
                    catch (ComponentCannotBePlacedException)
                    {
                        rotatedComponent.Rotation90CounterClock--;
                        componentMover.Object.PlaceComponent(x, y, rotatedComponent);
                        return false;
                    }

                });
        }

        [Fact]
        public void CanExecute_ReturnsFalse_WhenArgumentIsNotRotateComponentArgs()
        {
            var result = rotateCommand.CanExecute(new object());
            Assert.False(result);
        }

        [Fact]
        public void CanExecute_ReturnsFalse_WhenRotateIsInvalid()
        {
            // arrange
            var argsOnEmptyTile = new RotateComponentArgs(1, 1);
            var argsOutOfGrid = new RotateComponentArgs(-1, 0);

            // act
            var canExecuteOnEmptyTile = rotateCommand.CanExecute(argsOnEmptyTile);
            var canExecuteOutOfGrid = rotateCommand.CanExecute(argsOutOfGrid);

            // assert
            Assert.False(canExecuteOnEmptyTile);
            Assert.False(canExecuteOutOfGrid);
        }

        [Fact]
        public async Task TestExecute()
        {
            var MMIPos = new IntVector(5, 0);
            var StraightPos = new IntVector(8, 3);
            gridManagerMock.Object.ComponentMover.PlaceComponent(MMIPos.X, MMIPos.Y, TestComponentFactory.CreateComponent(TestComponentFactory.MMI3x3));
            gridManagerMock.Object.ComponentMover.PlaceComponent(StraightPos.X, StraightPos.Y, TestComponentFactory.CreateComponent(TestComponentFactory.StraightWGJson));

            // arrange
            var argsOnBigComponent = new RotateComponentArgs(MMIPos.X+2, MMIPos.Y+2);
            var argsOnSingleComponent = new RotateComponentArgs(StraightPos.X, StraightPos.Y);

            // act
            await rotateCommand.ExecuteAsync(argsOnBigComponent);
            await rotateCommand.ExecuteAsync(argsOnSingleComponent);

            bool rotatedBigComponent = gridManagerMock.Object.TileManager.Tiles[MMIPos.X, MMIPos.Y].Component?.Rotation90CounterClock == DiscreteRotation.R90;
            bool rotatedSingleComponent = gridManagerMock.Object.TileManager.Tiles[StraightPos.X, StraightPos.Y].Component?.Rotation90CounterClock == DiscreteRotation.R90;
            
            // undoes single deleted component
            rotateCommand.Undo();
            var StraightRot = gridManagerMock.Object.TileManager.Tiles[StraightPos.X, StraightPos.Y].Component?.Rotation90CounterClock;
            bool isComponentRotatedBack = StraightRot == DiscreteRotation.R0;

            // assert
            Assert.True(rotatedSingleComponent);
            Assert.True(rotatedBigComponent);
            Assert.True(isComponentRotatedBack);
        }
    }
}
