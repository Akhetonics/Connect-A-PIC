using System.Numerics;

namespace CAP_Core.Components.FormulaReading
{
    /// <summary>
    /// The Func CalcConnectionWeights gets a list of object parameters that it inserst into its delegate Func and then it returns a complex value for the given matrix-cell
    /// </summary>
    /// <param name="CalcConnectionWeight"></param>
    /// <param name="ConnectionsFunctionRaw"></param>
    /// <param name="UsedParameterGuids">there are the GUIDS of the Pins or Sliders that are used by the function in the order of the list. Use IDToParameterValueMapper to retrieve the correct object that will be inserted into the Func-Delegate CalcConnectionWeight</param>
    public record struct ConnectionFunction(Func<List<object>, Complex> CalcConnectionWeight, string ConnectionsFunctionRaw, List<Guid> UsedParameterGuids)
    {
        public static implicit operator (Func<List<object>, Complex> connectionWeights, string connectionWeightsRaw,List<Guid> UsedParameterGuids)(ConnectionFunction value)
        {
            return (value.CalcConnectionWeight, value.ConnectionsFunctionRaw, value.UsedParameterGuids);
        }

        public static implicit operator ConnectionFunction((Func<List<object>, Complex> CalcConnectionWeight, string ConnectionWeightsRaw, List<Guid> UsedParameterGuids) value)
        {
            return new ConnectionFunction(value.CalcConnectionWeight, value.ConnectionWeightsRaw, value.UsedParameterGuids);
        }
    }

}
