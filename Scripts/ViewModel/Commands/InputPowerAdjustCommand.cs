using CAP_Core.ExternalPorts;
using CAP_Core.Grid;
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.Scripts.ViewModel.Commands
{
    public class InputPowerAdjustCommand : ICommand
    {
        public GridManager Grid { get; }

        public InputPowerAdjustCommand(GridManager grid)
        {
            Grid = grid;
        }

        public bool CanExecute(object parameter)
        {
            InputPowerAdjustArgs inputParams = parameter as InputPowerAdjustArgs;
            if (inputParams == null || inputParams.PowerValue < 0 || inputParams.PowerValue > 1) return false;

            return Grid.ExternalPortManager.ExternalPorts.Contains(inputParams.Port) && inputParams.Port is ExternalInput;

        }
        public async Task ExecuteAsync(object parameter)
        {
            if (!CanExecute(parameter)) return;

            InputPowerAdjustArgs args = (InputPowerAdjustArgs)parameter;
            ExternalInput input = args.Port as ExternalInput;

            input.InFlowPower = args.PowerValue;

            Grid.LightManager.IsLightOn = !Grid.LightManager.IsLightOn;
            Grid.LightManager.IsLightOn = !Grid.LightManager.IsLightOn;
        }
        
    }

    public class InputPowerAdjustArgs
    {
        public InputPowerAdjustArgs(ExternalPort port, double powerValue)
        {
            Port = port;
            PowerValue = powerValue;
        }

        public ExternalPort Port { get; }
        public double PowerValue { get; }
    }
}
