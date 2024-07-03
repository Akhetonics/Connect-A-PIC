using CAP_Core;
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

        public ISelectionManager SelectionManager { get; }
        public List<Component> OldSelections { get; private set; } = new();
        public Dictionary<IntVector, Component> OldComponentsAndPositionInTargetArea { get; private set; }
        public Dictionary<IntVector, Component> OldComponentsAndPositions { get; private set; }

        public event EventHandler CanExecuteChanged;

        public MoveComponentCommand(GridManager grid, ISelectionManager selectionManager)
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
            StoreDataForUndo(parameter);
            var componentAndTargets = CollectMoveInfo(parameter);
            UnregisterSourceAndTargetAreas(componentAndTargets);
            PlaceComponentsInTargets(componentAndTargets);
            SelectMovedComponents(componentAndTargets);
            return Task.CompletedTask;
        }

        private void StoreDataForUndo(MoveComponentArgs parameter)
        {
            OldComponentsAndPositions = FetchInitialComponentPositionsForUndo(parameter);
            StoreSelectedElementsForUndo();
            StoreComponentsInTargetAreaForUndo(parameter, OldComponentsAndPositions);
        }

        // store the initial position of all elements that are about to be moved
        private Dictionary<IntVector, Component> FetchInitialComponentPositionsForUndo(MoveComponentArgs parameter)
        {
            var OldComponentsAndPositions = new Dictionary< IntVector, Component>();
            foreach (var (Source, _) in parameter.Transitions)
            {
                var oldComponent = grid.ComponentMover.GetComponentAt(Source.X, Source.Y);
                var oldPosition = new IntVector(oldComponent.GridXMainTile, oldComponent.GridYMainTile);
                OldComponentsAndPositions.Add(oldPosition, oldComponent);
            }
            return OldComponentsAndPositions;
        }

        private void StoreComponentsInTargetAreaForUndo(MoveComponentArgs parameter, Dictionary<IntVector, Component> exceptions)
        {
            // get SourceComponents 
            OldComponentsAndPositionInTargetArea = new();
            foreach (var (Source, Target) in parameter.Transitions)
            {
                var sourceComponent = grid.ComponentMover.GetComponentAt(Source.X, Source.Y);
                int sourceCmpWidth = sourceComponent.WidthInTiles;
                int sourceCmpHeight = sourceComponent.HeightInTiles;
                for(int x = 0; x < sourceCmpWidth; x++)
                {
                    for(int y = 0; y < sourceCmpHeight; y++)
                    {
                        var CmpInTargetArea = grid.ComponentMover.GetComponentAt(Target.X+x, Target.Y+y);
                        if (CmpInTargetArea == null) continue;
                        var TargetCmpPosition = new IntVector(CmpInTargetArea.GridXMainTile, CmpInTargetArea.GridYMainTile);
                        if(OldComponentsAndPositionInTargetArea.ContainsKey(TargetCmpPosition) == false)
                        {
                            if (exceptions.ContainsValue(CmpInTargetArea))
                            {
                                continue;
                            }
                            OldComponentsAndPositionInTargetArea.Add(TargetCmpPosition, CmpInTargetArea);
                        }
                    }
                }
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
            foreach (var (component, Target) in componentAndTargets)
            {
                grid.ComponentMover.PlaceComponent(Target.X, Target.Y, component);
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
            foreach (var (component, Target) in componentAndTargets)
            {
                Component comp = component;
                grid.ComponentMover.UnregisterComponentAt(comp.GridXMainTile, comp.GridYMainTile);

                // clear target landing area
                for (int x = 0; x < comp.WidthInTiles; x++)
                {
                    for (int y = 0; y < comp.HeightInTiles; y++)
                    {
                        var targetX = x + Target.X;
                        var targetY = y + Target.Y;
                        if (grid.TileManager.IsInGrid(targetX, targetY) == false) continue;
                        grid.ComponentMover.UnregisterComponentAt(targetX, targetY);
                    }
                }
            }
        }

        public override void Undo()
        {
            // first move the components back to where they came from
            foreach( var (_, ComponentToMove) in OldComponentsAndPositions)
            {
                // unregister the components to move them back
                grid.ComponentMover.UnregisterComponentAt(ComponentToMove.GridXMainTile, ComponentToMove.GridYMainTile);
            }
            // move back all now unregistered components
            foreach (var (StartPosition, ComponentToMove) in OldComponentsAndPositions)
            {
                grid.ComponentMover.PlaceComponent(StartPosition.X, StartPosition.Y, ComponentToMove);
            }
            // then recreate the deleted / overridden components
            foreach (var (Position, Component) in OldComponentsAndPositionInTargetArea)
            {
                grid.ComponentMover.PlaceComponent(Position.X, Position.Y, Component);
            }
            // then restore the selection
            SelectionManager.Selections.Clear();
            foreach ( var Component in OldSelections)
            {
                SelectionManager.Selections.Add(new IntVector(Component.GridXMainTile, Component.GridYMainTile));
            }
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
