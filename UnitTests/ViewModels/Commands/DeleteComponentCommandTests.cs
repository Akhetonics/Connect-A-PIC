using CAP_Core.Components;
using CAP_Core.Components.Creation;
using CAP_Core.Grid;
using CAP_Core.Helpers;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.ViewModels.Commands
{
    public class DeleteComponentCommandTests
    {
        private readonly Mock<GridManager> gridManagerMock;
        private readonly DeleteComponentCommand command;

        public DeleteComponentCommandTests()
        {
            var tileMgr = new TileManager(24, 12);
            var lightMgr = new LightManager();
            var componentMover = new Mock<IComponentMover>();
            var externalPortMgr = new Mock<IExternalPortManager>();
            var componentRotator = new Mock<IComponentRotator>();
            var componentRelationshipMgr = new Mock<IComponentRelationshipManager>();
            gridManagerMock = new Mock<GridManager>(tileMgr, componentMover.Object, externalPortMgr.Object,componentRotator.Object,componentRelationshipMgr.Object, lightMgr);

            command = new DeleteComponentCommand(gridManagerMock.Object);


            componentMover.Setup(m => m.GetComponentAt(It.IsAny<int>(), It.IsAny<int>(), 1, 1))
                .Returns<int, int, int, int>((x, y, width, height)
                    => tileMgr.Tiles[x, y].Component);

            componentMover.Setup(m => m.UnregisterComponentAt(It.IsAny<int>(), It.IsAny<int>()))
                .Callback<int, int>((x, y)
                    => tileMgr.Tiles[x, y].Component = null);

            componentMover.Setup(m => m.PlaceComponent(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Component>()))
                .Callback<int, int, Component>((x, y, comp)
                    => tileMgr.Tiles[x, y].Component = comp);

            componentMover.Setup(m => m.IsColliding(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Component>()))
                .Returns<int, int, int, int, Component>((x, y, width, height, comp)
                    => {
                        if (x < 0 || y < 0) return true;
                        if (tileMgr.Tiles[x, y].Component != null) return true;
                        return false;
                    });
        }

        [Fact]
        public void CanExecute_ReturnsFalse_WhenArgumentIsNotDeleteComponentArgs()
        {
            var result = command.CanExecute(new object());
            Assert.False(result);
        }

        [Fact]
        public void CanExecute_ReturnsFalse_WhenCreateIsInvalid()
        {  
            //// arrange
            //var onEmptyTile = new List<IntVector>() {
            //    new IntVector(1,0)
            //};
            //var outOfGrid = new List<IntVector>() {
            //    new IntVector(-1,0)
            //};

            //var argsOnEmptyTile = new DeleteComponentArgs(onEmptyTile);
            //var argsOutOfGrid = new DeleteComponentArgs(outOfGrid);

            //// act
            //var canExecuteOnEmptyTile = command.CanExecute(argsOnEmptyTile);
            //var canExecuteOutOfGrid = command.CanExecute(argsOutOfGrid);

            //// assert
            //Assert.False(canExecuteOnEmptyTile);
            //Assert.False(canExecuteOutOfGrid);
        }

        [Fact]
        public async Task TestExecute() {
            gridManagerMock.Object.ComponentMover.PlaceComponent(1, 0, TestComponentFactory.CreateComponent(TestComponentFactory.StraightWGJson));
            gridManagerMock.Object.ComponentMover.PlaceComponent(2, 0, TestComponentFactory.CreateComponent(TestComponentFactory.StraightWGJson));
            gridManagerMock.Object.ComponentMover.PlaceComponent(3, 0, TestComponentFactory.CreateComponent(TestComponentFactory.StraightWGJson));
            gridManagerMock.Object.ComponentMover.PlaceComponent(5, 0, TestComponentFactory.CreateComponent(TestComponentFactory.MMI3x3));

            // arrange
            var onSingleComponent = new List<IntVector>() {
                new IntVector(1,0)
            };
            var onMultipleComponents = new List<IntVector>() {
                new IntVector(2,0), new IntVector(3,0)
            };
            var onBigComponent = new List<IntVector>() {
                new IntVector(6,1)
            };

            var argsOnSingleComponent = new DeleteComponentArgs(onSingleComponent);
            var argsOnMultipleComponents = new DeleteComponentArgs(onMultipleComponents);
            var argsOnBigComponent = new DeleteComponentArgs(onBigComponent);

            // act
            await command.ExecuteAsync(argsOnMultipleComponents);
            //TODO: for this need better component checking in component mover
            //await command.ExecuteAsync(argsOnBigComponent);
            await command.ExecuteAsync(argsOnSingleComponent);


            bool deletedSingleComponent = gridManagerMock.Object.TileManager.Tiles[1, 0].Component == null;
            bool deletedMultiple = gridManagerMock.Object.TileManager.Tiles[2, 0].Component == null
                                    && gridManagerMock.Object.TileManager.Tiles[2, 1].Component == null;
            bool deletedBigComponent = gridManagerMock.Object.TileManager.Tiles[5, 0].Component == null;

            // undoes single deleted component
            command.Undo();

            // assert
            Assert.True(deletedSingleComponent);
            Assert.True(deletedMultiple);
            //TODO: for this need better component checking in component mover
            //Assert.True(deletedBigComponent);
            Assert.True(gridManagerMock.Object.TileManager.Tiles[1, 0].Component != null);
        }
    }
}
