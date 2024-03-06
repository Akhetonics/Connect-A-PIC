using CAP_Core.LightCalculation;
using System.Numerics;

namespace CAP_Core.ExternalPorts
{
    public static class UsedInputConverter
    {
        public static MathNet.Numerics.LinearAlgebra.Vector<Complex> ToVectorOfFields(List<UsedInput> usedInputs, SMatrix SystemSMatrix)
        {
            var inputVector = MathNet.Numerics.LinearAlgebra.Vector<Complex>.Build.Dense(SystemSMatrix.SMat.RowCount);
            foreach (var usedInput in usedInputs)
            {
                var rowNr = SystemSMatrix.PinReference.Single(p=>p.Key == usedInput.AttachedComponentPinId).Value;
                inputVector[rowNr] = usedInput.Input.InFlowField;
            }
            return inputVector;
        }
    }
}