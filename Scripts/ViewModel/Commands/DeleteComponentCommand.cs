using CAP_Core.Components;
using CAP_Core.Grid;
using CAP_Core.Helpers;
using ConnectAPIC.Scripts.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConnectAPIC.LayoutWindow.ViewModel.Commands
{
    public class DeleteComponentCommand : CommandBase<DeleteComponentArgs>
    {
        private readonly GridManager Grid;

        public List<(Component Component, IntVector Position)> DeletedComponents { get; private set; } = new();

        public DeleteComponentCommand(GridManager grid)
        {
            this.Grid = grid;
        }

        internal override Task ExecuteAsyncCmd(DeleteComponentArgs parameter)
        {
            foreach (IntVector deletePosition in parameter.DeletePositions)
            {
                var componentToDelete = Grid.ComponentMover.GetComponentAt(deletePosition.X, deletePosition.Y);
                if(componentToDelete == null) continue; // one mit accidentally have clicked on an empty field
                DeletedComponents.Add((componentToDelete,new IntVector(componentToDelete.GridXMainTile,componentToDelete.GridYMainTile)));
                Grid.ComponentMover.UnregisterComponentAt(deletePosition.X, deletePosition.Y);
            }
            return Task.CompletedTask;
        }

        public override void Undo()
        {
            foreach ( (var Component,var Position) in DeletedComponents)
            {
                Grid.ComponentMover.PlaceComponent(Position.X , Position.Y, Component);
            }
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
