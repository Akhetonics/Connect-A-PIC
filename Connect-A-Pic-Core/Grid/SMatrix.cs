using System.Text;
using System.Numerics;
using MathNet.Numerics.LinearAlgebra;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Linq;
using CAP_Core.Grid.FormulaReading;

namespace CAP_Core.Tiles.Grid
{
    public class SMatrix : ICloneable
    {
        public Matrix<Complex> SMat; // the SMat works like SMat[PinNROutflow, PinNRInflow] --> so opposite from what one might expect
        public readonly Dictionary<Guid,int> PinReference; // all PinIDs inside of the matrix. the int is the index of the row/column in the SMat.. and also of the inputVector.
        private readonly Dictionary<int, Guid> reversePinReference; // sometimes we want to find the GUID and only have the ID
        private readonly int size;
        public const int MaxToStringPinGuidSize = 6;
        public Dictionary<(Guid PinIdStart, Guid PinIdEnd), ConnectionFunction> NonLinearConnections;

        public SMatrix(List<Guid> allPinsInGrid)
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
            PinReference = new ();
            for(int i = 0; i < size; i++)
            {
                PinReference.Add(allPinsInGrid[i], i );
            }
            reversePinReference = PinReference.ToDictionary(pair => pair.Value, pair => pair.Key);
            NonLinearConnections = new();
        }

        public void SetNonLinearConnectionFunctions(Dictionary<(Guid PinIdStart, Guid PinIdEnd), ConnectionFunction> transfers)
        {
            NonLinearConnections = transfers;
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
                    transfers[ (reversePinReference[iIn],reversePinReference[iOut]) ] = SMat[iOut, iIn];
                }
            }
            return transfers;
        }

        public static SMatrix CreateSystemSMatrix(List<SMatrix> matrices)
        {
            var portsReference = matrices.SelectMany(x => x.PinReference).Distinct().ToList();
            SMatrix sysMat = new(portsReference.Select(p=>p.Key).Distinct().ToList());

            foreach (SMatrix matrix in matrices)
            {
                var transfers = matrix.GetNonNullValues();
                sysMat.SetValues(transfers);
            }

            //todo this also has to update the nonlinearConnections
            return sysMat;
        }

        // n is the number of time steps to move forward "steps=3" would return the light propagation after 3 steps.
        public Dictionary<Guid, Complex> GetLightPropagation(MathNet.Numerics.LinearAlgebra.Vector<Complex> inputVector, int maxSteps)
        {
            if (maxSteps < 1) return new Dictionary<Guid, Complex>();

            // update the SMat using the non linear connections
            RecomputeSMatNonLinearParts(inputVector);
            var inputAfterSteps = SMat * inputVector;
            for (int i = 1; i < maxSteps; i++)
            {
                var oldInputAfterSteps = inputAfterSteps;
                inputAfterSteps = SMat * inputAfterSteps + inputVector;
                if (oldInputAfterSteps.Equals(inputAfterSteps)) break;
            }

            return ConvertToDictWithGuids(inputAfterSteps);
        }

        private List<Complex> CalculateWeightParameters(IEnumerable<Guid> parameterPinGuids, MathNet.Numerics.LinearAlgebra.Vector<Complex> inputVector)
        {
            return parameterPinGuids.Select(p => PinReference[p])
                                    .Select(id => inputVector[id])
                                    .ToList();
        }
        private void RecomputeSMatNonLinearParts(MathNet.Numerics.LinearAlgebra.Vector<Complex> inputVector)
        {
            foreach (var connection in NonLinearConnections)
            {
                var indexStart = PinReference[connection.Key.PinIdStart];
                var indexEnd = PinReference[connection.Key.PinIdEnd];
                var weightParameters = CalculateWeightParameters(connection.Value.ParameterPinGuids, inputVector);
                var calculatedWeight = connection.Value.CalcConnectionWeight(weightParameters);
                SMat[indexEnd, indexStart] = calculatedWeight;
            }
        }

        private Dictionary<Guid, Complex> ConvertToDictWithGuids(MathNet.Numerics.LinearAlgebra.Vector<Complex> lightPropagationVector)
        {
            var GuidsAndLightValues = new Dictionary<Guid, Complex>();
            for (int i = 0; i < lightPropagationVector.Count; i++)
            {
                GuidsAndLightValues.Add(reversePinReference[i], lightPropagationVector[i]);
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
                result.Append($"{reversePinReference[i].ToString()[..MaxToStringPinGuidSize]}\t");
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
            var clonedSMatrix = new SMatrix(PinReference.Select(p=>p.Key).ToList());
            clonedSMatrix.SetValues(GetNonNullValues());
            return clonedSMatrix;
        }
    }
}
