using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;
using System;
using System.Linq;
using ConnectAPIC.Scenes.Component;
using System.Text;
using System.Numerics;

namespace TransferFunction
{
    public class SMatrix
    {
        public Matrix<Complex> SMat;
        public readonly List<Guid> PinReference;
        private readonly int size;

        public SMatrix(List<Guid> allPinsInGrid)
        {
            if (allPinsInGrid != null && allPinsInGrid.Count > 0)
            {
                this.size = allPinsInGrid.Count;
            }
            else
            {
                this.size = 0;
            }

            this.SMat = Matrix<Complex>.Build.Dense(this.size, this.size);
            this.PinReference = allPinsInGrid;
        }

        public void SetValues(Dictionary<(Guid, Guid), Complex> transfers, bool reset = false)
        {
            if (transfers == null || this.PinReference == null)
            {
                return;
            }

            if (reset)
            {
                this.SMat = Matrix<Complex>.Build.Dense(this.size, this.size);
            }

            foreach (var relation in transfers.Keys)
            {
                if (PinReference.Contains(relation.Item1) && PinReference.Contains(relation.Item2))
                {
                    int row = PinReference.IndexOf(relation.Item1);
                    int col = PinReference.IndexOf(relation.Item2);
                    this.SMat[row, col] = transfers[relation];
                }
            }
        }

        public Dictionary<(Guid, Guid), Complex> GetNonNullValues()
        {
            var transfers = new Dictionary<(Guid, Guid), Complex>();
            for (int i = 0; i < this.size; i++)
            {
                for (int j = 0; j < this.size; j++)
                {
                    if (this.SMat[i, j] == Complex.Zero) continue;
                    transfers[(this.PinReference[j], this.PinReference[i])] = this.SMat[i, j];
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

        // n is the number of timesteps to move forward "steps=3" would return the light propagation after 3 steps.
        public MathNet.Numerics.LinearAlgebra.Vector<Complex> GetLightPropagationAfterSteps(MathNet.Numerics.LinearAlgebra.Vector<Complex> inputVector , int steps)
        {
            if (steps < 1) return null;
            var inputAfterSteps = this.SMat * inputVector;
            for ( int i = 1; i < steps; i++)
            {
                inputAfterSteps = this.SMat * inputVector + inputAfterSteps;
            }
            return inputAfterSteps;
        }

        public override string ToString()
        {
            return ToString(false);
        }
        public string ToString(bool leaveOutImaginary )
        {
            var result = new StringBuilder();

            // Add header with the PinReference IDs for clarity
            result.Append("|\t");
            foreach (var pin in PinReference)
            {
                result.Append(pin.ToString()[..6] + "\t");  // Just showing first 6 chars of GUID for brevity
                if (leaveOutImaginary == false)
                {
                    result.Append('\t');
                }
            }
            result.AppendLine("|");

            // Now, iterate over each row of the matrix
            for (int i = 0; i < size; i++)
            {
                result.Append("|\t");
                for (int j = 0; j < size; j++)
                {
                    var complexValue = SMat[i, j];
                    if (leaveOutImaginary)
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
    }
}
