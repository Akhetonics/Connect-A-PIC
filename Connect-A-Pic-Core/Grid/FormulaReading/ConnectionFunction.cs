using System.Numerics;

namespace CAP_Core.Grid.FormulaReading
{
    public record struct ConnectionFunction(Func<List<Complex>, Complex> CalcConnectionWeight, string ConnectionsFunctionRaw, List<Guid> ParameterPinGuids)
    {
        public static implicit operator (Func<List<Complex>, Complex> connectionWeights, string connectionWeightsRaw, List<Guid> parameters)(ConnectionFunction value)
        {
            return (value.CalcConnectionWeight, value.ConnectionsFunctionRaw, value.ParameterPinGuids);
        }

        public static implicit operator ConnectionFunction((Func<List<Complex>, Complex> CalcConnectionWeight,string ConnectionWeightsRaw, List<Guid> parameters) value)
        {
            return new ConnectionFunction(value.CalcConnectionWeight, value.ConnectionWeightsRaw, value.parameters);
        }
    }
}
