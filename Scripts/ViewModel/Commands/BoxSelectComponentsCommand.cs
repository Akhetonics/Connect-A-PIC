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
        public BoxSelectComponentsCommand(GridManager grid, SelectionManager selectionManager)
        {
            Grid = grid;
            SelectionManager = selectionManager;
        }

        public GridManager Grid { get; }
        public SelectionManager SelectionManager { get; }
        public HashSet<IntVector> OldSelection { get; private set; }
        public HashSet<IntVector> NewSelection { get; private set; }

        public bool CanExecute(object parameter)
        {
            return parameter is BoxSelectComponentsArgs;
        }

        public Task ExecuteAsync(object parameter)
        {
            if (CanExecute(parameter) == false) return Task.CompletedTask;
            // select all components in box
            if (parameter is BoxSelectComponentsArgs box)
            {
                // define start and end to count from low to high in case the box is upside down
                var startX = Math.Min(box.GridStart.X, box.GridEnd.X);
                var stopX = Math.Max(box.GridStart.X, box.GridEnd.X);
                var startY = Math.Min(box.GridStart.Y, box.GridEnd.Y);
                var stopY = Math.Max(box.GridStart.Y, box.GridEnd.Y);

                OldSelection = new HashSet<IntVector>(SelectionManager.Selections);
                // collect all new Components inside the box
                NewSelection = CollectAllComponentsInBox(startX, stopX, startY, stopY);

                // AppendBehavior.CreateNew -> fill list with new items and remove old items
                if (box.AppendBehaviour == AppendBehaviors.CreateNew)
                {
                    CreateNewAndFillGridSelections();
                }
                else if (box.AppendBehaviour == AppendBehaviors.Append)
                {
                    AddNewItemsButExceptions(OldSelection);
                }
                else if (box.AppendBehaviour == AppendBehaviors.Remove) // remove the NewSelection From OldSelection
                {
                    RemoveFromGridSelectionGroup(NewSelection);
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
                    var newComponent = Grid.GetComponentAt(x, y);
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
    }
    public class BoxSelectComponentsArgs
    {
        public BoxSelectComponentsArgs(IntVector gridStart, IntVector gridEnd, AppendBehaviors appendBehaviour)
        {
            GridStart = gridStart;
            GridEnd = gridEnd;
            AppendBehaviour = appendBehaviour;
        }

        public IntVector GridStart { get; }
        public IntVector GridEnd { get; }
        public AppendBehaviors AppendBehaviour { get; }
    }
    public enum AppendBehaviors
    {
        CreateNew,
        Append,
        Remove
    }
}
