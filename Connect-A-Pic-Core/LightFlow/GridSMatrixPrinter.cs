﻿using CAP_Core.ExternalPorts;
using CAP_Core.Tiles;

namespace CAP_Core.LightFlow
{
    public class GridSMatrixPrinter
    {
        public GridSMatrixAnalyzer Analyzer { get; }

        public GridSMatrixPrinter(GridSMatrixAnalyzer gridSMatrixAnalyzer)
        {
            Analyzer = gridSMatrixAnalyzer;
        }
        private string GetSMatrixWithPinNames(SMatrix matrix)
        {
            if (matrix == null) return "";
            var allPinsInField = GetAllPinShortNames();
            var matrixStringified = matrix.ToString();
            foreach (Guid guid in matrix.PinReference)
            {
                matrixStringified = matrixStringified.Replace(guid.ToString()[..SMatrix.MaxToStringPinGuidSize], allPinsInField[guid]);
            }
            return matrixStringified;
        }
        private Dictionary<Guid, string> GetAllPinShortNames()
        {
            Dictionary<Guid, string> PinsProcessed = new();
            for (int x = 0; x < Analyzer.Grid.Width; x++)
            {
                for (int y = 0; y < Analyzer.Grid.Height; y++)
                {
                    var component = Analyzer.Grid.GetComponentAt(x, y);
                    if (component == null) continue;
                    var part = component.GetPartAtGridXY(x, y);
                    foreach (RectSide side in Enum.GetValues(typeof(RectSide)))
                    {
                        var pin = part.GetPinAt(side);
                        if (pin == null) continue;
                        var componentTypeName = component.GetType().Name[..3];
                        var sideShort = Enum.GetName(typeof(RectSide), side)[..1];
                        var pinName = $"{componentTypeName}[{x},{y}]{sideShort}i";
                        PinsProcessed.Add(pin.IDInFlow, pinName);
                        pinName = $"{componentTypeName}[{x},{y}]{sideShort}o";
                        PinsProcessed.Add(pin.IDOutFlow, pinName);
                    }
                }
            }
            return PinsProcessed;
        }

        public override string ToString()
        {
            string Debug_allConnections = GetSMatrixWithPinNames(Analyzer.CreateAllConnectionsMatrix());
            var allComponentsSMatrices = Analyzer.GetAllComponentsSMatrices();
            string SystemSMatrixWithNamedPins = GetSMatrixWithPinNames(Analyzer.SystemSMatrix);

            string all = $"All connections: \n{Debug_allConnections}\n";
            int componentNumber = 1;
            foreach (SMatrix componentMatrix in allComponentsSMatrices)
            {
                var Debug_CompMatrix = GetSMatrixWithPinNames(componentMatrix);
                all += $"Component {componentNumber}\n{Debug_CompMatrix}\n";
                componentNumber++;
            }
            all += $"SystemMatrix \n{SystemSMatrixWithNamedPins}";

            var allPinsInField = GetAllPinShortNames();
            var outputString = all;
            outputString += "\n\nLightPropagationVector:\n\n";

            foreach (var color in Enum.GetValues(typeof(LightColor)).OfType<LightColor>())
            {
                foreach (var lightIntensity in Analyzer.CalculateLightPropagation(color))
                {
                    string lightColorName = Enum.GetName(typeof(LightColor), color);
                    outputString += $"LightColor: {lightColorName}\t{allPinsInField[lightIntensity.Key]}\t{lightIntensity.Key}\t{lightIntensity.Value}\n";
                }
            }


            return outputString;
        }
    }
}
