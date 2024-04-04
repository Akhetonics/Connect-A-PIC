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
    public class InputOutputChangeCommand : ICommand
    {
        public GridManager Grid { get; }
        public LightCalculationService LightCalculator { get; }

        public InputOutputChangeCommand(GridManager grid, LightCalculationService lightCalculator)
        {
            LightCalculator = lightCalculator;
            Grid = grid;
        }

        public bool CanExecute(object parameter)
        {
            ExternalPortViewModel ViewModel = parameter as ExternalPortViewModel;
            if (ViewModel == null) return false;

            return Grid.ExternalPortManager.ExternalPorts.Contains(ViewModel.PortModel);
        }

        public async Task ExecuteAsync(object parameter)
        {
            if (!CanExecute(parameter)) return;
            ExternalPortViewModel ViewModel = parameter as ExternalPortViewModel;

            ExternalPort newPort;
            ExternalPort port = ViewModel.PortModel;
            int index = Grid.ExternalPortManager.ExternalPorts.IndexOf(port);

            if (port is ExternalInput)
                newPort = new ExternalOutput(port.PinName, port.TilePositionY);
            else
                newPort = new ExternalInput(port.PinName, LaserType.Red, port.TilePositionY, 1);

            ViewModel.PortModel = newPort;
            Grid.ExternalPortManager.ExternalPorts[index] = newPort;

            //LightCalculator.ShowLightPropagationAsync();
            Grid.LightManager.IsLightOn = !Grid.LightManager.IsLightOn;
            Grid.LightManager.IsLightOn = !Grid.LightManager.IsLightOn;
        }
    }
}
