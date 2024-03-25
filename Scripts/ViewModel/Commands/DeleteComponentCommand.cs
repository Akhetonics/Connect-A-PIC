using CAP_Core.Grid;
using CAP_Core.Helpers;
using ConnectAPIC.Scripts.ViewModel.Commands;
using System;
using System.Collections.Generic;
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
        public bool CanExecute(object parameter) => parameter is DeleteComponentArgs;

        public Task ExecuteAsync(object parameter)
        {
            if (!CanExecute(parameter)) return Task.CompletedTask;
            var deleteParameters = (DeleteComponentArgs)parameter;
            foreach (IntVector deletePosition in deleteParameters.DeletePositions)
            {
                grid.UnregisterComponentAt(deletePosition.X, deletePosition.Y);
            }
            return Task.CompletedTask;
        }
    }
    public class DeleteComponentArgs
    {
        public DeleteComponentArgs(List<IntVector> deletePositions )
        {
            DeletePositions = deletePositions;
        }

        public List<IntVector> DeletePositions { get; }
    }
}
