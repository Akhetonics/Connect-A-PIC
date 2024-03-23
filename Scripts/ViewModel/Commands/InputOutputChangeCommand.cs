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
    public class InputOutputChangeCommand : ICommand
    {
        public GridManager Grid { get; }

        public InputOutputChangeCommand(GridManager grid)
        {
            Grid = grid;
        }

        public bool CanExecute(object parameter)
        {
            if (parameter is not int) return false;
            
            foreach (var item in Grid.ExternalPorts)
                if (item.TilePositionY == (int)parameter)
                    return true;
            return false;
        }

        public async Task ExecuteAsync(object parameter)
        {
            if (!CanExecute(parameter)) return;

            int portPositionY = (int)parameter;
            ExternalPort input = null;

            foreach (var item in Grid.ExternalPorts)
                if (item.TilePositionY == portPositionY) { input = item; break; }

            if (input == null) return;
            int indexOfPort = Grid.ExternalPorts.IndexOf(input);

            if (input is ExternalInput)
            {
                Grid.ExternalPorts[indexOfPort] =
                    new ExternalOutput(input.PinName, input.TilePositionY);
            }
            else
            {
                Grid.ExternalPorts[indexOfPort] =
                    new ExternalInput(input.PinName, LaserType.Red, input.TilePositionY, 1);
            }
        }
    }
}
