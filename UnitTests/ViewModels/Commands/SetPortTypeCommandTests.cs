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
        [Fact]
        public async Task TestChangingExternalPortType()
        {
            // arrange
            var gridManager = GridHelpers.InitializeGridWithComponents();
            SystemMatrixBuilder systemMatrixBuilder = new(gridManager);
            var lightCalculationService = new LightCalculationService(gridManager, new GridLightCalculator(systemMatrixBuilder, gridManager));
            var command = new SetPortTypeCommand(gridManager, lightCalculationService);
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
