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
            public static void RegisterComplexFunctions(Expression e)
            {
                e.EvaluateFunction += delegate (string name, FunctionArgs args)
                {
                    if (name.ToLower() == nameof(ComplexMath.Add).ToLower())
                    {
                        var a = (Complex)args.Parameters[0].Evaluate();
                        var b = (Complex)args.Parameters[1].Evaluate();
                        args.Result = ComplexMath.Add(a, b);
                    }
                    if (name.ToLower() == nameof(ComplexMath.Sub).ToLower())
                    {
                        var a = (Complex)args.Parameters[0].Evaluate();
                        var b = (Complex)args.Parameters[1].Evaluate();
                        args.Result = ComplexMath.Sub(a, b);
                    }
                    if (name.ToLower() == nameof(ComplexMath.Div).ToLower())
                    {
                        var a = (Complex)args.Parameters[0].Evaluate();
                        var b = (Complex)args.Parameters[1].Evaluate();
                        args.Result = ComplexMath.Div(a, b);
                    }
                    if (name.ToLower() == nameof(ComplexMath.Mul).ToLower())
                    {
                        var a = (Complex)args.Parameters[0].Evaluate();
                        var b = (Complex)args.Parameters[1].Evaluate();
                        args.Result = ComplexMath.Mul(a, b);
                    }
                };
            }
        }
    }
}