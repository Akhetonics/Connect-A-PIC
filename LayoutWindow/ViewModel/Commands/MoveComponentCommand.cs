using ConnectAPIC.Scenes.Component;
using Godot;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
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
            return grid.CanMoveComponent(args.SourceX, args.SourceY, args.TargetX, args.TargetY);
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
