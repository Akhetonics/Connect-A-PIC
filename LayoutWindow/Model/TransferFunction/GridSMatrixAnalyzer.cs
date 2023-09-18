using ConnectAPIC.LayoutWindow.Model.ExternalPorts;
using ConnectAPIC.LayoutWindow.Model.Helpers;
using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using MathNet.Numerics.LinearAlgebra;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Tiles;
using TransferFunction;

public record LightFlow
{
    public Complex LightInFlow;
    public Complex LightOutFlow;
}
namespace ConnectAPIC.Scenes.TransferFunction
{
    public class GridSMatrixAnalyzer
    {
        public readonly Grid Grid;
        public Dictionary<(Guid, Guid), Complex> InterComponentConnections { get; private set; }
        public SMatrix SystemSMatrix { get; private set; }
        private Dictionary<Guid, Complex> lightPropagation;
        public Dictionary<Guid,Complex> LightPropagation { 
            get {
                lightPropagation ??= ReCalculateLightPropagation();
                return lightPropagation;
            } }
        public GridSMatrixAnalyzer(Grid grid)
        {
            this.Grid = grid;
            UpdateSystemSMatrix();
        }

        // calculates the light intensity and phase at a given PIN-ID for both light-flow-directions "in" and "out" for a given period of steps
        private Dictionary<Guid,Complex> ReCalculateLightPropagation()
        {
            var stepCount = SystemSMatrix.PinReference.Count() * 2;
            var usedInputs = Grid.GetUsedStandardInputs();
            var inputVector = UsedStandardInputConverter.ToVector(usedInputs, SystemSMatrix);
            return SystemSMatrix.GetLightPropagation(inputVector, stepCount);
        }

        private void UpdateSystemSMatrix()
        {
            var allComponentsSMatrices = GetAllComponentsSMatrices();
            SMatrix allConnectionsSMatrix = CreateAllConnectionsMatrix();
            allComponentsSMatrices.Add(allConnectionsSMatrix);
            SystemSMatrix = SMatrix.CreateSystemSMatrix(allComponentsSMatrices);
        }

        public SMatrix CreateAllConnectionsMatrix()
        {
            if(InterComponentConnections == null ||InterComponentConnections.Count <= 0)
            {
                CalcAllConnectionsBetweenComponents();
            }
            var allUsedPinIDs = InterComponentConnections.SelectMany(c => new[] { c.Key.Item1, c.Key.Item2 }).Distinct().ToList();
            var allConnectionsSMatrix = new SMatrix(allUsedPinIDs);
            allConnectionsSMatrix.SetValues(InterComponentConnections);
            return allConnectionsSMatrix;
        }

        public List<SMatrix> GetAllComponentsSMatrices()
        {
            List<SMatrix> sMatrices = new();
            foreach (Tile tile in Grid.Tiles)
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
            int gridWidth = Grid.Tiles.GetLength(0);
            int gridHeight = Grid.Tiles.GetLength(1);
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
                if (Grid.Tiles[x, y].Component == null) continue;
                if (!Grid.IsInGrid(x + offset.X, y + offset.Y)) continue;
                var foreignTile = Grid.Tiles[x + offset.X, y + offset.Y];
                if (!IsComponentBorderEdge(x, y, foreignTile)) continue;
                Pin currentPin = Grid.Tiles[x, y].GetPinAt(side);
                if (currentPin == null) continue;
                var foreignPinSide = offset * (-1);
                Pin foreignPin = foreignTile.GetPinAt(foreignPinSide);
                if (foreignPin == null) continue;
                
                InterComponentConnections.Add((currentPin.IDOutFlow, foreignPin.IDInFlow), 1);
            }
        }

        private bool IsComponentBorderEdge(int gridx, int gridy, Tile foreignTile)
        {
            if (foreignTile == null) return false;
            var centeredComponent = Grid.Tiles[gridx, gridy].Component;
            return centeredComponent != foreignTile.Component;
        }

        public override string ToString() => new GridSMatrixPrinter(this).ToString();
    }
}
