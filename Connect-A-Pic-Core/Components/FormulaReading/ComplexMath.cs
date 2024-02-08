using CAP_Core.Components.ComponentHelpers;
using NCalc;
using System.Numerics;

namespace CAP_Core.Grid.FormulaReading
{
    public static partial class MathExpressionReader
    {
        public class ComplexMath
        {
            public static Complex Add(Complex a, Complex b) => a + b;
            public static Complex Sub(Complex a, Complex b) => a - b;
            public static Complex Div(Complex a, Complex b) => a / b;
            public static Complex Mul(Complex a, Complex b) => a * b;
            public static Complex ToComplex(double a, double b) => new Complex(a,b);
            public static Complex ToComplexFromPolar(double a, double b) => Complex.FromPolarCoordinates(a, b);
            public static Complex PhaseShiftFromWGLength(double a, double b) => PhaseShiftCalculator.CalculateWave(a , b);
            public static Expression CreateExpressionWithCustomFunctions(string expressionString)
            {
                var e = new Expression(expressionString);
                e.EvaluateFunction += delegate (string name, FunctionArgs args)
                {
                    if (name.ToLower() == nameof(ComplexMath.Add).ToLower())
                    {
                        var a = ConvertToComplex(args.Parameters[0].Evaluate());
                        var b = ConvertToComplex(args.Parameters[1].Evaluate());
                        args.Result = ComplexMath.Add(a, b);
                    }
                    if (name.ToLower() == nameof(ComplexMath.Sub).ToLower())
                    {
                        var a = ConvertToComplex(args.Parameters[0].Evaluate());
                        var b = ConvertToComplex(args.Parameters[1].Evaluate());
                        args.Result = ComplexMath.Sub(a, b);
                    }
                    if (name.ToLower() == nameof(ComplexMath.Div).ToLower())
                    {
                        var a = ConvertToComplex(args.Parameters[0].Evaluate());
                        var b = ConvertToComplex(args.Parameters[1].Evaluate());
                        args.Result = ComplexMath.Div(a, b);
                    }
                    if (name.ToLower() == nameof(ComplexMath.Mul).ToLower())
                    {
                        var a = ConvertToComplex(args.Parameters[0].Evaluate());
                        var b = ConvertToComplex(args.Parameters[1].Evaluate());
                        args.Result = ComplexMath.Mul(a, b);
                    }
                    if (name.ToLower() == nameof(ComplexMath.ToComplex).ToLower())
                    {
                        var real = ConvertToComplex(args.Parameters[0].Evaluate());
                        var imaginary = ConvertToComplex(args.Parameters[1].Evaluate());
                        args.Result = ComplexMath.ToComplex(real.Real, imaginary.Real); // if the value was a double, then the phase is 0 so real=magnitude= doubleValue
                    }
                    if (name.ToLower() == nameof(ComplexMath.ToComplexFromPolar).ToLower())
                    {
                        var magnitude = ConvertToComplex(args.Parameters[0].Evaluate());
                        var phase = ConvertToComplex(args.Parameters[1].Evaluate());
                        args.Result = ComplexMath.ToComplexFromPolar(magnitude.Real, phase.Real);
                    }
                    if (name.ToLower() == nameof(ComplexMath.PhaseShiftFromWGLength).ToLower())
                    {
                        var wireLength = ConvertToComplex(args.Parameters[0].Evaluate());
                        var lifhtWaveLength = ConvertToComplex(args.Parameters[1].Evaluate());
                        args.Result = ComplexMath.ToComplexFromPolar(wireLength.Real, lifhtWaveLength.Real);
                    }
                };
                return e;
            }

            public static Complex ConvertToComplex(object obj)
            {
                if (obj is Complex complexVal)
                {
                    return complexVal;
                }
                else if (obj is double || obj is float || obj is int)
                {
                    double doubleVal = Convert.ToDouble(obj);
                    return new Complex(doubleVal, 0);
                }
                else
                {
                    throw new InvalidOperationException("Unsupported type for conversion to Complex.");
                }
            }
        }
    }
}