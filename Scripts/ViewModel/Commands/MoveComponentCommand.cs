using Antlr4.Runtime.Misc;
using CAP_Core.Components;
using CAP_Core.Components.ComponentHelpers;
using CAP_Core.Grid;
using CAP_Core.Helpers;
using ConnectAPIC.Scripts.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ConnectAPIC.LayoutWindow.ViewModel.Commands
{
    public class MoveComponentCommand : ICommand
    {
        private readonly GridManager grid;

        public SelectionManager SelectionManager { get; }

        public event EventHandler CanExecuteChanged;

        public MoveComponentCommand(GridManager grid, SelectionManager selectionManager)
        {
            this.grid = grid;
            SelectionManager = selectionManager;
        }
        public bool CanExecute(object parameter)
        {
            if (parameter is not MoveComponentArgs args) return false;
            try
            {
                // verify that all components would be inside of grid after moving
                var moveInfoData = CollectMoveInfo(args);
                // verify that no two components would overlap at TargetArea
                bool[,] isBlockedHere = new bool [grid.Width, grid.Height];
                foreach (var moveInfo in moveInfoData)
                {
                    for(int x = 0; x < moveInfo.component.WidthInTiles; x++)
                    {
                        for ( int y = 0; y < moveInfo.component.HeightInTiles; y++)
                        {
                            int gridX = x + moveInfo.Target.X;
                            int gridY = y + moveInfo.Target.Y;
                            if (isBlockedHere[gridX, gridY] == true)
                            {
                                throw new TargetOverlappingException($"Two components targeted the same area. X:{moveInfo.Target.X} Y:{moveInfo.Target.Y}");
                            }
                            isBlockedHere[gridX, gridY] = true;
                        }
                    }
                }
            } catch (TargetOutOfGridException)
            {
                return false;
            }
            catch (TargetOverlappingException)
            {
                return false;
            }
            return true;
        }

        public Task ExecuteAsync(object parameter)
        {
            var componentAndTargets = CollectMoveInfo((MoveComponentArgs)parameter);
            UnregisterSourceAndTargetAreas(componentAndTargets);
            PlaceComponentsInTargets(componentAndTargets);
            SelectMovedComponents(componentAndTargets);
            return Task.CompletedTask;
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
                grid.PlaceComponent(compAndTarget.Target.X, compAndTarget.Target.Y, compAndTarget.component);
            }
        }

        private HashSet<(Component component, IntVector Target)> CollectMoveInfo(MoveComponentArgs args)
        {
            HashSet<(Component component, IntVector Target)> comps = new();
            foreach (var displacement in args.Displacements)
            {
                var comp = grid.GetComponentAt(displacement.Source.X, displacement.Source.Y);
                if (grid.IsInGrid(displacement.Target.X, displacement.Target.Y, comp.WidthInTiles, comp.HeightInTiles) == false)
                    throw new TargetOutOfGridException($"Target + ComponentDimensions cannot be outside the grid target: X:{displacement.Target.X}_Y:{displacement.Target.Y}");
                comps.Add((comp, displacement.Target));
            }

            return comps;
        }

        private void UnregisterSourceAndTargetAreas(HashSet<(Component component, IntVector Target)> componentAndTargets)
        {
            foreach (var componentAndTarget in componentAndTargets)
            {
                Component comp = componentAndTarget.component;
                grid.UnregisterComponentAt(comp.GridXMainTile, comp.GridYMainTile);

                // clear target landing area
                for (int x = 0; x < comp.WidthInTiles; x++)
                {
                    for (int y = 0; y < comp.HeightInTiles; y++)
                    {
                        var targetX = x + componentAndTarget.Target.X;
                        var targetY = y + componentAndTarget.Target.Y;
                        if (grid.IsInGrid(targetX, targetY) == false) continue;
                        grid.UnregisterComponentAt(targetX, targetY);
                    }
                }
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
        public MoveComponentArgs( List<(IntVector Source , IntVector Target)> displacements)
        {
            Displacements = displacements;
        }

        public List<(IntVector Source, IntVector Target)> Displacements { get; }
    }
}
