using CAP_Core.Components;
using CAP_Core.Grid;
using CAP_Core.Helpers;
using ConnectAPIC.Scripts.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConnectAPIC.LayoutWindow.ViewModel.Commands
{
    public class MoveComponentCommand : CommandBase<MoveComponentArgs>
    {
        private readonly GridManager grid;

        public SelectionManager SelectionManager { get; }
        public List<Component> OldSelections { get; private set; } = new();
        public List<(Component Component, IntVector Position)> OldComponentsAndPositionInTargetArea { get; private set; }

        public event EventHandler CanExecuteChanged;

        public MoveComponentCommand(GridManager grid, SelectionManager selectionManager)
        {
            this.grid = grid;
            SelectionManager = selectionManager;
        }
        public override bool CanExecute(object parameter)
        {
            if (parameter is not MoveComponentArgs args) return false;
            if (VerifyAllInGrid(args) == false) return false;
            var componentAndTargets = CollectMoveInfo(args); // CollectMoveInfo would throw if a target would be outside of the grid
            if (VerifyIsComponentsTargetsUniqueness(componentAndTargets)== false) return false;
            if (VerifySingleComponentIsTargetFree(args) == false) return false; // If we drag only 1 single Element, it should not override existing components
            return true;
        }

        private bool VerifySingleComponentIsTargetFree(MoveComponentArgs args)
        {
            if (args.Transitions.Count == 1)
            {
                var source = args.Transitions.First().Source;
                var target = args.Transitions.First().Target;
                var componentSource = grid.ComponentMover.GetComponentAt(source.X, source.Y);
                if(grid.ComponentMover.IsColliding(target.X, target.Y, componentSource.WidthInTiles, componentSource.HeightInTiles, componentSource))
                {
                    return false;
                }
            }
            return true;
        }
        private bool VerifyAllInGrid (MoveComponentArgs args)
        {
            foreach (var displacement in args.Transitions)
            {
                var component = grid.ComponentMover.GetComponentAt(displacement.Source.X , displacement.Source.Y);
                if (component == null) return false;
                if ( grid.TileManager.IsInGrid(displacement.Source.X, displacement.Source.Y, component.WidthInTiles, component.HeightInTiles) == false) return false;
                if ( grid.TileManager.IsInGrid(displacement.Target.X, displacement.Target.Y, component.WidthInTiles, component.HeightInTiles) == false) return false;
            }
            return true;
        }
        private bool VerifyIsComponentsTargetsUniqueness(HashSet<(Component component, IntVector Target)> componentAndTargets)
        {
            bool[,] isBlockedHere = new bool[grid.TileManager.Width, grid.TileManager.Height];
            foreach (var moveInfo in componentAndTargets)
            {
                for (int x = 0; x < moveInfo.component.WidthInTiles; x++)
                {
                    for (int y = 0; y < moveInfo.component.HeightInTiles; y++)
                    {
                        int gridX = x + moveInfo.Target.X;
                        int gridY = y + moveInfo.Target.Y;
                        if (isBlockedHere[gridX, gridY] == true)
                        {
                            return false;
                        }
                        isBlockedHere[gridX, gridY] = true;
                    }
                }
            }
            return true;
        }

        internal override Task ExecuteAsyncCmd(MoveComponentArgs parameter)
        {
            if (CanExecute(parameter) == false) return Task.CompletedTask;

            StoreSelectedElementsForUndo();
            StoreComponentsInTargetAreaForUndo(parameter);
            // store the initial position of all elements that are about to be moved
            foreach( var transition in parameter.Transitions)
            {
                transition.Source.
            }
            var componentAndTargets = CollectMoveInfo(parameter);
            UnregisterSourceAndTargetAreas(componentAndTargets);
            PlaceComponentsInTargets(componentAndTargets);
            SelectMovedComponents(componentAndTargets);
            return Task.CompletedTask;
        }

        private void StoreComponentsInTargetAreaForUndo(MoveComponentArgs parameter)
        {
            foreach (var targetPosition in parameter.Transitions)
            {
                var targetCmp = grid.ComponentMover.GetComponentAt(targetPosition.Target.X, targetPosition.Target.Y);
                var targetPos = new IntVector(targetCmp.GridXMainTile, targetCmp.GridYMainTile);
                OldComponentsAndPositionInTargetArea.Add((Component: targetCmp, Position: targetPos));
            }
        }

        private void StoreSelectedElementsForUndo()
        {
            OldSelections = new();
            foreach (var selection in SelectionManager.Selections)
            {
                OldSelections.Add(grid.ComponentMover.GetComponentAt(selection.X, selection.Y));
            }
        }

        private void SelectMovedComponents(HashSet<(Component component, IntVector Target)> componentAndTargets)
        {
            SelectionManager.Selections.Clear();
            foreach (var componentAndTarget in componentAndTargets)
            {
                SelectionManager.Selections.Add(new IntVector(componentAndTarget.component.GridXMainTile, componentAndTarget.component.GridYMainTile));
            }
        }

        private void PlaceComponentsInTargets(HashSet<(Component component, IntVector Target)> componentAndTargets)
        {
            foreach (var compAndTarget in componentAndTargets)
            {
                grid.ComponentMover.PlaceComponent(compAndTarget.Target.X, compAndTarget.Target.Y, compAndTarget.component);
            }
        }

        private HashSet<(Component component, IntVector Target)> CollectMoveInfo(MoveComponentArgs args)
        {
            HashSet<(Component component, IntVector Target)> comps = new();
            foreach (var displacement in args.Transitions)
            {
                var comp = grid.ComponentMover.GetComponentAt(displacement.Source.X, displacement.Source.Y);
                comps.Add((comp, displacement.Target));
            }

            return comps;
        }

        private void UnregisterSourceAndTargetAreas(HashSet<(Component component, IntVector Target)> componentAndTargets)
        {
            foreach (var componentAndTarget in componentAndTargets)
            {
                Component comp = componentAndTarget.component;
                grid.ComponentMover.UnregisterComponentAt(comp.GridXMainTile, comp.GridYMainTile);

                // clear target landing area
                for (int x = 0; x < comp.WidthInTiles; x++)
                {
                    for (int y = 0; y < comp.HeightInTiles; y++)
                    {
                        var targetX = x + componentAndTarget.Target.X;
                        var targetY = y + componentAndTarget.Target.Y;
                        if (grid.TileManager.IsInGrid(targetX, targetY) == false) continue;
                        grid.ComponentMover.UnregisterComponentAt(targetX, targetY);
                    }
                }
            }
        }

        public override void Undo()
        {
            // it could be that there was a list of components that has been dragged.
            // they had overridden the elements on the target area -> so we have to recreate the deleted elements
            // and also move all moved components back.
            // and we have to reset the selection as well
        }
    }

    
    public class MoveComponentException : Exception
    {
        public MoveComponentException(string Msg)
        {
            this.Msg = Msg;
        }

        public string Msg { get; }
    }
    public class TargetOutOfGridException : MoveComponentException
    {
        public TargetOutOfGridException(string Msg) : base(Msg)
        { }
    }
    public class TargetOverlappingException : MoveComponentException
    {
        public TargetOverlappingException(string Msg) : base(Msg)
        { }
    }
    
    public class MoveComponentArgs
    {
        public MoveComponentArgs( List<(IntVector Source , IntVector Target)> transitions)
        {
            Transitions = transitions;
        }

        public List<(IntVector Source, IntVector Target)> Transitions { get; }
    }
}
