using Antlr4.Runtime.Misc;
using CAP_Core.ExternalPorts;
using CAP_Core.Grid;
using CAP_Core.LightCalculation;
using ConnectAPIC.LayoutWindow.ViewModel;
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
    public class SetInputColorCommand : CommandBase<SetInputColorArgs>
    {
        public GridManager Grid { get; }
        public LaserType OldLaserType { get; private set; }

        public SetInputColorCommand(GridManager grid)
        {
            Grid = grid;
        }

        public override bool CanExecute(object parameter)
        {
            if (parameter is not SetInputColorArgs inputParams || inputParams.LaserType == null) return false;
            if (inputParams.PortIndex < 0 || inputParams.PortIndex >= Grid.ExternalPortManager.ExternalPorts.Count) return false;
            if (Grid.ExternalPortManager.ExternalPorts[inputParams.PortIndex] is ExternalOutput) return false;

            return true;
        }

        internal override Task ExecuteAsyncCmd(SetInputColorArgs args)
        {
            if (Grid.ExternalPortManager.ExternalPorts[args.PortIndex] is not ExternalInput port ) return Task.CompletedTask;
            OldLaserType = port.LaserType;
            port.LaserType = args.LaserType;
            RestartLight();
            return Task.CompletedTask;
        }
        public override void Undo()
        {
            if (Grid.ExternalPortManager.ExternalPorts[this.ExecutionParams.PortIndex] is not ExternalInput port) return;
            // the port could have been replaced to be an input instead of an output
            port.LaserType = OldLaserType;
            RestartLight();
        }
        public override void Redo()
        {
            base.Redo();
            RestartLight();
        }

        private void RestartLight()
        {
            if (!Grid.LightManager.IsLightOn) return;
            Grid.LightManager.IsLightOn = !Grid.LightManager.IsLightOn;
            Grid.LightManager.IsLightOn = !Grid.LightManager.IsLightOn;
        }
    }

    public class SetInputColorArgs
    {
        public SetInputColorArgs(int portIndex , LaserType laserType)
        {
            LaserType = laserType;
            PortIndex = portIndex;
        }

        public int PortIndex { get; }
        public LaserType LaserType { get; }
    }
}
