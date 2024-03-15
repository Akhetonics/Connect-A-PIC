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

    public class SwitchOnLightCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public GridManager Grid { get; }

        public SwitchOnLightCommand( GridManager grid)
        {
            Grid = grid;
        }
        
        public bool CanExecute(object parameter)
        {
            return parameter is bool;
        }
        
        public async Task ExecuteAsync(object parameter)
        {
            if (!CanExecute(parameter)) return;
            Grid.IsLightOn = (bool)parameter;
        }
    }

}
