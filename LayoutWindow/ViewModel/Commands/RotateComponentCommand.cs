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
    public class RotateComponentCommand : ICommand
    {
        private readonly Grid grid;
        private readonly int gridX;
        private readonly int gridY;

        public event EventHandler CanExecuteChanged;
        
        public RotateComponentCommand(Grid grid, int gridX , int gridY)
        {
            this.grid = grid;
            this.gridX = gridX;
            this.gridY = gridY;
        }
        public bool CanExecute(object parameter)
        {
            if( grid.GetComponentAt(gridX,gridY) != null)
                return true;
            return false;
        }

        public void Execute(object parameter)
        {
            grid.RotateComponentBy90(gridX,gridY);
        }
    }
}
