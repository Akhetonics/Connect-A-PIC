using CAP_Core.Components.Creation;
using CAP_Core.Grid.FormulaReading;
using CAP_Core.Helpers;
using CAP_Core.Tiles.Grid;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CAP_Core.Components
{
    public class Component : ICloneable
    {
        public int WidthInTiles => Parts.GetLength(0);
        public int HeightInTiles => Parts.GetLength(1);
        public int TypeNumber { get; set; }
        public string Identifier{ get; set; }
        private bool IsPlacedInGrid { get; set; }
        [JsonIgnore] public int GridXMainTile { get; protected set; }
        [JsonIgnore] public int GridYMainTile { get; protected set; }
        public Part[,] Parts { get; protected set; }
        public Dictionary<int, SMatrix> LaserWaveLengthToSMatrixMap { get; set; }
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
        public Component(Dictionary<int,SMatrix> laserWaveLengthToSMatrixMap , string nazcaFunctionName, string nazcaFunctionParams, Part[,] parts, int typeNumber, string identifier, DiscreteRotation rotationCounterClock)
        {
            Parts = parts;
            TypeNumber = typeNumber;
            Identifier = identifier;
            _discreteRotation = rotationCounterClock;
            LaserWaveLengthToSMatrixMap = laserWaveLengthToSMatrixMap;
            NazcaFunctionName = nazcaFunctionName;
            NazcaFunctionParameters = nazcaFunctionParams;
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
                   $"Defined SMatrices: {LaserWaveLengthToSMatrixMap.ToCustomString<int,SMatrix>()}";
        }
        public List<Pin> GetAllPins()
        {
            return GetAllPins(Parts);
        }
        public static List<Pin> GetAllPins(Part[,]parts)
        {
            var pinList = new List<Pin>();
            foreach(var part in parts)
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
                    // clone all Parts which also clones the Pins. 
                    clonedParts[i, j] = Parts[i, j]?.Clone() as Part;
                    // set new PinIDs as they should differ from the cloned original object but cloning makes them have the same ones.
                    foreach (Pin pin in clonedParts[i, j].Pins)
                    {
                        pin.IDInFlow = Guid.NewGuid();
                        pin.IDOutFlow = Guid.NewGuid();
                    }
                }
            }

            return clonedParts;
        }

        private Dictionary<Guid, Guid> MapPinIDsWithNewIDs(Part[,] clonedParts)
        {
            Dictionary<Guid, Guid> pinIdMapping = new ();
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
            var clonedPins = GetAllPins(clonedParts);
            // Create a mapping from old pin IDs to new pin IDs
            Dictionary<Guid, Guid> oldToNewPinIds = MapPinIDsWithNewIDs(clonedParts);

            // Clone the existing connections and update with new pin IDs
            foreach (var laserAndMatrix in LaserWaveLengthToSMatrixMap)
            {
                var oldMatrix = laserAndMatrix.Value;
                var newMat = new SMatrix(oldMatrix.PinReference.Keys.ToList());
                // assign the linear connections
                newMat.SetValues(oldMatrix.GetNonNullValues());

                // now recreate the nonLinearConnections and assign them to the Matrix
                var nonLinearTransfers = new Dictionary<(Guid PinIdStart, Guid PinIdEnd), ConnectionFunction>();
                foreach (var nonLin in oldMatrix.NonLinearConnections)
                {
                    // convert the old Key to the new one.
                    var newKey = (oldToNewPinIds[nonLin.Key.PinIdStart] , oldToNewPinIds[nonLin.Key.PinIdEnd]);
                    // recreate the non linear function with the new Pins.
                    var newFunction = MathExpressionReader.ConvertToDelegate(nonLin.Value.ConnectionsFunctionRaw, clonedPins);
                    // assign the new Pin and new function to our dictionary
                    nonLinearTransfers.Add(newKey, (ConnectionFunction)newFunction);
                }
                newMat.SetNonLinearConnectionFunctions(nonLinearTransfers);
            }

            return new Component(LaserWaveLengthToSMatrixMap, NazcaFunctionName, NazcaFunctionParameters, clonedParts, TypeNumber, Identifier, Rotation90CounterClock);
        }

    }
}