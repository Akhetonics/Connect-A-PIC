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
            var componentMover = new ComponentMover(tileMgr);//new Mock<IComponentMover>();
            var externalPortMgr = new Mock<IExternalPortManager>();
            var componentRotator = new Mock<IComponentRotator>();
            var componentRelationshipMgr = new Mock<IComponentRelationshipManager>();
            gridManagerMock = new Mock<GridManager>(tileMgr, componentMover, externalPortMgr.Object, componentRotator.Object, componentRelationshipMgr.Object, lightMgr);

            command = new DeleteComponentCommand(gridManagerMock.Object);
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
            await command.ExecuteAsync(argsOnBigComponent);
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
            Assert.True(deletedBigComponent);
            Assert.True(gridManagerMock.Object.TileManager.Tiles[1, 0].Component != null);
        }
    }
}
