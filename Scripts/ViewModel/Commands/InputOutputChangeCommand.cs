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
    public class InputOutputChangeCommand : CommandBase<ExternalPortViewModel>
    {
        public GridManager Grid { get; }
        public LightCalculationService LightCalculator { get; }
        public ExternalPort OldPort { get; private set; }
        public int OldPortIndex { get; private set; }

        public InputOutputChangeCommand(GridManager grid, LightCalculationService lightCalculator)
        {
            LightCalculator = lightCalculator;
            Grid = grid;
        }

        public override bool CanExecute(object parameter)
        {
            if (parameter is not ExternalPortViewModel ViewModel) return false;
            return Grid.ExternalPortManager.ExternalPorts.Contains(ViewModel.PortModel);
        }

        internal override Task ExecuteAsyncCmd(ExternalPortViewModel ViewModel)
        {
            OldPort = ViewModel.PortModel;
            OldPortIndex = Grid.ExternalPortManager.ExternalPorts.IndexOf(OldPort);

            ExternalPort newPort;
            if (OldPort is ExternalInput)
                newPort = new ExternalOutput(OldPort.PinName, OldPort.TilePositionY);
            else
                newPort = new ExternalInput(OldPort.PinName, LaserType.Red, OldPort.TilePositionY, 1);

            ViewModel.PortModel = newPort;
            Grid.ExternalPortManager.ExternalPorts[OldPortIndex] = newPort;
            RestartLight();
            return Task.CompletedTask;
        }

        private void RestartLight()
        {
            Grid.LightManager.IsLightOn = !Grid.LightManager.IsLightOn;
            Grid.LightManager.IsLightOn = !Grid.LightManager.IsLightOn;
        }

        public override void Undo()
        {
            ExecutionParams.PortModel = OldPort;
            Grid.ExternalPortManager.ExternalPorts[OldPortIndex] = OldPort;
            RestartLight();
        }
    }
}
