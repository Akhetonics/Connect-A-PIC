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
            
            bool found = false;
            foreach (var item in Grid.ExternalPorts)
                if (item.TilePositionY == inputParams.PortPositionY)
                {
                    found = (item is ExternalInput);
                    break;
                }

            return found;
        }
        public async Task ExecuteAsync(object parameter)
        {
            if (!CanExecute(parameter)) return;

            InputPowerAdjustArgs args = (InputPowerAdjustArgs)parameter;
            ExternalInput input = null;

            foreach (var item in Grid.ExternalPorts)
                if (item.TilePositionY == args.PortPositionY) { input = (ExternalInput)item; break; }

            if (input == null) return;

            //TODO: this is kinda hacky way of doing it, needs to be discussed
            int i = Grid.ExternalPorts.IndexOf(input);
            input.InFlowPower = args.PowerValue;
            Grid.ExternalPorts[i] = input;

            //TODO: grid needs light calculation to be recaluclated
        }
        
    }

    public class InputPowerAdjustArgs
    {
        public InputPowerAdjustArgs(int portPositionY, double powerValue)
        {
            PortPositionY = portPositionY;
            PowerValue = powerValue;
        }

        public int PortPositionY { get; }
        public double PowerValue { get; }
    }
}
