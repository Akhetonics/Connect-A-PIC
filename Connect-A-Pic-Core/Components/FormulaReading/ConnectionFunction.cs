﻿using System.Numerics;

namespace CAP_Core.Components.FormulaReading
{
    /// <summary>
    /// The Func CalcConnectionWeights gets a list of object parameters that it inserts into its delegate Func and then it returns a complex value for the given matrix-cell
    /// </summary>
    /// <param name="CalcConnectionWeight"></param>
    /// <param name="ConnectionsFunctionRaw"></param>
    /// <param name="UsedParameterGuids">there are the GUIDS of the Pins or Sliders that are used by the function in the order of the list. Use IDToParameterValueMapper to retrieve the correct object that will be inserted into the Func-Delegate CalcConnectionWeight</param>
    public record struct ConnectionFunction(Func<List<object>, Complex> CalcConnectionWeightAsync, string ConnectionsFunctionRaw, List<Guid> UsedParameterGuids, bool IsInnerLoopFunction)
    {
        public static implicit operator (Func<List<object>, Complex> connectionWeights, string connectionWeightsRaw,List<Guid> UsedParameterGuids, bool IsInnerLoopFunction)(ConnectionFunction value)
        {
            return (value.CalcConnectionWeightAsync, value.ConnectionsFunctionRaw, value.UsedParameterGuids , value.IsInnerLoopFunction);
        }

        public static implicit operator ConnectionFunction((Func<List<object>, Complex> CalcConnectionWeight, string ConnectionWeightsRaw, List<Guid> UsedParameterGuids, bool IsInnerLoopFunction) value)
        {
            return new ConnectionFunction(value.CalcConnectionWeight, value.ConnectionWeightsRaw, value.UsedParameterGuids , value.IsInnerLoopFunction);
        }
    }

}
