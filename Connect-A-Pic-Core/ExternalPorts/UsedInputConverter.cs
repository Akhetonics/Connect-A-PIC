using CAP_Core.LightCalculation;
using System.Collections.Concurrent;
using System.Numerics;

namespace CAP_Core.ExternalPorts
{
    public static class UsedInputConverter
    {
        public static MathNet.Numerics.LinearAlgebra.Vector<Complex> ToVectorOfFields(ConcurrentBag<UsedInput> usedInputs, SMatrix SystemSMatrix)
        {
            var inputVector = MathNet.Numerics.LinearAlgebra.Vector<Complex>.Build.Dense(SystemSMatrix.SMat.RowCount);
            foreach (var usedInput in usedInputs)
            {
                if (SystemSMatrix.PinReference.TryGetValue(usedInput.AttachedComponentPinId, out int rowNr))
                {
                    inputVector[rowNr] = usedInput.Input.InFlowField;
                };
            }
            return inputVector;
        }
    }
}
