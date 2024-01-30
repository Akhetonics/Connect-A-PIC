using System.Text;
using System.Numerics;
using MathNet.Numerics.LinearAlgebra;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Linq;
using CAP_Core.Components.FormulaReading;

namespace CAP_Core.Components
{
    public class SMatrix : ICloneable
    {
        public Matrix<Complex> SMat; // the SMat works like SMat[PinNROutflow, PinNRInflow] --> so opposite from what one might expect
        public readonly Dictionary<Guid, int> PinReference; // all PinIDs inside of the matrix. the int is the index of the row/column in the SMat.. and also of the inputVector.
        public Dictionary<Guid, double> SliderReference { get; internal set; }
        private readonly Dictionary<int, Guid> ReversePinReference; // sometimes we want to find the GUID and only have the ID
        private readonly int size;
        public const int MaxToStringPinGuidSize = 6;
        public Dictionary<(Guid PinIdStart, Guid PinIdEnd), ConnectionFunction> NonLinearConnections { get; set; }

        public SMatrix(List<Guid> allPinsInGrid, List<Guid> AllSliders)
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
                SliderReference.Add(slider, 0);
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
            var allSliderIDs = matrices.SelectMany(x => x.SliderReference.Keys).Distinct().ToList();
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
        public Dictionary<Guid, Complex> GetLightPropagation(MathNet.Numerics.LinearAlgebra.Vector<Complex> inputVector, int maxSteps)
        {
            if (maxSteps < 1) return new Dictionary<Guid, Complex>();

            // update the SMat using the non linear connections
            RecomputeSMatNonLinearParts(inputVector, SkipOuterloopFunctions:false);

            var inputAfterSteps = SMat * inputVector;
            for (int i = 1; i < maxSteps; i++)
            {
                var oldInputAfterSteps = inputAfterSteps;
                // recalculating non linear values because the inputvector has changed and could now change the connections like activate a logic gate for example.
                RecomputeSMatNonLinearParts(inputAfterSteps, SkipOuterloopFunctions: true);
                // multiplying the adjusted matrix and also adding the initial inputVector again because there is more light incoming
                inputAfterSteps = SMat * inputAfterSteps + inputVector;
                if (oldInputAfterSteps.Equals(inputAfterSteps)) break;
            }

            return ConvertToDictWithGuids(inputAfterSteps);
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
        private void RecomputeSMatNonLinearParts(MathNet.Numerics.LinearAlgebra.Vector<Complex> inputVector , bool SkipOuterloopFunctions = true)
        {
            foreach (var connection in NonLinearConnections)
            {
                if (connection.Value.IsInnerLoopFunction == false && SkipOuterloopFunctions == true)// some functions 
                    continue;
                var indexStart = PinReference[connection.Key.PinIdStart];
                var indexEnd = PinReference[connection.Key.PinIdEnd];
                var weightParameters = GetWeightParameters(connection.Value.UsedParameterGuids, inputVector);
                var calculatedWeight = connection.Value.CalcConnectionWeight(weightParameters);
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

        public override string ToString()
        {
            return ToString(false);
        }
        public string ToString(bool leaveOutImaginary)
        {
            var result = new StringBuilder();

            // Add header with the PinReference IDs for clarity
            result.Append("|\t");
            foreach (var pinGuid in PinReference)
            {
                result.Append(pinGuid.ToString()[..MaxToStringPinGuidSize] + "\t");  // Just showing first 6 chars of GUID for brevity
            }
            result.AppendLine("|");

            // Now, iterate over each row of the matrix
            for (int i = 0; i < size; i++)
            {
                result.Append($"{ReversePinReference[i].ToString()[..MaxToStringPinGuidSize]}\t");
                for (int j = 0; j < size; j++)
                {
                    var complexValue = SMat[i, j];
                    if (leaveOutImaginary || complexValue.Imaginary == 0)
                    {
                        result.Append($"{complexValue.Real:F2}\t");
                    }
                    else
                    {
                        result.Append($"{complexValue.Real:F2}+{complexValue.Imaginary:F1}i\t");
                    }
                }
                result.AppendLine("|");
            }

            return result.ToString();
        }

        public object Clone()
        {
            var allPinIDs = PinReference.Select(p => p.Key).ToList();
            var allSliderIDs = SliderReference.Select(s=>s.Key).ToList();
            var clonedSMatrix = new SMatrix(allPinIDs, allSliderIDs);
            clonedSMatrix.SetValues(GetNonNullValues());
            return clonedSMatrix;
        }
    }
}
