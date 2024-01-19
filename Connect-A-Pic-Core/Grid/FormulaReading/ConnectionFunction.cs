using System.Numerics;

namespace CAP_Core.Grid.FormulaReading
{
    public record struct ConnectionFunction(Func<List<Complex>, Complex> CalcConnectionWeight, List<Guid> ParameterPinGuids)
    {
        public static implicit operator (Func<List<Complex>, Complex> connectionWeights, List<Guid> parameters)(ConnectionFunction value)
        {
            return (value.CalcConnectionWeight, value.ParameterPinGuids);
        }

        public static implicit operator ConnectionFunction((Func<List<Complex>, Complex> CalcConnectionWeight, List<Guid> parameters) value)
        {
            return new ConnectionFunction(value.CalcConnectionWeight, value.parameters);
        }
    }
}
