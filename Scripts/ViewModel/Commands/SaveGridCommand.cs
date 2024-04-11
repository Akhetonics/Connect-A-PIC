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

    public class SaveGridCommand : CommandBase<SaveGridParameters>
    {
        public event EventHandler CanExecuteChanged;
        private GridPersistenceManager gridPersistenceManager;

        public SaveGridCommand( GridManager grid, IDataAccessor dataAccessor)
        { 
            this.gridPersistenceManager = new GridPersistenceManager(grid, dataAccessor);
        }

        internal async override Task ExecuteAsyncCmd(SaveGridParameters parameter)
        {
            var saveParams = (SaveGridParameters)parameter;
            await gridPersistenceManager.SaveAsync(saveParams.Path);
        }
    }

    public class SaveGridParameters
    {
        public SaveGridParameters( string path)
        {
            Path = path;
        }

        public string Path { get; }
    }
}
