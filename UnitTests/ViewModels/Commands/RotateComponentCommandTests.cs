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
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.ViewModels.Commands
{
    public class RotateComponentCommandTests
    {
        private readonly Mock<GridManager> gridManagerMock;
        private readonly RotateComponentCommand command;

        public RotateComponentCommandTests()
        {
            var tileMgr = new TileManager(24, 12);
            var lightMgr = new LightManager();
            var componentMover = new ComponentMover(tileMgr);
            var externalPortMgr = new Mock<IExternalPortManager>();
            var componentRotator = new ComponentRotator(tileMgr, componentMover);//new Mock<IComponentRotator>();
            var componentRelationshipMgr = new Mock<IComponentRelationshipManager>();
            gridManagerMock = new Mock<GridManager>(tileMgr, componentMover, externalPortMgr.Object,componentRotator,componentRelationshipMgr.Object, lightMgr);

            command = new RotateComponentCommand(gridManagerMock.Object);
        }

        [Fact]
        public void CanExecute_ReturnsFalse_WhenArgumentIsNotRotateComponentArgs()
        {
            var result = command.CanExecute(new object());
            Assert.False(result);
        }

        [Fact]
        public void CanExecute_ReturnsFalse_WhenRotateIsInvalid()
        {
            // arrange
            var argsOnEmptyTile = new RotateComponentArgs(1, 1);
            var argsOutOfGrid = new RotateComponentArgs(-1, 0);

            // act
            var canExecuteOnEmptyTile = command.CanExecute(argsOnEmptyTile);
            var canExecuteOutOfGrid = command.CanExecute(argsOutOfGrid);

            // assert
            Assert.False(canExecuteOnEmptyTile);
            Assert.False(canExecuteOutOfGrid);
        }

        [Fact]
        public async Task TestExecute() {
            gridManagerMock.Object.ComponentMover.PlaceComponent(5, 0, TestComponentFactory.CreateComponent(TestComponentFactory.MMI3x3));
            gridManagerMock.Object.ComponentMover.PlaceComponent(1, 0, TestComponentFactory.CreateComponent(TestComponentFactory.StraightWGJson));

            // arrange
            var argsOnBigComponent = new RotateComponentArgs(6, 1);
            var argsOnSingleComponent = new RotateComponentArgs(1, 0);

            // act
            await command.ExecuteAsync(argsOnBigComponent);
            await command.ExecuteAsync(argsOnSingleComponent);


            bool rotatedBigComponent = gridManagerMock.Object.TileManager.Tiles[5, 0].Component?.Rotation90CounterClock == DiscreteRotation.R90;
            bool rotatedSingleComponent = gridManagerMock.Object.TileManager.Tiles[1, 0].Component?.Rotation90CounterClock == DiscreteRotation.R90;

            // undoes single deleted component
            command.Undo();

            // assert
            Assert.True(rotatedSingleComponent);
            Assert.True(rotatedBigComponent);
            Assert.True(gridManagerMock.Object.TileManager.Tiles[1, 0].Component?.Rotation90CounterClock == DiscreteRotation.R0);
        }
    }
}
