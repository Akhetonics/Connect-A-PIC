using CAP_Core.Components;
using CAP_Core.Grid;
using CAP_Core.Helpers;
using ConnectAPIC.Scripts.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConnectAPIC.Scripts.View.ToolBox
{
    public class BoxSelectComponentsCommand : ICommand
    {
        public BoxSelectComponentsCommand(GridManager grid, SelectionManager selectionManager, AppendBehaviors appendBehavior)
        {
            Grid = grid;
            SelectionManager = selectionManager;
            AppendBehavior = appendBehavior;
        }

        public GridManager Grid { get; }
        public SelectionManager SelectionManager { get; }
        public AppendBehaviors AppendBehavior { get; }
        public HashSet<IntVector> OldSelection { get; private set; }
        public HashSet<IntVector> NewSelection { get; private set; }

        public bool CanExecute(object parameter)
        {
            if (parameter is BoxSelectComponentsArgs boxParam)
            {
                if (boxParam.BoxSelections.Count == 1)
                {
                    var startX = boxParam.BoxSelections.First().GridStart;
                    var endY = boxParam.BoxSelections.First().GridEnd;
                    if(startX == endY)
                    {
                        var component = Grid.ComponentMover.GetComponentAt(startX.X, startX.Y);
                        if (component == null)
                            return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public Task ExecuteAsync(object parameter)
        {
            if (CanExecute(parameter) == false) return Task.CompletedTask;
            // select all components in box
            if (parameter is BoxSelectComponentsArgs box)
            {
                foreach (var boxSelection in box.BoxSelections)
                {
                    // define start and end to count from low to high in case the box is upside down
                    var startX = Math.Min(boxSelection.GridStart.X, boxSelection.GridEnd.X);
                    var stopX = Math.Max(boxSelection.GridStart.X, boxSelection.GridEnd.X);
                    var startY = Math.Min(boxSelection.GridStart.Y, boxSelection.GridEnd.Y);
                    var stopY = Math.Max(boxSelection.GridStart.Y, boxSelection.GridEnd.Y);

                    OldSelection = new HashSet<IntVector>(SelectionManager.Selections);
                    // collect all new Components inside the box
                    NewSelection = CollectAllComponentsInBox(startX, stopX, startY, stopY);

                    // AppendBehavior.CreateNew -> fill list with new items and remove old items
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
                }
            }
            return Task.CompletedTask;
        }

        private HashSet<IntVector> CollectAllComponentsInBox(int startX, int stopX, int startY, int stopY)
        {
            var componentsInBox = new HashSet<IntVector>();
            for (int x = startX; x <= stopX; x++)
            {
                for (int y = startY; y <= stopY; y++)
                {
                    var newComponent = Grid.ComponentMover.GetComponentAt(x, y);
                    if (newComponent != null)
                    {
                        componentsInBox.Add(new IntVector(newComponent.GridXMainTile, newComponent.GridYMainTile));
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
                foreach( var itemToRemove in itemsToRemove)
                {
                    if (itemToRemove.X == componentPos.X && itemToRemove.Y== componentPos.Y)
                    {
                        SelectionManager.Selections.Remove(componentPos);
                    }
                }
            }
        }

        public void Undo()
        {
            // should select the items that had been selected before that command was executed.
            // so remove all items that are not part of the initial selection and add all those that are missing.. 
            throw new NotImplementedException();
        }

        // can merge, when the appendBehavior is equal to the current one.
        // other is the new command..
        public bool CanMergeWith(ICommand other)
        {
            if(other is BoxSelectComponentsCommand boxSelectionCommand && boxSelectionCommand.AppendBehavior == this.AppendBehavior && boxSelectionCommand.AppendBehavior != AppendBehaviors.CreateNew)
                return true;
            return false;
        }

        public void MergeWith(ICommand other)
        {
            if(!CanMergeWith(other))
                throw new InvalidOperationException("Cannot merge with the provided command.");

        }
    }
    public class BoxSelectComponentsArgs
    {
        public BoxSelectComponentsArgs(List<(IntVector GridStart, IntVector GridEnd)> boxSelections)
        {
            BoxSelections = boxSelections;
        }
        public List<(IntVector GridStart, IntVector GridEnd)> BoxSelections { get; }
    }
    public enum AppendBehaviors
    {
        CreateNew,
        Append,
        Remove
    }
}
