using CAP_Core.Helpers;
using CAP_Core.LightFlow;
using CAP_Core.Tiles;
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
                    clonedParts[i, j] = Parts[i, j]?.Clone() as Part;
                }
            }

            return clonedParts;
        }
        public object Clone()
        {
            var newComponent = new Component((SMatrix)Connections.Clone(), NazcaFunctionName, NazcaFunctionParameters, CloneParts(), TypeNumber, Rotation90CounterClock);
            return newComponent;
        }
    }
}