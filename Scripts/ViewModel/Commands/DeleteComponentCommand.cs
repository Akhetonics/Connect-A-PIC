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

        public List<(Component Component, IntVector Position)> DeletedComponents { get; private set; }
        public List<IntVector> SelectedComponentPositions { get; private set; }
        public ISelectionManager SelectionManager { get; }

        public DeleteComponentCommand(GridManager grid, ISelectionManager selectionManager)
        {
            this.Grid = grid;
            SelectionManager = selectionManager;
        }

        internal override Task ExecuteAsyncCmd(DeleteComponentArgs parameter)
        {
            DeletedComponents = new();
            SelectedComponentPositions = new();
            foreach (IntVector deletePosition in parameter.DeletePositions)
            {
                var componentToDelete = Grid.ComponentMover.GetComponentAt(deletePosition.X, deletePosition.Y);
                if(componentToDelete == null) continue; // one might accidentally have clicked on an empty field
                var componentPosition = new IntVector(componentToDelete.GridXMainTile, componentToDelete.GridYMainTile);
                DeletedComponents.Add((componentToDelete, componentPosition));
                if (SelectionManager.Selections.Contains(componentPosition)){
                    SelectedComponentPositions.Add(componentPosition);
                }
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
            // restore selection
            foreach(var componentPos in SelectedComponentPositions)
            {
                SelectionManager.Selections.Add(componentPos);
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
