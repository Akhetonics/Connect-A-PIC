using CAP_Core.ExternalPorts;
using CAP_Core.Grid;
using CAP_Core.LightCalculation;
using ConnectAPIC.Scripts.ViewModel.Commands.ExternalPorts;
using Shouldly;
using UnitTests.Grid;

namespace UnitTests.ViewModels.Commands
{
    public class SetInputPowerCommandTests
    {
        private readonly GridManager gridManager;
        private readonly SetInputPowerCommand command;
        private readonly LightCalculationService lightCalculationService;

        private readonly float maxPower = 1f;
        private readonly float halfPower = 0.5f;
        private readonly float minPower = 0f;
        private readonly float toleranceMargins = 0.01f;

        public SetInputPowerCommandTests()
        {
            gridManager = GridHelpers.InitializeGridWithComponents();
            SystemMatrixBuilder systemMatrixBuilder = new(gridManager);
            lightCalculationService = new LightCalculationService(gridManager, new GridLightCalculator(systemMatrixBuilder, gridManager));
            command = new SetInputPowerCommand(gridManager, lightCalculationService);
        }

        [Fact]
        public void CanExecute_ReturnsFalse_WhenArgumentIsNotSetInputPowerArgs()
        {
            var result = command.CanExecute(new object());
            Assert.False(result);
        }

        [Fact]
        public void CanExecute_ReturnsFalse_WhenSetPowerIsInvalid()
        {
            // arrange
            var externalInput = gridManager.ExternalPortManager.ExternalPorts.First();
            var inputViewModel = new ConnectAPIC.Scripts.ViewModel.ExternalPortViewModel(gridManager, externalInput, lightCalculationService);
            var externalOutput = gridManager.ExternalPortManager.ExternalPorts.Last();
            var outputViewModel = new ConnectAPIC.Scripts.ViewModel.ExternalPortViewModel(gridManager, externalOutput, lightCalculationService);

            var onOutputPortArgs = new SetInputPowerArgs(outputViewModel.PortModel as ExternalInput, 0.5f);
            var overTheLimitPowerArgs = new SetInputPowerArgs(inputViewModel.PortModel as ExternalInput, 10);
            var negativePowerArgs = new SetInputPowerArgs(inputViewModel.PortModel as ExternalInput, -10);

            // act
            var canExecuteOnOutputPortArgs = command.CanExecute(onOutputPortArgs);
            var canExecuteOnOverTheLimit = command.CanExecute(overTheLimitPowerArgs);
            var canExecuteOnNegativePower = command.CanExecute(negativePowerArgs);

            // assert
            canExecuteOnOutputPortArgs.ShouldBeFalse();
            canExecuteOnOverTheLimit.ShouldBeFalse();
            canExecuteOnNegativePower.ShouldBeFalse();
        }

        [Fact]
        public async Task TestExecute()
        {
            // arrange
            var externalPort = gridManager.ExternalPortManager.ExternalPorts.First();
            var portViewModel = new ConnectAPIC.Scripts.ViewModel.ExternalPortViewModel(gridManager, externalPort, lightCalculationService);

            var setToZeroArgs = new SetInputPowerArgs(portViewModel.PortModel as ExternalInput, minPower);
            var setToHalfArgs = new SetInputPowerArgs(portViewModel.PortModel as ExternalInput, halfPower);
            var setToMaxArgs = new SetInputPowerArgs(portViewModel.PortModel as ExternalInput, maxPower);

            // act
            await command.ExecuteAsync(setToZeroArgs);
            var powerOnZero = portViewModel.Power.Length();
            await command.ExecuteAsync(setToHalfArgs);
            var powerOnHalf = portViewModel.Power.Length();
            await command.ExecuteAsync(setToMaxArgs);
            var powerOnMax = portViewModel.Power.Length();

            command.Undo();
            var powerAfterUndo = portViewModel.Power.Length();

            // assert
            powerOnZero.ShouldBeInRange(minPower - toleranceMargins, minPower + toleranceMargins);
            powerOnHalf.ShouldBeInRange(halfPower - toleranceMargins, halfPower + toleranceMargins);
            powerOnMax.ShouldBeInRange(maxPower - toleranceMargins, maxPower + toleranceMargins);
            powerAfterUndo.ShouldBeInRange(halfPower - toleranceMargins, halfPower + toleranceMargins);
        }
    }
}
