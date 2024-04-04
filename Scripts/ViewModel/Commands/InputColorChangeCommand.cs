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

namespace ConnectAPIC.Scripts.ViewModel.Commands
{
    public class InputColorChangeCommand : ICommand
    {
        public GridManager Grid { get; }
        public LightCalculationService LightCalculator { get; }

        public InputColorChangeCommand(GridManager grid, LightCalculationService lightCalculator) {
            LightCalculator = lightCalculator;
            Grid = grid;
        }

        public bool CanExecute(object parameter)
        {
            InputColorChangeArgs inputParams = parameter as InputColorChangeArgs;
            if (inputParams == null || inputParams.LaserColor == null) return false;

            return Grid.ExternalPortManager.ExternalPorts.Contains(inputParams.Port);
        }

        public async Task ExecuteAsync(object parameter)
        {
            if (!CanExecute(parameter)) return;

            InputColorChangeArgs args = parameter as InputColorChangeArgs;

            args.Port.LaserType = args.LaserColor;

            //LightCalculator.ShowLightPropagationAsync();
            Grid.LightManager.IsLightOn = !Grid.LightManager.IsLightOn;
            Grid.LightManager.IsLightOn = !Grid.LightManager.IsLightOn;
        }
    }

    public class InputColorChangeArgs
    {
        public InputColorChangeArgs(ExternalInput port, String colorString)
        {
            Port = port;
            switch (colorString.ToLower()) {
                case "red": LaserColor = LaserType.Red; break;
                case "green": LaserColor = LaserType.Green; break;
                case "blue": LaserColor = LaserType.Blue; break;
            }
        }

        public InputColorChangeArgs(ExternalInput port, LaserType laserColor)
        {
            Port = port;
            LaserColor = laserColor;
        }

        public ExternalInput Port { get; }
        public LaserType LaserColor { get; }
    }
}
