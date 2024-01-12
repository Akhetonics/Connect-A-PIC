using CAP_Core.Grid;
using ConnectAPIC.Scripts.ViewModel.Commands;
using System;
using System.Threading.Tasks;

namespace ConnectAPIC.LayoutWindow.ViewModel.Commands
{
    public class DeleteComponentCommand :ICommand
    {
        private readonly GridManager grid;

        public event EventHandler CanExecuteChanged;
        
        public DeleteComponentCommand(GridManager grid)
        {
            this.grid = grid;
        }
        public bool CanExecute(object parameter)
        {
            if (parameter is DeleteComponentArgs deleteParameters
                && grid.GetComponentAt(deleteParameters.gridX, deleteParameters.gridY) != null)
                return true;
            return false;
        }

        public Task ExecuteAsync(object parameter)
        {
            if (!CanExecute(parameter)) return default;
            var deleteParameters = (DeleteComponentArgs)parameter;
            grid.UnregisterComponentAt(deleteParameters.gridX, deleteParameters.gridY);
            return Task.CompletedTask;
        }
    }
    public class DeleteComponentArgs
    {
        public readonly int gridX;
        public readonly int gridY;

        public DeleteComponentArgs(int gridX, int gridY)
        {
            this.gridX = gridX;
            this.gridY = gridY;
        }
    }
}
