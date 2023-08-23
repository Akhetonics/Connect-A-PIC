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
            var allConnectionsSMatrix = new SMatrix(InterComponentConnections.Select(c => c.Key.Item1).ToList());
            allConnectionsSMatrix.setValues(InterComponentConnections);
            allComponentsSMatrices.Add(allConnectionsSMatrix);
            return SMatrix.CreateSystemSMatrix(allComponentsSMatrices);
        }
        private List<SMatrix> GetAllComponentsSMatrices()
        {
            List<SMatrix> sMatrices = new();
            foreach(Tile tile in grid.Tiles)
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
            Array allSides = Enum.GetValues(typeof(RectSide));
            InterComponentConnections = new();

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    foreach (RectSide side in allSides)
                    {
                        ConnectComponentBorderEdge( x, y, side);
                    }
                }
            }
        }

        private void ConnectComponentBorderEdge( int x, int y, RectSide side)
        {
            if (!IsComponentBorderEdge(x, y, side)) return;
            if (grid.Tiles[x, y].Component == null) return;
            IntVector offset = side;
            
            Pin currentPin = grid.Tiles[x, y].GetPinAt(side);
            if (currentPin == null) return;
            if (grid.IsInGrid(x + offset.X, y + offset.Y))
            {
                var foreignPinSide = (IntVector)side * (-1);
                Pin foreignPin = grid.Tiles[x+offset.X, y+offset.Y].GetPinAt(foreignPinSide);
                if (foreignPin == null) return;
                InterComponentConnections.Add((currentPin.ID, foreignPin.ID), 1);
            }
        }

        private bool IsComponentBorderEdge(int gridx, int gridy, RectSide side)
        {
            IntVector offset = side;
            var centeredComponent = grid.Tiles[gridx, gridy].Component;
            if (!grid.IsInGrid(gridx + offset.X, gridy + offset.Y))
            {
                return true;
            }
            var offsetComponent = grid.Tiles[gridx + offset.X, gridy + offset.Y].Component;
            return centeredComponent != offsetComponent;
        }
    }
}
