using CAP_Core.Grid;
using ConnectAPIC.Scripts.ViewModel.Commands;
using System;
using System.Threading.Tasks;

namespace ConnectAPIC.LayoutWindow.ViewModel.Commands
{
    public class RotateComponentCommand : ICommandBase
    {
        private readonly GridManager Grid;

        public event EventHandler CanExecuteChanged;

        public RotateComponentCommand(GridManager grid)
        {
            this.Grid = grid;
        }
        public bool CanExecute(object parameter)
        {
            if (parameter is not RotateComponentArgs args) return false;
            return CanRotateComponentBy90(args.GridX, args.GridY);
        }
        private bool CanRotateComponentBy90(int GridX, int GridY)
        {
            var component = Grid.ComponentMover.GetComponentAt(GridX, GridY);
            if (component == null) return false;
            int widthAfterRotation = component.HeightInTiles;
            int heightAfterRotation = component.WidthInTiles;
            for (int i = 0; i < widthAfterRotation; i++)
            {
                for (int j = 0; j < heightAfterRotation; j++)
                {
                    var componentAtTile = Grid.ComponentMover.GetComponentAt(GridX + i, GridY + j);
                    if (componentAtTile != component && componentAtTile != null)
                        return false;
                }
            }
            return true;
        }
        public Task ExecuteAsync(object parameter)
        {
            if (!CanExecute(parameter)) return Task.CompletedTask;
            var args = (RotateComponentArgs)parameter;
            Grid.ComponentRotator.RotateComponentBy90CounterClockwise(args.GridX, args.GridY);
            return Task.CompletedTask;
        }
    }
    public class RotateComponentArgs
    {
        public RotateComponentArgs(int gridX, int gridY)
        {
            GridX = gridX;
            GridY = gridY;
        }

        public int GridX { get; }
        public int GridY { get; }
    }
}
