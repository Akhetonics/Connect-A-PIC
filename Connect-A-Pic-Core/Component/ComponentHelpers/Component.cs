using CAP_Core.Helpers;
using CAP_Core.LightFlow;
using CAP_Core.Tiles;
using System.Numerics;
using System.Text.Json.Serialization;

namespace CAP_Core.Component.ComponentHelpers
{
    public class Component : ICloneable
    {
        public int WidthInTiles => Parts.GetLength(0);
        public int HeightInTiles => Parts.GetLength(1);
        public int TypeNumber { get; set; }
        private bool IsPlacedInGrid { get; set; }
        [JsonIgnore] public int GridXMainTile { get; protected set; }
        [JsonIgnore] public int GridYMainTile { get; protected set; }
        public Part[,] Parts { get; protected set; }
        public SMatrix Connections { get; protected set; }
        public string NazcaFunctionName { get; set; }
        public string NazcaFunctionParameters { get; set; }
        private DiscreteRotation _discreteRotation;
        public DiscreteRotation Rotation90CounterClock
        {
            get => _discreteRotation;
            set
            {
                int rotationIntervals = _discreteRotation.CalculateCyclesTillTargetRotation(value);
                for (int i = 0; i < rotationIntervals; i++)
                {
                    RotateBy90CounterClockwise();
                }
            }
        }
        public Component(SMatrix connections ,string nazcaFunctionName, string nazcaFunctionParameters, Part[,] parts, int typeNumber, DiscreteRotation discreteRotationCounterClock)
        {
            Parts = parts;
            TypeNumber = typeNumber;
            _discreteRotation = discreteRotationCounterClock;
            Connections = connections;
            NazcaFunctionName = nazcaFunctionName;
            NazcaFunctionParameters = nazcaFunctionParameters;
        }

        public void RegisterPositionInGrid(int gridX, int gridY)
        {
            IsPlacedInGrid = true;
            GridXMainTile = gridX;
            GridYMainTile = gridY;
        }
        public void ClearGridData()
        {
            IsPlacedInGrid = false;
            GridXMainTile = -1;
            GridYMainTile = -1;
        }
        public void RotateBy90CounterClockwise()
        {
            Parts = Parts.RotateCounterClockwise();
            _discreteRotation = _discreteRotation.RotateBy90CounterC();
            foreach (Part part in Parts)
            {
                part.Rotation90 = _discreteRotation;
            }
        }
        public Part? GetPartAtGridXY(int gridX, int gridY)
        {
            int offsetX = gridX - GridXMainTile;
            int offsetY = gridY - GridYMainTile;
            return GetPartAt(offsetX, offsetY);
        }
        public Part? GetPartAt(int offsetX, int offsetY)
        {
            if (offsetX < 0 || offsetY < 0 || offsetX >= WidthInTiles || offsetY >= HeightInTiles)
            {
                return null;
            }
            return Parts[offsetX, offsetY];
        }
        public Part? CreatePart(params RectSide[] LightTransmittingSides)
        {
            var part = new Part();
            foreach (RectSide side in LightTransmittingSides)
            {
                part.InitializePin(side, null, MatterType.Light);
            }
            return part;
        }
        public override string ToString()
        {
            return $"Nazca Name: {NazcaFunctionName} \n" +
                   $"Parameters: {NazcaFunctionParameters} \n" +
                   $"Width in Tiles: {WidthInTiles} \n" +
                   $"Height in Tiles: {HeightInTiles} \n" +
                   $"Is Placed in Grid: {IsPlacedInGrid} \n" +
                   $"Grid X (Main Tile): {GridXMainTile} \n" +
                   $"Grid Y (Main Tile): {GridYMainTile} \n" +
                   $"Rotation: {Rotation90CounterClock} \n" +
                   $"Parts Length: {Parts?.Length} \n" +
                   $"Connections PinReferences Count: {Connections?.PinReference?.Count}";
        }
        private Part[,] CloneParts()
        {
            Part[,] clonedParts = new Part[Parts.GetLength(0), Parts.GetLength(1)];

            for (int i = 0; i < Parts.GetLength(0); i++)
            {
                for (int j = 0; j < Parts.GetLength(1); j++)
                {
                    // set new PinIDs as they should differ from the cloned original object but cloning makes them have the same ones.
                    clonedParts[i, j] = Parts[i, j]?.Clone() as Part;
                    foreach (Pin p in clonedParts[i, j].Pins)
                    {
                        p.IDInFlow = new Guid();
                        p.IDOutFlow = new Guid();
                    }
                }
            }

            return clonedParts;
        }
        public object Clone()
        {
            var clonedParts = CloneParts();
            // Create a mapping from old pin IDs to new pin IDs
            Dictionary<Guid, Guid> oldToNewPinIds = new Dictionary<Guid, Guid>();
            List<Guid> newPinIds = new List<Guid>();
            for (int i = 0; i < Parts.GetLength(0); i++)
            {
                for (int j = 0; j < Parts.GetLength(1); j++)
                {
                    var oldPart = Parts[i, j];
                    var newPart = clonedParts[i, j];

                    if (oldPart != null && newPart != null)
                    {
                        for (int k = 0; k < oldPart.Pins.Count; k++)
                        {
                            var oldPin = oldPart.Pins[k];
                            var newPin = newPart.Pins[k];
                            oldToNewPinIds[oldPin.IDInFlow] = newPin.IDInFlow;
                            oldToNewPinIds[oldPin.IDOutFlow] = newPin.IDOutFlow;
                            newPinIds.Add(newPin.IDInFlow);
                            newPinIds.Add(newPin.IDOutFlow);
                        }
                    }
                }
            }

            // Clone the existing connections and update with new pin IDs
            var oldConnections = this.Connections.GetNonNullValues();
            var newConnections = new Dictionary<(Guid, Guid), Complex>();

            foreach (var oldConnection in oldConnections)
            {
                var oldInflowId = oldConnection.Key.Item1;
                var oldOutflowId = oldConnection.Key.Item2;
                var newInflowId = oldToNewPinIds[oldInflowId];
                var newOutflowId = oldToNewPinIds[oldOutflowId];
                newConnections[(newInflowId, newOutflowId)] = oldConnection.Value;
            }

            var clonedSMatrix = new SMatrix(newPinIds);
            clonedSMatrix.SetValues(newConnections);
            var newComponent = new Component(clonedSMatrix, NazcaFunctionName, NazcaFunctionParameters, clonedParts, TypeNumber, Rotation90CounterClock);
            return newComponent;
        }
    }
}