using ConnectAPIC.Scenes.Component;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ConnectAPIC.LayoutWindow.ViewModel.Commands
{
    public class DeleteComponentCommand :ICommand
    {
        private readonly Grid grid;

        public event EventHandler CanExecuteChanged;
        
        public DeleteComponentCommand(Grid grid)
        {
            this.grid = grid;
        }
        public bool CanExecute(object parameter)
        {
            if (parameter is DeleteComponentArgs deleteparams
                && grid.GetComponentAt(deleteparams.gridX, deleteparams.gridY) != null)
                return true;
            return false;
        }

        public void Execute(object parameter)
        {
            if (!CanExecute(parameter)) return;
            var deleteparams = (DeleteComponentArgs)parameter;
            grid.UnregisterComponentAt(deleteparams.gridX, deleteparams.gridY);
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
