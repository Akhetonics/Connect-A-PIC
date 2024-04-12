using CAP_Core.ExternalPorts;
using CAP_Core.Grid;
using CAP_Core.LightCalculation;
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.Scripts.ViewModel.Commands.ExternalPorts
{
    public class SetInputPowerCommand : CommandBase<SetInputPowerArgs>
    {
        public GridManager Grid { get; }
        public LightCalculationService LightCalculator { get; }
        public Complex OldInflowPower { get; private set; }

        public SetInputPowerCommand(GridManager grid, LightCalculationService lightCalculator)
        {
            LightCalculator = lightCalculator;
            Grid = grid;
        }

        public override bool CanExecute(object parameter)
        {
            if (parameter is not SetInputPowerArgs inputParams || inputParams.PowerValue < 0 || inputParams.PowerValue > 1) return false;
            return Grid.ExternalPortManager.ExternalPorts.Contains(inputParams.Port) && inputParams.Port is ExternalInput;
        }
        internal async override Task ExecuteAsyncCmd(SetInputPowerArgs args)
        {
            ExternalInput input = args.Port as ExternalInput;
            OldInflowPower = input.InFlowPower;
            input.InFlowPower = args.PowerValue;
            await LightCalculator.ShowLightPropagationAsync();
            return;
        }
        public override void Undo()
        {
            ((ExternalInput)ExecutionParams.Port).InFlowPower = OldInflowPower;
            LightCalculator.ShowLightPropagationAsync().Wait();
        }

    }

    public class SetInputPowerArgs
    {
        public SetInputPowerArgs(ExternalPort port, double powerValue)
        {
            Port = port;
            PowerValue = powerValue;
        }

        public ExternalPort Port { get; }
        public double PowerValue { get; }
    }
}
