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
    public class RotateComponentCommand : ICommand
    {
        private readonly Grid grid;

        public event EventHandler CanExecuteChanged;

        public RotateComponentCommand(Grid grid)
        {
            this.grid = grid;
        }
        public bool CanExecute(object parameter)
        {
            if (parameter is not RotateComponentArgs args) return false;
            return CanRotateComponentBy90(args.Gridx, args.Gridy);
        }
        private bool CanRotateComponentBy90(int Gridx, int Gridy)
        {
            var component = grid.GetComponentAt(Gridx, Gridy);
            if (component == null) return false;
            int widthAfterRotation = component.HeightInTiles;
            int heightAfterRotation = component.WidthInTiles;
            for (int i = 0; i < widthAfterRotation; i++)
            {
                for (int j = 0; j < heightAfterRotation; j++)
                {
                    var componentAtTile = grid.GetComponentAt(Gridx + i, Gridy + j);
                    if (componentAtTile != component && componentAtTile != null)
                        return false;
                }
            }
            return true;
        }
        public void Execute(object parameter)
        {
            if (!CanExecute(parameter)) return;
            var args = (RotateComponentArgs)parameter;
            grid.RotateComponentBy90CounterClockwise(args.Gridx, args.Gridy);
        }
    }
    public class RotateComponentArgs
    {
        public RotateComponentArgs(int gridx, int gridy)
        {
            Gridx = gridx;
            Gridy = gridy;
        }

        public int Gridx { get; }
        public int Gridy { get; }
    }
}
