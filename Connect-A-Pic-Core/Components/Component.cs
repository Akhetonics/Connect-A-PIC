using CAP_Core.Components.Creation;
using CAP_Core.Components.FormulaReading;
using CAP_Core.Grid.FormulaReading;
using CAP_Core.Helpers;
using System.Collections.Generic;
using System.Numerics;
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
        public Dictionary<int, SMatrix> WaveLengthToSMatrixMap { get; set; }
        private Dictionary<int, Slider> SliderMap { get; set; } // where int is the sliderNumber
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
        public Component(Dictionary<int,SMatrix> laserWaveLengthToSMatrixMap , List<Slider> sliders, string nazcaFunctionName, string nazcaFunctionParams, Part[,] parts, int typeNumber, string identifier, DiscreteRotation rotationCounterClock)
        {
            Parts = parts;
            TypeNumber = typeNumber;
            Identifier = identifier;
            _discreteRotation = rotationCounterClock;
            WaveLengthToSMatrixMap = laserWaveLengthToSMatrixMap;
            SliderMap = new();
            sliders.ForEach(s => {
                AddSlider(s.Number, s);
                // initialize the SliderValues with two different values to ensure the Observers (Matrix Updater) are being called
                s.Value = 0; 
                s.Value = (s.MinValue + s.MaxValue) / 2;
            });
            NazcaFunctionName = nazcaFunctionName;
            NazcaFunctionParameters = nazcaFunctionParams;
        }
        public void AddSlider(int sliderNr , Slider slider)
        {
            SliderMap.TryAdd(sliderNr, slider);
            slider.PropertyChanged += Slider_PropertyChanged;
        }

        private void Slider_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(Slider.Value) && sender is Slider slider)
            {
                foreach (var sMatrix in WaveLengthToSMatrixMap.Values)
                {
                    if (sMatrix.SliderReference.ContainsKey(slider.ID))
                    {
                        sMatrix.SliderReference[slider.ID] = slider.Value;
                    }
                    else
                    {
                        sMatrix.SliderReference.Add(slider.ID, slider.Value);
                    }
                }
            }
        }

        public Slider GetSlider (int sliderNr)
        {
            return SliderMap[sliderNr];
        }
        public List<Slider> GetAllSliders()
        {
            return SliderMap.Values.ToList();
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
                   $"Defined SMatrices: {WaveLengthToSMatrixMap.ToCustomString<int,SMatrix>()}";
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
            var clonedSliderMap = CloneSliders();
            var clonedPins = GetAllPins(clonedParts);
            var allClonedPinIDs = clonedPins.SelectMany(p => new[] { p.IDInFlow, p.IDOutFlow }).ToList();
            var allClonedSliderIDs = clonedSliderMap.Select(s => s.Value.ID).ToList();
            // Create a mapping from old pin IDs to new pin IDs
            var oldToNewPinIds = MapPinIDsWithNewIDs(clonedParts);

            // Clone the existing connections and update with new pin IDs
            Dictionary<int, SMatrix> clonedLaserSMatrixMap = new();
            foreach (var laserAndMatrix in WaveLengthToSMatrixMap)
            {
                var oldMatrix = laserAndMatrix.Value;
                
                var newMat = new SMatrix(allClonedPinIDs , allClonedSliderIDs);
                // assign the linear connections
                var newConnections = CreateConnectionsWithUpdatedPins(oldToNewPinIds, oldMatrix);
                newMat.SetValues(newConnections);

                // now recreate the nonLinearConnections and assign them to the Matrix
                foreach (var nonLin in oldMatrix.NonLinearConnections)
                {
                    // convert the old Key to the new one.
                    var newKey = (oldToNewPinIds[nonLin.Key.PinIdStart], oldToNewPinIds[nonLin.Key.PinIdEnd]);
                    // recreate the non linear function with the new Pins.
                    var newFunction = MathExpressionReader.ConvertToDelegate(nonLin.Value.ConnectionsFunctionRaw, clonedPins, clonedSliderMap.Values.ToList());
                    // assign the new Pin and new function to our dictionary
                    newMat.NonLinearConnections.Add(newKey, (ConnectionFunction)newFunction);
                }
                clonedLaserSMatrixMap.Add(laserAndMatrix.Key, newMat);
            }

            return new Component(clonedLaserSMatrixMap, clonedSliderMap.Values.ToList(), NazcaFunctionName, NazcaFunctionParameters, clonedParts, TypeNumber, Identifier, Rotation90CounterClock);
        }

        private Dictionary<int, Slider> CloneSliders()
        {
            var clonedSliderMap = new Dictionary<int, Slider>();
            foreach (var sliderID in SliderMap.Keys)
            {
                var slider = SliderMap[sliderID];
                var clonedSlider = (Slider)slider.Clone();
                clonedSlider.ID = Guid.NewGuid();
                clonedSlider.Value = slider.Value;
                clonedSliderMap.Add(slider.Number, clonedSlider);
            }

            return clonedSliderMap;
        }

        private static Dictionary<(Guid,Guid),Complex> CreateConnectionsWithUpdatedPins(Dictionary<Guid, Guid> oldToNewPinIds, SMatrix oldMatrix)
        {
            var newConnections = new Dictionary<(Guid, Guid), Complex>();
            foreach (var oldConnection in oldMatrix.GetNonNullValues())
            {
                var newKey = (oldToNewPinIds[oldConnection.Key.PinIdStart], oldToNewPinIds[oldConnection.Key.PinIdEnd]);
                newConnections.Add(newKey, oldConnection.Value);
            }
            return newConnections;
        }
    }
}