using CAP_Core.Grid;
using ConnectAPIC.Scripts.ViewModel.Commands;
using System;
using System.Threading.Tasks;

namespace ConnectAPIC.LayoutWindow.ViewModel.Commands
{
    public class MoveComponentCommand : ICommand
    {
        private readonly GridManager grid;

        public event EventHandler CanExecuteChanged;

        public MoveComponentCommand(GridManager grid)
        {
            this.grid = grid;
        }
        public bool CanExecute(object parameter)
        {
            if (parameter is not MoveComponentArgs args) return false;
            return CanMoveComponent(args.SourceX, args.SourceY, args.TargetX, args.TargetY);
        }
        private bool CanMoveComponent(int sourceX, int sourceY, int targetX, int targetY)
        {
            var comp = grid.GetComponentAt(sourceX, sourceY);
            if (comp == null) return false;
            for (int i = 0; i < comp.WidthInTiles; i++)
            {
                for (int j = 0; j < comp.HeightInTiles; j++)
                {
                    var foundComp = grid.GetComponentAt(targetX + i, targetY + j);
                    if (foundComp != comp && foundComp != null)
                        return false;
                }
            }
            return true;
        }
        public Task ExecuteAsync(object parameter)
        {
            if (CanExecute(parameter) == false) return default;
            var args = (MoveComponentArgs)parameter;
            grid.MoveComponent(args.TargetX, args.TargetY, args.SourceX, args.SourceY);
            return Task.CompletedTask;
        }
    }
    public class MoveComponentArgs
    {
        public MoveComponentArgs( int sourceX, int sourceY, int targetX, int targetY)
        {
            TargetX = targetX;
            TargetY = targetY;
            SourceX = sourceX;
            SourceY = sourceY;
        }

        public int TargetX { get; }
        public int TargetY { get; }
        public int SourceX { get; }
        public int SourceY { get; }
    }
}
