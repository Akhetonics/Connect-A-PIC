using System.Numerics;
using CAP_Core.LightFlow;

namespace CAP_Core.ExternalPorts
{
    public static class UsedStandardInputConverter
    {
        public static MathNet.Numerics.LinearAlgebra.Vector<Complex> ToVector(List<UsedStandardInput> usedInputs, SMatrix SystemSMatrix)
        {
            var inputVector = MathNet.Numerics.LinearAlgebra.Vector<Complex>.Build.Dense(SystemSMatrix.SMat.RowCount);
            foreach (var inputData in usedInputs)
            {
                var rowNr = SystemSMatrix.PinReference.IndexOf(inputData.AttachedComponentPinId);
                inputVector[rowNr] = inputData.Input.LightInflow;
            }
            return inputVector;
        }
    }
}