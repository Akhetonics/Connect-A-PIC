using CAP_Core.ExternalPorts;
using CAP_Core.Grid;
using CAP_Core.Helpers;
using CAP_Core.LightCalculation;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using ConnectAPIC.Scripts.Helpers;
using ConnectAPIC.Scripts.View.ToolBox;
using ConnectAPIC.Scripts.ViewModel.Commands.ExternalPorts;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTests.Grid;
using static UnitTests.Helpers.GridHelpers;

namespace UnitTests.ViewModels.Commands
{
    public class SetInputColorCommandTests
    {
        private readonly GridManager gridManager;
        private readonly SetInputColorCommand command;
        private readonly LightCalculationService lightCalculationService;

        public SetInputColorCommandTests()
        {
            gridManager = GridHelpers.InitializeGridWithComponents();
            SystemMatrixBuilder systemMatrixBuilder = new(gridManager);
            lightCalculationService = new LightCalculationService(gridManager, new GridLightCalculator(systemMatrixBuilder, gridManager));
            command = new SetInputColorCommand(gridManager);
        }

        [Fact]
        public void CanExecute_ReturnsFalse_WhenArgumentIsNotSetInputColorArgs()
        {
            var result = command.CanExecute(new object());
            Assert.False(result);
        }

        [Fact]
        public void CanExecute_ReturnsFalse_WhenSetColorIsInvalid()
        {
            // arrange
            var externalPort = gridManager.ExternalPortManager.ExternalPorts.Last();
            var portViewModel = new ConnectAPIC.Scripts.ViewModel.ExternalPortViewModel(gridManager, externalPort, lightCalculationService);

            var outputArgs = new SetInputColorArgs(portViewModel.PortModel as ExternalInput, LaserType.Blue);

            // act
            var canExecuteOnOutput = command.CanExecute(outputArgs);

            // assert
            canExecuteOnOutput.ShouldBeFalse();
        }

        [Fact]
        public async Task TestExecute()
        {
            // arrange
            var externalPort = gridManager.ExternalPortManager.ExternalPorts.First();
            var portViewModel = new ConnectAPIC.Scripts.ViewModel.ExternalPortViewModel(gridManager, externalPort, lightCalculationService);

            var setToBlueArgs = new SetInputColorArgs(portViewModel.PortModel as ExternalInput, LaserType.Blue);
            var setToGreenArgs = new SetInputColorArgs(portViewModel.PortModel as ExternalInput, LaserType.Green);
            var setToRedArgs = new SetInputColorArgs(portViewModel.PortModel as ExternalInput, LaserType.Red);

            // act
            await command.ExecuteAsync(setToBlueArgs);
            var IsInputSetToBlue = portViewModel.Color == LaserType.Blue;
            await command.ExecuteAsync(setToGreenArgs);
            var IsInputSetToGreen = portViewModel.Color == LaserType.Green;
            await command.ExecuteAsync(setToRedArgs);
            var IsInputSetToRed = portViewModel.Color == LaserType.Red;

            command.Undo();
            var isBackToGreen = portViewModel.Color == LaserType.Green;

            // assert
            IsInputSetToBlue.ShouldBeTrue();
            IsInputSetToGreen.ShouldBeTrue();
            IsInputSetToRed.ShouldBeTrue();
            isBackToGreen.ShouldBeTrue();
        }
    }
}
