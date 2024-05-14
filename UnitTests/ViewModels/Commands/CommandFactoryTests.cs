using CAP_Contracts.Logger;
using CAP_Core.Components;
using CAP_Core.Components.ComponentHelpers;
using CAP_Core.Components.Creation;
using CAP_Core.Grid;
using CAP_Core.LightCalculation;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using ConnectAPIC.Scripts.ViewModel.CommandFactory;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.ViewModels.Commands
{
    public class CommandFactoryTests
    {
        private readonly Mock<GridManager> gridManagerMock;
        private readonly CommandFactory commandFactory;
        public CommandFactoryTests()
        {
            var tileMgr = new TileManager(24, 12);
            var lightMgr = new LightManager();
            var componentMover = new Mock<IComponentMover>();
            var externalPortMgr = new Mock<IExternalPortManager>();
            var componentRotator = new Mock<IComponentRotator>();
            var componentRelationshipMgr = new Mock<IComponentRelationshipManager>();
            gridManagerMock = new Mock<GridManager>(tileMgr, componentMover.Object, externalPortMgr.Object, componentRotator.Object, componentRelationshipMgr.Object, lightMgr);

            var lightService = new Mock<ILightCalculationService>();
            var componentFactory = new Mock<IComponentFactory>();
            var selectionManager = new Mock<ISelectionManager>();
            var logger = new Mock<ILogger>();
            GridViewModel gridViewModel = new GridViewModel(gridManagerMock.Object, logger.Object, componentFactory.Object, lightService.Object);
            commandFactory = new CommandFactory(gridManagerMock.Object, componentFactory.Object, selectionManager.Object, logger.Object, lightService.Object, gridViewModel);

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
        public void UndoAndRedoSequencesOfCommandsTest()
        {
            // arrange


            // act

            // assert

        }
    }
}
