using CAP_Core.Helpers;
using CAP_Core.LightFlow;
using CAP_Core.Tiles;

namespace CAP_Core.Component.ComponentHelpers
{
    public abstract class ComponentBase
    {
        public int WidthInTiles => Parts.GetLength(0);
        public int HeightInTiles => Parts.GetLength(1);
        public bool IsPlacedInGrid { get; private set; }
        public int GridXMainTile { get; protected set; }
        public int GridYMainTile { get; protected set; }
        public virtual Part[,] Parts { get; protected set; }
        public SMatrix Connections { get; protected set; }
        public abstract string NazcaFunctionName { get; set; }
        public abstract string NazcaFunctionParameters { get; }
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
        protected ComponentBase()
        {
            Parts = new Part[1, 1];
            _discreteRotation = DiscreteRotation.R0;
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
            _discreteRotation = _discreteRotation.RotateBy90();
            foreach (Part part in Parts)
            {
                part.Rotation90 = _discreteRotation;
            }
        }
        public Part GetPartAtGridXY(int gridX, int gridY)
        {
            int offsetX = gridX - GridXMainTile;
            int offsetY = gridY - GridYMainTile;
            return GetPartAt(offsetX, offsetY);
        }
        public Part GetPartAt(int offsetX, int offsetY)
        {
            if (offsetX < 0 || offsetY < 0 || offsetX >= WidthInTiles || offsetY >= HeightInTiles)
            {
                return null;
            }
            return Parts[offsetX, offsetY];
        }
        public Part CreatePart(params RectSide[] LightTransmittingSides)
        {
            var part = new Part();
            foreach (RectSide side in LightTransmittingSides)
            {
                part.InitializePin(side, null, MatterType.Light);
            }
            return part;
        }
        public Guid PinIdRightIn(int offsetX = 0, int offsetY = 0)
        {
            return Parts[offsetX, offsetY].GetPinAt(RectSide.Right).IDInFlow;
        }
        public Guid PinIdRightOut(int offsetX = 0, int offsetY = 0)
        {
            return Parts[offsetX, offsetY].GetPinAt(RectSide.Right).IDOutFlow;
        }
        public Guid PinIdDownIn(int offsetX = 0, int offsetY = 0)
        {
            return Parts[offsetX, offsetY].GetPinAt(RectSide.Down).IDInFlow;
        }
        public Guid PinIdDownOut(int offsetX = 0, int offsetY = 0)
        {
            return Parts[offsetX, offsetY].GetPinAt(RectSide.Down).IDOutFlow;
        }
        public Guid PinIdLeftIn(int offsetX = 0, int offsetY = 0)
        {
            return Parts[offsetX, offsetY].GetPinAt(RectSide.Left).IDInFlow;
        }
        public Guid PinIdLeftOut(int offsetX = 0, int offsetY = 0)
        {
            return Parts[offsetX, offsetY].GetPinAt(RectSide.Left).IDOutFlow;
        }
        public Guid PinIdUpIn(int offsetX = 0, int offsetY = 0)
        {
            return Parts[offsetX, offsetY].GetPinAt(RectSide.Up).IDInFlow;
        }
        public Guid PinIdUpOut(int offsetX = 0, int offsetY = 0)
        {
            return Parts[offsetX, offsetY].GetPinAt(RectSide.Up).IDOutFlow;
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
    }
}