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
    public class InputColorChangeCommand : ICommand
    {
        public GridManager Grid { get; }

        public InputColorChangeCommand(GridManager grid)
        {
            Grid = grid;
        }

        public bool CanExecute(object parameter)
        {
            InputColorChangeArgs inputParams = parameter as InputColorChangeArgs;
            if (inputParams == null || inputParams.LaserColor == null) return false;
            
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

            InputColorChangeArgs args = parameter as InputColorChangeArgs;
            ExternalInput input = null;

            foreach (var item in Grid.ExternalPorts)
                if (item.TilePositionY == args.PortPositionY) { input = (ExternalInput)item; break; }

            if (input == null) return;

            //TODO: this is kinda hacky way of doing it, needs to be discussed
            int i = Grid.ExternalPorts.IndexOf(input);
            input.LaserType = args.LaserColor;
            Grid.ExternalPorts[i] = input;

            //TODO: light calculator to recalculate light
        }
    }

    public class InputColorChangeArgs
    {
        public InputColorChangeArgs(int portPositionY, String colorString)
        {
            PortPositionY = portPositionY;
            switch (colorString.ToLower()) {
                case "red": LaserColor = LaserType.Red; break;
                case "green": LaserColor = LaserType.Green; break;
                case "blue": LaserColor = LaserType.Blue; break;
            }
        }

        public InputColorChangeArgs(int portPositionY, LaserType laserColor)
        {
            PortPositionY = portPositionY;
            LaserColor = laserColor;
        }

        public int PortPositionY { get; }
        public LaserType LaserColor { get; }
    }
}
