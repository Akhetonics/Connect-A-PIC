using CAP_Core.ExternalPorts;
using CAP_Core.Grid;
using CAP_Core.Helpers;
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
    public class SetPortTypeCommand : CommandBase<SetPortTypeArgs>
    {
        public GridManager Grid { get; }
        public ILightCalculationService LightCalculator { get; }
        public ExternalPort OldPort { get; private set; }
        public int OldPortIndex { get; private set; }

        public SetPortTypeCommand(GridManager grid, ILightCalculationService lightCalculator)
        {
            LightCalculator = lightCalculator;
            Grid = grid;
        }

        public override bool CanExecute(object parameter)
        {
            if (parameter is not SetPortTypeArgs args) return false;
            return Grid.ExternalPortManager.ExternalPorts.Contains(args.PortViewModel.PortModel);
        }

        internal override Task ExecuteAsyncCmd(SetPortTypeArgs args)
        {
            OldPort = args.PortViewModel.PortModel;
            OldPortIndex = Grid.ExternalPortManager.ExternalPorts.IndexOf(OldPort);

            ExternalPort newPort;
            if (args.IsSetToOutput)
                newPort = new ExternalOutput(OldPort.PinName, OldPort.TilePositionY, OldPort.IsLeftPort);
            else
                newPort = new ExternalInput(OldPort.PinName, LaserType.Red, OldPort.TilePositionY, 1, OldPort.IsLeftPort);

            args.PortViewModel.PortModel = newPort;
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
            ExecutionParams.PortViewModel.PortModel = OldPort;
            Grid.ExternalPortManager.ExternalPorts[OldPortIndex] = OldPort;
            RestartLight();
        }
    }

    public class SetPortTypeArgs
    {
        public SetPortTypeArgs(ExternalPortViewModel portViewModel, bool isSetToOutput)
        {
            PortViewModel = portViewModel;
            IsSetToOutput = isSetToOutput;
        }
        public ExternalPortViewModel PortViewModel { get; }
        public bool IsSetToOutput { get; }
    }
}
