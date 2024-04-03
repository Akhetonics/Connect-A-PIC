using CAP_Core.Components;
using CAP_Core.Grid;
using CAP_Core.Helpers;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.ViewModels.Commands
{
    public class MoveComponentCommandTests
    {
        private readonly Mock<GridManager> gridManagerMock;
        private readonly Mock<SelectionManager> selectionManagerMock;
        private readonly MoveComponentCommand command;

        public MoveComponentCommandTests()
        {
            // IExternalPortManager externalPortManager, IComponentRotator componentRotator, IComponentRelationshipManager componentRelationshipManager
            var tileMgr = new TileManager(24, 12);
            var componentMover = new Mock<IComponentMover>();
            var externalPortMgr = new Mock<IExternalPortManager>();
            var componentRotator = new Mock<IComponentRotator>();
            var componentRelationshipMgr = new Mock<IComponentRelationshipManager>();
            gridManagerMock = new Mock<GridManager>(tileMgr, componentMover.Object, externalPortMgr.Object,componentRotator.Object,componentRelationshipMgr.Object);
            selectionManagerMock = new Mock<SelectionManager>(gridManagerMock.Object);
            command = new MoveComponentCommand(gridManagerMock.Object, selectionManagerMock.Object);

            componentMover.Setup(m => m.GetComponentAt(It.IsAny<int>(), It.IsAny<int>() , 1 ,1 ))
                .Returns<int,int,int,int>((x,y,width,height)
                    => tileMgr.Tiles[x, y].Component );
            componentMover.Setup(m => m.UnregisterComponentAt(It.IsAny<int>(), It.IsAny<int>()))
                .Callback<int,int>((x,y)
                    => tileMgr.Tiles[x, y].Component = null);
            componentMover.Setup(m => m.PlaceComponent(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Component>()))
                .Callback<int, int, Component>((x, y, comp)
                    => tileMgr.Tiles[x, y].Component = comp);
            componentMover.Setup(m => m.IsColliding(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Component>()))
                .Returns<int, int, int, int, Component>((x, y, width, height, comp)
                    => {
                        if (tileMgr.Tiles[x, y].Component != null) return true;
                        return false;
                    });
        }

        [Fact]
        public void CanExecute_ReturnsFalse_WhenArgumentIsNotMoveComponentArgs()
        {
            var result = command.CanExecute(new object());
            Assert.False(result);
        }

        [Fact]
        public void CanExecute_ReturnsFalse_WhenMoveIsInvalid()
        {
            gridManagerMock.Object.ComponentMover.PlaceComponent(1, 0, TestComponentFactory.CreateComponent(TestComponentFactory.StraightWGJson));
            gridManagerMock.Object.ComponentMover.PlaceComponent(0, 1, TestComponentFactory.CreateComponent(TestComponentFactory.StraightWGJson));

            // arrange
            var transitionsDuplicateTarget = new List<(IntVector start, IntVector end)>() {
                (new IntVector(1,0), new IntVector(1,1)),
                (new IntVector(0,1), new IntVector(1,1))
            };
            var transitionsOutOfGrid = new List<(IntVector start, IntVector end)>() {
                (new IntVector(1,0), new IntVector(-1,0))
            };
            var transitionWorking = new List<(IntVector start, IntVector end)>() {
                (new IntVector(1,0), new IntVector(1,4)),
                (new IntVector(0,1), new IntVector(1,2))
            };
            var transitionOverrideForbidden = new List<(IntVector start, IntVector end)>() {
                (new IntVector(1,0), new IntVector(0,1))
            };

            var argsDuplicateTarget = new MoveComponentArgs(transitionsDuplicateTarget);
            var argsOutOfGrid = new MoveComponentArgs(transitionsOutOfGrid);
            var argsWorking = new MoveComponentArgs(transitionWorking);
            var argsOverrideForbidden = new MoveComponentArgs(transitionOverrideForbidden);
            // act
            var canExecuteDuplicateTarget = command.CanExecute(argsDuplicateTarget);
            var canExecuteOutOfGrid = command.CanExecute(argsOutOfGrid);
            var canExecuteWorking = command.CanExecute(argsWorking);
            var canExecuteOverrideForbidden = command.CanExecute(argsOverrideForbidden);

            // assert
            Assert.False(canExecuteDuplicateTarget);
            Assert.False(canExecuteOutOfGrid);
            Assert.True(canExecuteWorking);
            Assert.False(canExecuteOverrideForbidden);
        }
        [Fact]
        public async Task TestExecute()
        {
            gridManagerMock.Object.ComponentMover.PlaceComponent(1, 0, TestComponentFactory.CreateComponent(TestComponentFactory.StraightWGJson));
            gridManagerMock.Object.ComponentMover.PlaceComponent(0, 1, TestComponentFactory.CreateComponent(TestComponentFactory.StraightWGJson));
            // arrange
            var transitionWorking = new List<(IntVector start, IntVector end)>() {
                (new IntVector(1,0), new IntVector(1,4)),
                (new IntVector(0,1), new IntVector(1,2))
            };
            var argsWorking = new MoveComponentArgs(transitionWorking);
            // act
            await command.ExecuteAsync(argsWorking);

            // assert
            Assert.True(gridManagerMock.Object.TileManager.Tiles[1, 4].Component != null);
            Assert.True(gridManagerMock.Object.TileManager.Tiles[1, 2].Component != null);
        }
    }

}
