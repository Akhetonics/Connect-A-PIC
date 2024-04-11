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

namespace ConnectAPIC.Scripts.ViewModel.Commands
{
    public class InputColorChangeCommand : CommandBase<InputColorChangeArgs>
    {
        public GridManager Grid { get; }
        public GridViewModel GridViewModel { get; }
        public LaserType OldLaserType { get; private set; }

        public InputColorChangeCommand(GridManager grid, GridViewModel gridViewModel) {
            Grid = grid;
            GridViewModel = gridViewModel;
        }

        public override bool CanExecute(object parameter)
        {
            if (parameter is not InputColorChangeArgs inputParams || inputParams.LaserType == null) return false;
            return Grid.ExternalPortManager.ExternalPorts.Contains(inputParams.Port);
        }

        internal override Task ExecuteAsyncCmd(InputColorChangeArgs args)
        {
            OldLaserType = args.Port.LaserType;
            args.Port.LaserType = args.LaserType;
            RestartLight();
            return Task.CompletedTask;
        }
        public override void Undo()
        {
            // the port could have been replaced to be an input instead of an output
            ExecutionParams.Port.LaserType = OldLaserType;
            RestartLight();
        }
        public override void Redo()
        {
            base.Redo();
            RestartLight();
        }

        private void RestartLight()
        {
            Grid.LightManager.IsLightOn = !Grid.LightManager.IsLightOn;
            Grid.LightManager.IsLightOn = !Grid.LightManager.IsLightOn;
        }
    }

    public class InputColorChangeArgs
    {
        public InputColorChangeArgs(ExternalInput port, LaserType laserType)
        {
            Port = port;
            LaserType = laserType;
        }

        public ExternalInput Port { get; }
        public LaserType LaserType { get; }
    }
}
