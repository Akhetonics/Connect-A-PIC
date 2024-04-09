using CAP_Core.Components;
using CAP_Core.Grid;
using CAP_Core.Helpers;
using ConnectAPIC.Scripts.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Godot.WebSocketPeer;
using static System.Formats.Asn1.AsnWriter;

namespace ConnectAPIC.Scripts.View.ToolBox
{
    public class BoxSelectComponentsCommand : ICommand
    {
        public BoxSelectComponentsCommand(GridManager grid, SelectionManager selectionManager)
        {
            Grid = grid;
            SelectionManager = selectionManager;
        }

        public GridManager Grid { get; }
        public SelectionManager SelectionManager { get; }
        public AppendBehaviors AppendBehavior { get; set; }
        public bool WasExecuted { get; set; }
        public HashSet<IntVector> OldSelection { get; private set; }
        public HashSet<IntVector> NewSelection { get; private set; }

        public event EventHandler Executed;

        public bool CanExecute(object parameter)
        {
            if (parameter is BoxSelectComponentsArgs boxParam && CollectAllComponentsInBoxes(boxParam.BoxSelections).Any())
                return true;
            return false;
        }

        public Task ExecuteAsync(object parameter)
        {
            if (CanExecute(parameter) == false) return Task.CompletedTask;
            // select all components in box
            var box = (BoxSelectComponentsArgs)parameter;

            AppendBehavior = box.AppendBehavior;
            OldSelection = new HashSet<IntVector>(SelectionManager.Selections);
            NewSelection = CollectAllComponentsInBoxes(box.BoxSelections);
            
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
                for (int x = Start.X; x <= End.X; x++)
                {
                    for (int y = Start.Y; y <= End.Y; y++)
                    {
                        var newComponent = Grid.ComponentMover.GetComponentAt(x, y);
                        if (newComponent != null)
                        {
                            componentsInBox.Add(new IntVector(newComponent.GridXMainTile, newComponent.GridYMainTile));
                        }
                    }
                }
            }
            
            return componentsInBox;
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

        public void Undo()
        {
            RemoveAllButExceptions(OldSelection);
            NewSelection = OldSelection;
            AddNewItemsButExceptions(new HashSet<IntVector>());
        }

        // can merge, when the appendBehavior is equal to the current one.
        public bool CanMergeWith(ICommand newCommand)
        {
            if (newCommand is BoxSelectComponentsCommand boxSelectionCommand && boxSelectionCommand.AppendBehavior == this.AppendBehavior && boxSelectionCommand.AppendBehavior != AppendBehaviors.CreateNew)
                return true;
            return false;
        }

        public void MergeWith(ICommand other)
        {
            if (!CanMergeWith(other))
                throw new InvalidOperationException("Cannot merge with the provided command.");
            NewSelection.((BoxSelectComponentsCommand)other).NewSelection));
        }

        public void Redo()
        {
            throw new NotImplementedException();
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
