using CAP_Core;
using System;
using System.Windows.Input;

namespace ConnectAPIC.LayoutWindow.ViewModel.Commands
{
    public class MoveComponentCommand : ICommand
    {
        private readonly Grid grid;

        public event EventHandler CanExecuteChanged;

        public MoveComponentCommand(Grid grid)
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
        public void Execute(object parameter)
        {
            if (CanExecute(parameter) == false) return;
            var args = (MoveComponentArgs)parameter;
            grid.MoveComponent(args.TargetX, args.TargetY, args.SourceX, args.SourceY);
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
