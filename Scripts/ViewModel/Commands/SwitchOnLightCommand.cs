using CAP_Contracts;
using CAP_Core;
using CAP_Core.CodeExporter;
using CAP_Core.Grid;
using CAP_Core.Helpers;
using ConnectAPIC.Scripts.ViewModel.Commands;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ConnectAPIC.LayoutWindow.ViewModel.Commands
{

    public class SwitchOnLightCommand : CommandBase<bool>
    {
        public LightManager LightManager { get; }
        public bool OldLightState { get; private set; }

        public event EventHandler CanExecuteChanged;

        public SwitchOnLightCommand( LightManager lightManager)
        {
            LightManager = lightManager;
        }

        internal override Task ExecuteAsyncCmd(bool parameter)
        {
            OldLightState = LightManager.IsLightOn;
            LightManager.IsLightOn = (bool)parameter;
            return Task.CompletedTask;
        }
        public override void Undo()
        {
            LightManager.IsLightOn = OldLightState;
        }
    }

}
