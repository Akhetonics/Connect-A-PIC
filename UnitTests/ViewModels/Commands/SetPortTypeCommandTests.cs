using CAP_Core.ExternalPorts;
using CAP_Core.Grid;
using CAP_Core.Helpers;
using CAP_Core.LightCalculation;
using ConnectAPIC.Scripts.Helpers;
using ConnectAPIC.Scripts.View.ToolBox;
using ConnectAPIC.Scripts.ViewModel.Commands.ExternalPorts;
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
    public class SetPortTypeCommandTests
    {
        private readonly GridManager gridManager;
        private readonly SetPortTypeCommand command;
        private readonly LightCalculationService lightCalculationService;

        public SetPortTypeCommandTests()
        {
            gridManager = GridHelpers.InitializeGridWithComponents();
            SystemMatrixBuilder systemMatrixBuilder = new(gridManager);
            lightCalculationService = new LightCalculationService(gridManager, new GridLightCalculator(systemMatrixBuilder, gridManager));
            command = new SetPortTypeCommand(gridManager, lightCalculationService);
        }

        [Fact]
        public void CanExecute_ReturnsFalse_WhenArgumentIsNotSetPortTypeArgs()
        {
            var result = command.CanExecute(new object());
            Assert.False(result);
        }

        [Fact]
        public async Task TestExecute()
        {
            // arrange
            var externalPort = gridManager.ExternalPortManager.ExternalPorts.First();
            var portViewModel = new ConnectAPIC.Scripts.ViewModel.ExternalPortViewModel(gridManager, externalPort, lightCalculationService);
            var setToOutputParams = new SetPortTypeArgs(portViewModel,true);
            var setToInputParams = new SetPortTypeArgs(portViewModel,false);

            // act
            await command.ExecuteAsync(setToOutputParams);
            var IsInputAfterSetToOutput = portViewModel.IsInput;
            await command.ExecuteAsync(setToInputParams);
            var IsInputAfterSetToInput = portViewModel.IsInput;

            // assert
            IsInputAfterSetToOutput.ShouldBeFalse();
            IsInputAfterSetToInput.ShouldBeTrue();
        }
    }
}
