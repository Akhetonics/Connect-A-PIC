using System.Text;
using System.Numerics;
using MathNet.Numerics.LinearAlgebra;
using System.Linq.Dynamic.Core;
using CAP_Core.Components.FormulaReading;

namespace CAP_Core.Components
{
    public class SMatrix 
    {
        public Matrix<Complex> SMat; // the SMat works like SMat[PinNROutflow, PinNRInflow] --> so opposite from what one might expect
        public readonly Dictionary<Guid, int> PinReference; // all PinIDs inside of the matrix. the int is the index of the row/column in the SMat.. and also of the inputVector.
        public Dictionary<Guid, double> SliderReference { get; internal set; }
        private readonly Dictionary<int, Guid> ReversePinReference; // sometimes we want to find the GUID and only have the ID
        private readonly int size;
        public const int MaxToStringPinGuidSize = 6;
        public Dictionary<(Guid PinIdStart, Guid PinIdEnd), ConnectionFunction> NonLinearConnections { get; set; }

        public SMatrix(List<Guid> allPinsInGrid, List<(Guid sliderID ,double value)> AllSliders)
        {
            if (allPinsInGrid != null && allPinsInGrid.Count > 0)
            {
                size = allPinsInGrid.Count;
            }
            else
            {
                size = 0;
            }

            SMat = Matrix<Complex>.Build.Dense(size, size);
            // initialize PinReferences
            PinReference = new();
            ReversePinReference = new();
            int i = 0;
            foreach( var pin in allPinsInGrid)
            {
                PinReference.Add(pin , i);
                ReversePinReference.Add(i, pin);
                i++;
            }
            NonLinearConnections = new();
            SliderReference = new();
            foreach( var slider in AllSliders)
            {
                SliderReference.Add(slider.sliderID,slider.value );
            }
        }

        public void SetValues(Dictionary<(Guid PinIdInflow, Guid PinIdOutflow), Complex> transfers, bool reset = false)
        {
            if (transfers == null || PinReference == null)
            {
                return;
            }

            if (reset)
            {
                SMat = Matrix<Complex>.Build.Dense(size, size);
            }

            foreach (var relation in transfers.Keys)
            {
                if (PinReference.ContainsKey(relation.PinIdInflow) && PinReference.ContainsKey(relation.PinIdOutflow))
                {
                    int indexInflow = PinReference[relation.PinIdInflow];
                    int indexOutflow = PinReference[relation.PinIdOutflow];
                    SMat[indexOutflow, indexInflow] = transfers[relation];
                }
            }
        }

        public Dictionary<(Guid PinIdStart, Guid PinIdEnd), Complex> GetNonNullValues()
        {
            var transfers = new Dictionary<(Guid inflow, Guid outflow), Complex>();
            for (int iOut = 0; iOut < size; iOut++)
            {
                for (int iIn = 0; iIn < size; iIn++)
                {
                    if (SMat[iOut, iIn] == Complex.Zero) continue;
                    transfers[(ReversePinReference[iIn], ReversePinReference[iOut])] = SMat[iOut, iIn];
                }
            }
            return transfers;
        }

        public static SMatrix CreateSystemSMatrix(List<SMatrix> matrices)
        {
            var allPinIDs = matrices.SelectMany(x => x.PinReference.Keys).Distinct().ToList();
            var allSliderIDs = matrices.SelectMany(x => x.SliderReference.Select(k => (k.Key, k.Value))).ToList(); // convert SliderReference to the required tuple
            SMatrix sysMat = new(allPinIDs , allSliderIDs);

            foreach (SMatrix matrix in matrices)
            {
                var transfers = matrix.GetNonNullValues();
                var nonLinearTransfers = matrix.NonLinearConnections;
                sysMat.SetValues(transfers);
                // also copy the nonlinear functions
                foreach(var key in nonLinearTransfers.Keys)
                {
                    sysMat.NonLinearConnections.Add(key, nonLinearTransfers[key]);
                }
            }
            return sysMat;
        }

        // n is the number of time steps to move forward "steps=3" would return the light propagation after 3 steps.
        public async Task<Dictionary<Guid, Complex>> CalcFieldAtPinsAfterStepsAsync(MathNet.Numerics.LinearAlgebra.Vector<Complex> inputVector, int maxSteps , CancellationTokenSource cancellation)
        {
            if (maxSteps < 1) return new Dictionary<Guid, Complex>();

            // update the SMat using the non linear connections - including those who are not depending on the input vector (the PIN1 etc)
            await RecomputeSMatNonLinearPartsAsync(inputVector, SkipOuterLoopFunctions:false);
            try
            {

                var inputAfterSteps = SMat * inputVector + inputVector;
                for (int i = 1; i < maxSteps; i++)
                {
                    cancellation.Token.ThrowIfCancellationRequested();
                    await Task.Run(async () =>
                    {
                        var oldInputAfterSteps = inputAfterSteps;
                        // recalculating non linear values because the inputVector has changed and could now change the connections like activate a logic gate for example.
                        await RecomputeSMatNonLinearPartsAsync(inputAfterSteps, SkipOuterLoopFunctions: true);
                        // multiplying the adjusted matrix and also adding the initial inputVector again because there is more light incoming
                        inputAfterSteps = SMat * inputAfterSteps + inputVector;
                        if (oldInputAfterSteps.Equals(inputAfterSteps))
                            maxSteps = 0;
                    }, cancellation.Token);
                }

                return ConvertToDictWithGuids(inputAfterSteps);
            }
            catch 
            {
                return new Dictionary<Guid, Complex>();
            }
        }

        private List<object> GetWeightParameters(IEnumerable<Guid> parameterGuids, MathNet.Numerics.LinearAlgebra.Vector<Complex> inputVector)
        {
            List<object> usedParameterValues = new();
            foreach( var paramGuid in parameterGuids)
            {
                // first check if the parameterGuid is in the pin-Dict
                if (PinReference.TryGetValue(paramGuid, out int pinNumber))
                {
                    usedParameterValues.Add(inputVector[pinNumber]);
                }
                // check if parameterGuid is in the slider Dict
                else if (SliderReference.TryGetValue(paramGuid, out double sliderPosition))
                {
                    usedParameterValues.Add(sliderPosition);
                }
            }

            return usedParameterValues;
        }
        private async Task RecomputeSMatNonLinearPartsAsync(MathNet.Numerics.LinearAlgebra.Vector<Complex> inputVector , bool SkipOuterLoopFunctions = true)
        {
            foreach (var connection in NonLinearConnections)
            {
                if (connection.Value.IsInnerLoopFunction == false && SkipOuterLoopFunctions == true)// some functions 
                    continue;
                var indexStart = PinReference[connection.Key.PinIdStart];
                var indexEnd = PinReference[connection.Key.PinIdEnd];
                var weightParameters = GetWeightParameters(connection.Value.UsedParameterGuids, inputVector);
                var calculatedWeight = connection.Value.CalcConnectionWeightAsync(weightParameters);
                SMat[indexEnd, indexStart] = calculatedWeight;
            }
        }

        private Dictionary<Guid, Complex> ConvertToDictWithGuids(MathNet.Numerics.LinearAlgebra.Vector<Complex> lightPropagationVector)
        {
            var GuidsAndLightValues = new Dictionary<Guid, Complex>();
            for (int i = 0; i < lightPropagationVector.Count; i++)
            {
                GuidsAndLightValues.Add(ReversePinReference[i], lightPropagationVector[i]);
            }
            return GuidsAndLightValues;
        }
       
    }
}
