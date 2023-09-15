using ConnectAPIC.LayoutWindow.Model.Helpers;
using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using MathNet.Numerics.LinearAlgebra;
using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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
        private readonly Grid grid;
        private Dictionary<(Guid, Guid), Complex> InterComponentConnections;
        public SMatrix SystemSMatrix { get; private set; }
        private Dictionary<Guid, Complex> lightPropagation;
        public Dictionary<Guid,Complex> LightPropagation { 
            get {
                if (lightPropagation == null)
                {
                    lightPropagation = ReCalculateLightPropagation();
                }
                return lightPropagation;
            } }
        public GridSMatrixAnalyzer(Grid grid)
        {
            this.grid = grid;
            UpdateSystemSMatrix();
        }

        // calculates the light intensity and phase at a given PIN-ID for both light-flow-directions "in" and "out" for a given period of steps
        private Dictionary<Guid,Complex> ReCalculateLightPropagation()
        {
            var stepCount = SystemSMatrix.PinReference.Count() * 2;
            var usedInputs = grid.GetUsedStandardInputs();
            var inputVector = UsedStandardInputConverter.ToVector(usedInputs, SystemSMatrix);
            return SystemSMatrix.GetLightPropagation(inputVector, stepCount);
        }

        private void UpdateSystemSMatrix()
        {
            CalcAllConnectionsBetweenComponents();
            var allComponentsSMatrices = GetAllComponentsSMatrices();
            var allUsedPinIDs = InterComponentConnections.SelectMany(c => new[] { c.Key.Item1, c.Key.Item2 }).Distinct().ToList();
            var allConnectionsSMatrix = new SMatrix(allUsedPinIDs);
            allConnectionsSMatrix.SetValues(InterComponentConnections);
            allComponentsSMatrices.Add(allConnectionsSMatrix);
            SystemSMatrix = SMatrix.CreateSystemSMatrix(allComponentsSMatrices);
            
            string Debug_allConnections = GetSMatrixWithPinNames(allConnectionsSMatrix);
            string Degug_FirstCompMatrix = GetSMatrixWithPinNames(allComponentsSMatrices[0]);
            string Degug_2CompMatrix = GetSMatrixWithPinNames(allComponentsSMatrices[1]);
            string Degug_3CompMatrix = GetSMatrixWithPinNames(allComponentsSMatrices[2]);
            string Degug_SystemSMatrix = GetSMatrixWithPinNames(SystemSMatrix);

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
                
                InterComponentConnections.Add((currentPin.IDOutFlow, foreignPin.IDInFlow), 1);
            }
        }

        private bool IsComponentBorderEdge(int gridx, int gridy, Tile foreignTile)
        {
            if (foreignTile == null) return false;
            var centeredComponent = grid.Tiles[gridx, gridy].Component;
            return centeredComponent != foreignTile.Component;
        }

        public override string ToString()
        {
            var allPinsInField = GetAllPinShortNames();
            var outputstring = GetSMatrixWithPinNames(this.SystemSMatrix);
            outputstring += "\n\nLightPropagationVector:\n\n";

            foreach (var lightIntensity in lightPropagation)
            {
                outputstring += $"({allPinsInField[lightIntensity.Key]}\t{lightIntensity.Key}\t{lightIntensity.Value})\n";
            }

            return outputstring;
        }
        public string GetSMatrixWithPinNames(SMatrix matrix)
        {
            if (matrix == null) return "" ;
            // get the smatrix tostring of the whole systemmatrix, also get the LightPropagation vector, replace the IDs of the pins with the name of the pin
            var allPinsInField = GetAllPinShortNames();
            var outputstring = matrix.ToString();
            foreach (Guid guid in matrix.PinReference)
            {
                outputstring = outputstring.Replace(guid.ToString()[..SMatrix.MaxToStringPinGuidSize], allPinsInField[guid]);
            }
            return outputstring;
        }
        private Dictionary<Guid, string> GetAllPinShortNames()
        {
            Dictionary<Guid, string> PinsProcessed = new();
            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    var component = grid.GetComponentAt(x, y);
                    if (component == null) continue;
                    var part = component.GetPartAtGridXY(x, y);
                    foreach (RectSide side in Enum.GetValues(typeof(RectSide)))
                    {
                        var pin = part.GetPinAt(side);
                        if (pin == null) continue;
                        var componentTypeName = component.GetType().Name[..3];
                        var sideShort = Enum.GetName(typeof(RectSide), side)[..1];
                        var pinname = $"{componentTypeName}[{x},{y}]{sideShort}i";
                        PinsProcessed.Add(pin.IDInFlow, pinname);
                        pinname = $"{componentTypeName}[{x},{y}]{sideShort}o";
                        PinsProcessed.Add(pin.IDOutFlow, pinname);
                    }
                }
            }
            return PinsProcessed;
        }
    }
}
