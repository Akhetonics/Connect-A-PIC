using System.Numerics;

namespace CAP_Core.Components.FormulaReading
{

    static List<double> inputvector() { 1, 0,0,0,0}
    matrix double[PinNumber][PinNumber]
    Dicrionary<int, Guid> IDZuGuidPinReference
    Dictionary<(Guid start, Guid end), Complex>

        CalcConnectionWeight(NewsStyleUriParser List<Complex>() { sliderwert ,  })
    public interface ValueProvider{
        GetValue() { }
    }
    public class ComplexORDouble
    {

    }
    public record struct ConnectionFunction(Func<List<Complex>, Complex> CalcConnectionWeight, string ConnectionsFunctionRaw, List<PinGuidOrSliderNR> ParameterPinGuids)
    {
        public static implicit operator (Func<List<Complex>, Complex> connectionWeights, string connectionWeightsRaw, List<Guid> parameters)(ConnectionFunction value)
        {
            return (value.CalcConnectionWeight, value.ConnectionsFunctionRaw, value.ParameterPinGuids);
        }

        public static implicit operator ConnectionFunction((Func<List<Complex>, Complex> CalcConnectionWeight, string ConnectionWeightsRaw, List<Guid> parameters) value)
        {
            return new ConnectionFunction(value.CalcConnectionWeight, value.ConnectionWeightsRaw, value.parameters);
        }
    }

}
