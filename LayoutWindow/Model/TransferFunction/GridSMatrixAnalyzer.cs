using ConnectAPIC.LayoutWindow.Model.Helpers;
using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Tiles;
using TransferFunction;

namespace ConnectAPIC.Scenes.TransferFunction
{
    public class GridSMatrixAnalyzer
    {
        private readonly Grid grid;
        private Dictionary<(Guid, Guid), Complex> InterComponentConnections;

        public GridSMatrixAnalyzer(Grid grid)
        {
            this.grid = grid;
        }

        public SMatrix CreateSystemSMatrix()
        {
            CalcAllConnectionsBetweenComponents();
            var allComponentsSMatrices = GetAllComponentsSMatrices();
            var allUsedPinIDs = InterComponentConnections.Select(c => c.Key.Item1).Distinct().ToList();
            var allConnectionsSMatrix = new SMatrix(allUsedPinIDs); // it is enough to get all Item1s-PinIDs as the Item2s are identically as they all have a backlink
            allConnectionsSMatrix.SetValues(InterComponentConnections);
            allComponentsSMatrices.Add(allConnectionsSMatrix);
            return SMatrix.CreateSystemSMatrix(allComponentsSMatrices);
        }
        private List<SMatrix> GetAllComponentsSMatrices()
        {
            List<SMatrix> sMatrices = new();
            foreach (Tile tile in grid.Tiles)
            {
                if (tile.Component == null) continue;
                if (tile.Component.Connections == null) continue;
                if (!sMatrices.Contains(tile.Component.Connections))
                {
                    sMatrices.Add(tile.Component.Connections);
                }
            }
            return sMatrices;
        }
        private void CalcAllConnectionsBetweenComponents()
        {
            int gridWidth = grid.Tiles.GetLength(0);
            int gridHeight = grid.Tiles.GetLength(1);
            InterComponentConnections = new();

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    ConnectAllBorderEdgesOfComponentAt(x, y);
                }
            }
        }

        private void ConnectAllBorderEdgesOfComponentAt(int x, int y)
        {
            Array allSides = Enum.GetValues(typeof(RectSide));
            foreach (RectSide side in allSides)
            {
                IntVector offset = side;
                if (grid.Tiles[x, y].Component == null) continue;
                if (!grid.IsInGrid(x + offset.X, y + offset.Y)) continue;
                var foreignTile = grid.Tiles[x + offset.X, y + offset.Y];
                if (!IsComponentBorderEdge(x, y, foreignTile)) continue;
                Pin currentPin = grid.Tiles[x, y].GetPinAt(side);
                if (currentPin == null) continue;
                var foreignPinSide = offset * (-1);
                Pin foreignPin = foreignTile.GetPinAt(foreignPinSide);
                if (foreignPin == null) continue;
                
                InterComponentConnections.Add((currentPin.ID, foreignPin.ID), 1);
                
            }
        }

        private bool IsComponentBorderEdge(int gridx, int gridy, Tile foreignTile)
        {
            if (foreignTile == null) return false;
            var centeredComponent = grid.Tiles[gridx, gridy].Component;
            return centeredComponent != foreignTile.Component;
        }
    }
}
