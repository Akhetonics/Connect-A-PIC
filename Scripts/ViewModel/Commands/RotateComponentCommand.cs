using CAP_Core.Grid;
using CAP_Core.Helpers;
using ConnectAPIC.Scripts.ViewModel.Commands;
using System;
using System.Threading.Tasks;

namespace ConnectAPIC.LayoutWindow.ViewModel.Commands
{
    public class RotateComponentCommand : CommandBase<RotateComponentArgs>
    {
        private readonly GridManager Grid;
        public IntVector OldComponentPosition { get; private set; }

        public event EventHandler CanExecuteChanged;

        public RotateComponentCommand(GridManager grid)
        {
            this.Grid = grid;
        }
        public override bool CanExecute(object parameter)
        {
            if (parameter is not RotateComponentArgs args) return false;
            return CanRotateComponentBy90(args.GridX, args.GridY);
        }
        private bool CanRotateComponentBy90(int GridX, int GridY)
        {
            var component = Grid.ComponentMover.GetComponentAt(GridX, GridY);
            GridX = component.GridXMainTile;
            GridY = component.GridYMainTile;
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
        internal override Task ExecuteAsyncCmd(RotateComponentArgs args)
        {
            var oldComponent = Grid.ComponentMover.GetComponentAt(args.GridX, args.GridY);
            OldComponentPosition = new IntVector(oldComponent.GridXMainTile, oldComponent.GridYMainTile);
            Grid.ComponentRotator.RotateComponentBy90CounterClockwise(args.GridX, args.GridY);
            return Task.CompletedTask;
        }
        public override void Undo()
        {
            // rotating something back means to rotate it 270° = 3 times by 90°
            Grid.ComponentRotator.RotateComponentBy90CounterClockwise(OldComponentPosition.X, OldComponentPosition.Y);
            Grid.ComponentRotator.RotateComponentBy90CounterClockwise(OldComponentPosition.X, OldComponentPosition.Y);
            Grid.ComponentRotator.RotateComponentBy90CounterClockwise(OldComponentPosition.X, OldComponentPosition.Y);
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
