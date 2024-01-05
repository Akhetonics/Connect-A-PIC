using CAP_Core.Components.Creation;
using CAP_Core.Helpers;
using CAP_Core.LightFlow;
using CAP_Core.Tiles;
using System.Numerics;
using System.Text.Json.Serialization;

namespace CAP_Core.Components
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
        private List<Connection> RawConnections;
        public SMatrix Connections(double waveLength) => SMatrixFactory.GetSMatrix(RawConnections, Parts, waveLength);
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
        public Component(List<Connection> connections , string nazcaFunctionName, string nazcaFunctionParameters, Part[,] parts, int typeNumber, DiscreteRotation discreteRotationCounterClock)
        {
            Parts = parts;
            TypeNumber = typeNumber;
            _discreteRotation = discreteRotationCounterClock;
            RawConnections = connections;
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
                   $"Connections Count: {RawConnections.Count}";
        }
        public List<Pin> GetAllPins()
        {
            var pinList = new List<Pin>();
            foreach(var part in Parts)
            {
                pinList.AddRange(part.Pins);
            }
            return pinList;
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
                        p.IDInFlow = Guid.NewGuid();
                        p.IDOutFlow = Guid.NewGuid();
                    }
                }
            }

            return clonedParts;
        }

        private Dictionary<Guid, Guid> MapPinIDsWithNewIDs(Part[,] clonedParts)
        {
            Dictionary<Guid, Guid> pinIdMapping = new Dictionary<Guid, Guid>();
            for (int x = 0; x < Parts.GetLength(0); x++)
            {
                for (int y = 0; y < Parts.GetLength(1); y++)
                {
                    var oldPart = Parts[x, y];
                    var newPart = clonedParts[x, y];

                    if (oldPart != null && newPart != null)
                    {
                        for (int p = 0; p < oldPart.Pins.Count; p++)
                        {
                            var oldPin = oldPart.Pins[p];
                            var newPin = newPart.Pins[p];
                            pinIdMapping[oldPin.IDInFlow] = newPin.IDInFlow;
                            pinIdMapping[oldPin.IDOutFlow] = newPin.IDOutFlow;
                        }
                    }
                }
            }

            return pinIdMapping;
        }
        public object Clone()
        {
            var clonedParts = CloneParts();
            // Create a mapping from old pin IDs to new pin IDs
            Dictionary<Guid, Guid> oldToNewPinIds = MapPinIDsWithNewIDs(clonedParts);

            // Clone the existing connections and update with new pin IDs
            var clonedRawConnections = RawConnections.Select(c => new Connection()
            {
                FromPin = oldToNewPinIds[c.FromPin],
                ToPin = oldToNewPinIds[c.ToPin],
                Magnitude = c.Magnitude,
                WireLengthNM = c.WireLengthNM
            }).ToList();
            return new Component(clonedRawConnections, NazcaFunctionName, NazcaFunctionParameters, clonedParts, TypeNumber, Rotation90CounterClock);
        }

    }
}