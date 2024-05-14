using CAP_Core.Components;
using CAP_Core.Grid;
using CAP_Core.Helpers;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using ConnectAPIC.Scripts.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Godot.WebSocketPeer;
using static System.Formats.Asn1.AsnWriter;

namespace ConnectAPIC.Scripts.View.ToolBox
{
    public class BoxSelectComponentsCommand : CommandBase<BoxSelectComponentsArgs>
    {
        public BoxSelectComponentsCommand(GridManager grid, ISelectionManager selectionManager)
        {
            Grid = grid;
            SelectionManager = selectionManager;
        }

        public GridManager Grid { get; }
        public ISelectionManager SelectionManager { get; }
        public AppendBehaviors AppendBehavior { get; set; }
        public bool WasExecuted { get; set; }
        public HashSet<IntVector> OldSelection { get; private set; } = new();
        public HashSet<IntVector> NewSelection { get; private set; } = new();

        public override bool CanExecute (object parameter)
        {
            if (parameter is BoxSelectComponentsArgs boxParam)
            {
                NewSelection = CollectAllComponentsInBoxes(boxParam.BoxSelections);
                if(SelectionManager.Selections.Count == 0 && NewSelection.Count == 0)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        internal override Task ExecuteAsyncCmd(BoxSelectComponentsArgs parameter)
        {
            // select all components in box
            AppendBehavior = ExecutionParams.AppendBehavior;
            OldSelection = new HashSet<IntVector>(SelectionManager.Selections);
            NewSelection = CollectAllComponentsInBoxes(ExecutionParams.BoxSelections);
            
            if (AppendBehavior == AppendBehaviors.CreateNew)
            {
                CreateNewAndFillGridSelections();
            }
            else if (AppendBehavior == AppendBehaviors.Append)
            {
                AddNewItemsButExceptions(OldSelection);
            }
            else if (AppendBehavior == AppendBehaviors.Remove) // remove the NewSelection From OldSelection
            {
                RemoveFromGridSelectionGroup(NewSelection);
            }
            WasExecuted = true;
            return Task.CompletedTask;
        }

        private HashSet<IntVector> CollectAllComponentsInBoxes(List<(IntVector Start, IntVector End)> boxes)
        {
            var componentsInBox = new HashSet<IntVector>();

            foreach(var (Start, End) in boxes) {
                var startX = Math.Min(Start.X, End.X);
                var endX = Math.Max(Start.X, End.X);
                var startY = Math.Min(Start.Y, End.Y);
                var endY = Math.Max(Start.Y, End.Y);
                for (int x = startX; x <= endX; x++)
                {
                    for (int y = startY; y <= endY; y++)
                    {
                        AddComponentPositionAt(componentsInBox, x, y);
                    }
                }
            }
            
            return componentsInBox;
        }

        private void AddComponentPositionAt(HashSet<IntVector> componentsInBox, int x, int y)
        {
            var newComponent = Grid.ComponentMover.GetComponentAt(x, y);
            if (newComponent == null) return;
            componentsInBox.Add(new IntVector(newComponent.GridXMainTile, newComponent.GridYMainTile));
        }

        private void CreateNewAndFillGridSelections()
        {
            RemoveAllButExceptions(NewSelection);
            // add all newly selected Components that are not already in the selection list
            AddNewItemsButExceptions(OldSelection);
        }

        private void AddNewItemsButExceptions(IEnumerable<IntVector> exceptions)
        {
            foreach (var newComponentPos in NewSelection)
            {
                if (!exceptions.Contains(newComponentPos))
                {
                    SelectionManager.Selections.Add(newComponentPos);
                }
            }
        }

        /// Removes all Items except the exceptions
        private void RemoveAllButExceptions(HashSet<IntVector> removeExceptions)
        {
            foreach (var componentPos in OldSelection)
            {
                if (removeExceptions.Contains(componentPos)) continue;
                SelectionManager.Selections.Remove(componentPos);
            }
        }

        /// Removes all Items except the exceptions
        private void RemoveFromGridSelectionGroup(HashSet<IntVector> itemsToRemove)
        {
            foreach (var componentPos in OldSelection)
            {
                foreach (var itemToRemove in itemsToRemove)
                {
                    if (itemToRemove.X == componentPos.X && itemToRemove.Y == componentPos.Y)
                    {
                        SelectionManager.Selections.Remove(componentPos);
                    }
                }
            }
        }

        public override void Undo()
        {
            UnselectAllButPreviouslySelectedItems();
            NewSelection = OldSelection;
            AddNewItemsButExceptions(new HashSet<IntVector>());
        }

        private void UnselectAllButPreviouslySelectedItems()
        {
            foreach (var componentPos in NewSelection)
            {
                if (OldSelection.Contains(componentPos)) continue;
                SelectionManager.Selections.Remove(componentPos);
            }
        }

        // can merge, when the appendBehavior is equal to the current one.
        public override bool CanMergeWith(ICommand newCommand)
        {
            if (newCommand is BoxSelectComponentsCommand boxSelectionCommand && boxSelectionCommand.AppendBehavior == this.AppendBehavior && boxSelectionCommand.AppendBehavior != AppendBehaviors.CreateNew)
                return true;
            return false;
        }

        public override void MergeWith(ICommand other)
        {
            if (!CanMergeWith(other))
                throw new InvalidOperationException("Cannot merge with the provided command.");
            ExecutionParams.BoxSelections.AddRange(((BoxSelectComponentsCommand)other).ExecutionParams.BoxSelections);
        }
    }
    public class BoxSelectComponentsArgs
    {
        public BoxSelectComponentsArgs(List<(IntVector GridStart, IntVector GridEnd)> boxSelections, AppendBehaviors appendBehavior)
        {
            BoxSelections = boxSelections;
            AppendBehavior = appendBehavior;
        }
        public List<(IntVector GridStart, IntVector GridEnd)> BoxSelections { get; }
        public AppendBehaviors AppendBehavior { get; }
    }
    public enum AppendBehaviors
    {
        CreateNew,
        Append,
        Remove
    }
}
