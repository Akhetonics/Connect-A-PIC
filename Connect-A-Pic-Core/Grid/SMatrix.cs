using System.Text;
using System.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace CAP_Core.Tiles.Grid
{
    public class SMatrix : ICloneable
    {
        public Matrix<Complex> SMat;
        public readonly List<Guid> PinReference;
        private readonly int size;
        public const int MaxToStringPinGuidSize = 6;

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
            PinReference = new List<Guid>();
            PinReference.AddRange(allPinsInGrid);
        }

        public void SetValues(Dictionary<(Guid PinIdStart, Guid PinIdEnd), Complex> transfers, bool reset = false)
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
                if (PinReference.Contains(relation.Item1) && PinReference.Contains(relation.Item2))
                {
                    int col = PinReference.IndexOf(relation.Item1);
                    int row = PinReference.IndexOf(relation.Item2);
                    SMat[row, col] = transfers[relation];
                }
            }
        }

        public void SetValues(Matrix<Complex> sMatrix)
        {
            this.SMat = sMatrix;
        }

        public Dictionary<(Guid PinIdStart, Guid PinIdEnd), Complex> GetNonNullValues()
        {
            var transfers = new Dictionary<(Guid, Guid), Complex>();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (SMat[i, j] == Complex.Zero) continue;
                    transfers[(PinReference[j], PinReference[i])] = SMat[i, j];
                }
            }
            return transfers;
        }

        public static SMatrix CreateSystemSMatrix(List<SMatrix> matrices)
        {
            var portsReference = matrices.SelectMany(x => x.PinReference).Distinct().ToList();
            SMatrix sysMat = new(portsReference);

            foreach (SMatrix matrix in matrices)
            {
                var transfers = matrix.GetNonNullValues();
                sysMat.SetValues(transfers);
            }

            return sysMat;
        }

        /*(1); PINA
        (0)
        (0)

          A  B  C
        A 0  0  0
        B fun(a) 0 0 
        C 0 1 0 

        (0)
        (0)
        (1)
        one could write a wrapper and then override the * function so that the func(a) will be calculated
        */

        // n is the number of time steps to move forward "steps=3" would return the light propagation after 3 steps.
        public Dictionary<Guid, Complex> GetLightPropagation(MathNet.Numerics.LinearAlgebra.Vector<Complex> inputVector, int maxSteps)
        {
            if (maxSteps < 1) return new Dictionary<Guid, Complex>();
            var inputAfterSteps = SMat * inputVector;
            for (int i = 1; i < maxSteps; i++)
            {
                var oldInputAfterSteps = inputAfterSteps;
                inputAfterSteps = SMat * inputAfterSteps + inputVector;
                if (oldInputAfterSteps.Equals(inputAfterSteps)) break;
            }

            return ConvertToDictWithGuids(inputAfterSteps);
        }

        private Dictionary<Guid, Complex> ConvertToDictWithGuids(MathNet.Numerics.LinearAlgebra.Vector<Complex> lightPropagationVector)
        {
            var GuidsAndLightValues = new Dictionary<Guid, Complex>();
            for (int i = 0; i < lightPropagationVector.Count; i++)
            {
                GuidsAndLightValues.Add(PinReference[i], lightPropagationVector[i]);
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
                result.Append($"{PinReference[i].ToString()[..MaxToStringPinGuidSize]}\t");
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
            var clonedSMatrix = new SMatrix(PinReference);
            clonedSMatrix.SetValues(GetNonNullValues());
            return clonedSMatrix;
        }
    }
}
