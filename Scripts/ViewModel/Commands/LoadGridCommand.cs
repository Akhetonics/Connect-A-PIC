using CAP_Contracts;
using CAP_Core;
using CAP_Core.CodeExporter;
using CAP_Core.Components.Creation;
using CAP_Core.Grid;
using CAP_Core.Helpers;
using ConnectAPIC.Scripts.ViewModel.Commands;
using Microsoft.VisualBasic;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ConnectAPIC.LayoutWindow.ViewModel.Commands
{

    public class LoadGridCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private GridPersistenceManager gridPersistenceManager;

        public IComponentFactory ComponentFactory { get; }
        public GridViewModel ViewModel { get; }

        public LoadGridCommand(GridManager grid, IDataAccessor dataAccessor, IComponentFactory componentFactory, GridViewModel viewModel)
        { 
            this.gridPersistenceManager = new GridPersistenceManager(grid, dataAccessor);
            ComponentFactory = componentFactory;
            ViewModel = viewModel;
        }
        
        public bool CanExecute(object parameter)
        {
            if( parameter is LoadGridParameters)
            {
                return true;
            }
            return false;
        }
        
        public async Task ExecuteAsync(object parameter)
        {
            if (!CanExecute(parameter)) return;
            var loadParams = (LoadGridParameters)parameter;
            var lightStatus = ViewModel.LightManager.IsLightOn;
            ViewModel.LightManager.IsLightOn = false;
            await gridPersistenceManager.LoadAsync(loadParams.Path, ComponentFactory);
            ViewModel.LightManager.IsLightOn = lightStatus;
        }
    }

    public class LoadGridParameters
    {
        public LoadGridParameters( string path)
        {
            Path = path;
        }

        public string Path { get; }
    }
}
