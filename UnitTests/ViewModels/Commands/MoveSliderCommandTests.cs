using CAP_Core.Components;
using CAP_Core.Components.Creation;
using CAP_Core.Grid;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using ConnectAPIC.Scripts.ViewModel;
using ConnectAPIC.Scripts.ViewModel.Commands;
using Moq;
using Shouldly;

namespace UnitTests.ViewModels.Commands
{
    public class MoveSliderCommandTests
    {
        private readonly Mock<GridManager> gridManagerMock;
        private readonly MoveSliderCommand command;
        private Mock<GridViewModel> gridViewModelMock;
        private readonly float minValue = 0;
        private readonly float halfValue = 0.5f;
        private readonly float maxValue = 1;
        private readonly float invalidValue = 16;
        private readonly float acceptableError = 0.01f;

        public MoveSliderCommandTests()
        {
            var tileMgr = new TileManager(24, 12);
            var lightMgr = new LightManager();
            var componentMover = new Mock<IComponentMover>();
            var externalPortMgr = new Mock<IExternalPortManager>();
            var componentRotator = new Mock<IComponentRotator>();
            var componentRelationshipMgr = new Mock<IComponentRelationshipManager>();
            gridManagerMock = new Mock<GridManager>(tileMgr, componentMover.Object, externalPortMgr.Object, componentRotator.Object, componentRelationshipMgr.Object, lightMgr);
            command = new MoveSliderCommand(gridManagerMock.Object);
            gridViewModelMock = new Mock<GridViewModel>();

            componentMover.Setup(m => m.GetComponentAt(It.IsAny<int>(), It.IsAny<int>(), 1, 1))
                .Returns<int, int, int, int>((x, y, width, height)
                    =>
                    {
                        if (x < 0 || y < 0) return null;
                        return tileMgr.Tiles[x, y].Component;
                    });

            componentMover.Setup(m => m.PlaceComponent(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Component>()))
                .Callback<int, int, Component>((x, y, comp)
                    => tileMgr.Tiles[x, y].Component = comp);
        }

        [Fact]
        public void CanExecute_ReturnsFalse_WhenArgumentIsNotMoveSliderCommandArgs()
        {
            var result = command.CanExecute(new object());
            Assert.False(result);
        }

        [Fact]
        public void CanExecute_ReturnsFalse_WhenMoveSliderIsInvalid()
        {
            gridManagerMock.Object.ComponentMover.PlaceComponent(1, 0, TestComponentFactory.CreateComponent(TestComponentFactory.StraightWGJson));
            gridManagerMock.Object.ComponentMover.PlaceComponent(5, 5, TestComponentFactory.CreateComponent(TestComponentFactory.DirectionalCouplerJSON));


            // arrange
            var argsOnNonSliderComponent = new MoveSliderCommandArgs(1, 0, 0, halfValue);
            var argsOutOfGrid = new MoveSliderCommandArgs(0, -1, -1, halfValue);
            var argsOnEmptyTile = new MoveSliderCommandArgs(0, 0, 0, halfValue);
            var argsOnInvalidSliderValue = new MoveSliderCommandArgs(5, 5, 0, invalidValue);

            // act
            var canSlideNonSliderComponent = command.CanExecute(argsOnNonSliderComponent);
            var canExecuteOutOfGrid = command.CanExecute(argsOutOfGrid);
            var canExecuteOnEmptyTile = command.CanExecute(argsOnEmptyTile);
            var canExecuteOnInvalidSliderValue = command.CanExecute(argsOnInvalidSliderValue);

            // assert
            canSlideNonSliderComponent.ShouldBeFalse();
            canExecuteOutOfGrid.ShouldBeFalse();
            canExecuteOnEmptyTile.ShouldBeFalse();
            canExecuteOnInvalidSliderValue.ShouldBeTrue();
        }

        [Fact]
        public async Task TestExecute()
        {
            gridManagerMock.Object.ComponentMover.PlaceComponent(1, 0, TestComponentFactory.CreateComponent(TestComponentFactory.DirectionalCouplerJSON));

            // arrange
            var argsOnMoveToMin = new MoveSliderCommandArgs(1, 0, 0, minValue);
            var argsOnMoveToHalf = new MoveSliderCommandArgs(1, 0, 0, halfValue);
            var argsOnMoveToMax = new MoveSliderCommandArgs(1, 0, 0, maxValue);


            // act
            await command.ExecuteAsync(argsOnMoveToMin);
            double valueOnSetMin = gridManagerMock.Object.TileManager.Tiles[1, 0].Component.GetSlider(0).Value;
            await command.ExecuteAsync(argsOnMoveToMax);
            double valueOnSetMax = gridManagerMock.Object.TileManager.Tiles[1, 0].Component.GetSlider(0).Value;
            await command.ExecuteAsync(argsOnMoveToHalf);
            double valueOnSetHalf = gridManagerMock.Object.TileManager.Tiles[1, 0].Component.GetSlider(0).Value;

            command.Undo();
            double valueAfterUndo = gridManagerMock.Object.TileManager.Tiles[1, 0].Component.GetSlider(0).Value;

            // assert
            valueOnSetMin.ShouldBeInRange<double>(minValue - acceptableError, minValue + acceptableError);
            valueOnSetMax.ShouldBeInRange<double>(maxValue - acceptableError, maxValue + acceptableError);
            valueOnSetHalf.ShouldBeInRange<double>(halfValue - acceptableError, halfValue + acceptableError);
            valueAfterUndo.ShouldBeInRange<double>(maxValue - acceptableError, maxValue + acceptableError);
        }
    }

}
