using CAP_Core.Grid;
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
            gridManagerMock = new Mock<GridManager>();
            selectionManagerMock = new Mock<SelectionManager>(gridManagerMock.Object);
            command = new MoveComponentCommand(gridManagerMock.Object, selectionManagerMock.Object);
        }

        [Fact]
        public void CanExecute_ReturnsFalse_WhenArgumentIsNotMoveComponentArgs()
        {
            var result = command.CanExecute(new object());
            Assert.False(result);
        }
    }

}
