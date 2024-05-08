using CAP_Core.Components;
using CAP_Core.Components.ComponentHelpers;
using CAP_Core.Grid;
using CAP_Core.Helpers;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using Moq;

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
            var componentMover = new ComponentMover(tileMgr);
            var externalPortMgr = new Mock<IExternalPortManager>();
            var componentRotator = new Mock<IComponentRotator>();
            var componentRelationshipMgr = new Mock<IComponentRelationshipManager>();
            gridManagerMock = new Mock<GridManager>(tileMgr, componentMover, externalPortMgr.Object, componentRotator.Object, componentRelationshipMgr.Object, lightMgr);
            var selectionManager = new Mock<ISelectionManager>(gridManagerMock.Object);
            command = new DeleteComponentCommand(gridManagerMock.Object, selectionManager.Object);
        }

        [Fact]
        public void CanExecute_ReturnsFalse_WhenArgumentIsNotDeleteComponentArgs()
        {
            var result = command.CanExecute(new object());
            Assert.False(result);
        }

        [Fact]
        public async Task TestExecute()
        {
            gridManagerMock.Object.ComponentMover.PlaceComponent(1, 0, TestComponentFactory.CreateComponent(TestComponentFactory.StraightWGJson));
            gridManagerMock.Object.ComponentMover.PlaceComponent(2, 0, TestComponentFactory.CreateComponent(TestComponentFactory.StraightWGJson));
            gridManagerMock.Object.ComponentMover.PlaceComponent(3, 0, TestComponentFactory.CreateComponent(TestComponentFactory.StraightWGJson));
            gridManagerMock.Object.ComponentMover.PlaceComponent(5, 0, TestComponentFactory.CreateComponent(TestComponentFactory.MMI3x3));

            // arrange
            var singleComponentPos = new List<IntVector>() { new (1,0) };
            var multipleComponentsPos = new List<IntVector>() { new (2,0), new (3,0) };
            var bigComponentPos = new List<IntVector>() { new (6,1) };
            var argsOnMultipleComponents = new DeleteComponentArgs(multipleComponentsPos.ToHashSet(), Guid.NewGuid());
            var argsOnBigComponent = new DeleteComponentArgs(bigComponentPos.ToHashSet(), Guid.NewGuid());
            var argsOnSingleComponent = new DeleteComponentArgs(singleComponentPos.ToHashSet(), Guid.NewGuid());

            // act
            await command.ExecuteAsync(argsOnMultipleComponents);
            await command.ExecuteAsync(argsOnBigComponent);
            await command.ExecuteAsync(argsOnSingleComponent);


            bool deletedSingleComponent = gridManagerMock.Object.TileManager.Tiles[singleComponentPos.First().X, singleComponentPos.First().Y].Component == null;
            bool deletedMultiple = gridManagerMock.Object.TileManager.Tiles[multipleComponentsPos[0].X, multipleComponentsPos[0].X].Component == null
                                    && gridManagerMock.Object.TileManager.Tiles[multipleComponentsPos[1].X, multipleComponentsPos[1].X].Component == null;
            bool deletedBigComponent = gridManagerMock.Object.TileManager.Tiles[bigComponentPos[0].X, bigComponentPos[0].Y].Component == null;

            // undoes single deleted component
            command.Undo();
            command.Redo();
            command.Undo(); // undo again
            command.Redo();
            command.Undo(); // undo again
            // assert
            Assert.True(deletedSingleComponent);
            Assert.True(deletedMultiple);
            Assert.True(deletedBigComponent);
            Assert.True(gridManagerMock.Object.TileManager.Tiles[singleComponentPos[0].X, singleComponentPos[0].Y].Component != null);
        }
    }
}
