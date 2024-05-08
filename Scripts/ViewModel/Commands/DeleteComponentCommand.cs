using CAP_Core.Components;
using CAP_Core.Grid;
using CAP_Core.Helpers;
using ConnectAPIC.Scripts.View.ToolBox;
using ConnectAPIC.Scripts.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConnectAPIC.LayoutWindow.ViewModel.Commands
{
    public class DeleteComponentCommand : CommandBase<DeleteComponentArgs>
    {
        private readonly GridManager Grid;

        public HashSet<(Component Component, IntVector Position)> DeletedComponentsAndPos { get; private set; }
        public List<IntVector> SelectedComponentPositions { get; private set; }
        public ISelectionManager SelectionManager { get; }

        public DeleteComponentCommand(GridManager grid, ISelectionManager selectionManager)
        {
            this.Grid = grid;
            SelectionManager = selectionManager;
        }

        internal override Task ExecuteAsyncCmd(DeleteComponentArgs parameter)
        {
            DeletedComponentsAndPos = new();
            SelectedComponentPositions = new();
            foreach (IntVector deletePosition in parameter.DeletePositions)
            {
                var componentToDelete = Grid.ComponentMover.GetComponentAt(deletePosition.X, deletePosition.Y);
                if(componentToDelete == null) continue; // one might accidentally have clicked on an empty field
                var componentPosition = new IntVector(componentToDelete.GridXMainTile, componentToDelete.GridYMainTile);
                DeletedComponentsAndPos.Add((componentToDelete, componentPosition));
                if (SelectionManager.Selections.Contains(componentPosition)){
                    SelectedComponentPositions.Add(componentPosition);
                }
                Grid.ComponentMover.UnregisterComponentAt(deletePosition.X, deletePosition.Y);
            }
            return Task.CompletedTask;
        }

        public override void Undo()
        {
            foreach ((var Component,var Position) in DeletedComponentsAndPos)
            {
                Grid.ComponentMover.PlaceComponent(Position.X , Position.Y, Component);
            }
            // restore selection
            foreach(var componentPos in SelectedComponentPositions)
            {
                SelectionManager.Selections.Add(componentPos);
            }
        }
        // can merge, when the appendBehavior is equal to the current one.
        public override bool CanMergeWith(ICommand newCommand)
        {
            if (newCommand is DeleteComponentCommand deleteComponentCommand && deleteComponentCommand.ExecutionParams.StrokeID == this.ExecutionParams.StrokeID)
                return true;
            return false;
        }

        public override void MergeWith(ICommand other)
        {
            if (!CanMergeWith(other))
                throw new InvalidOperationException("Cannot merge with the provided command.");
            var otherCmd = (DeleteComponentCommand)other;
            foreach ( var deletePos in otherCmd.ExecutionParams.DeletePositions)
            {
                if (ExecutionParams.DeletePositions.Contains(deletePos) == false)
                {
                    ExecutionParams.DeletePositions.Add(deletePos);
                }
            }
            // also store previously deleted components for undo
            foreach (var deleteComponentAndPos in otherCmd.DeletedComponentsAndPos)
            {
                if (DeletedComponentsAndPos.Contains(deleteComponentAndPos) == false)
                {
                    DeletedComponentsAndPos.Add(deleteComponentAndPos);
                }
            }
        }
    }
    public class DeleteComponentArgs
    {
        public DeleteComponentArgs(HashSet<IntVector> deletePositions , Guid strokeID)
        {
            DeletePositions = deletePositions;
            StrokeID = strokeID;
        }

        public HashSet<IntVector> DeletePositions { get; }
        public Guid StrokeID { get; }
    }
}
