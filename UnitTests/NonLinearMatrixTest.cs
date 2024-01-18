using CAP_Core.Tiles.Grid;
using Chickensoft.GoDotTest;
using MathNet.Numerics.LinearAlgebra;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    public class NonLinearMatrixTest
    {
        [Fact]
        public void NonLinearMatrixCalculationTest()
        {
            List<Guid> pins = new() { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            var movementFunction = MathExpressionReader.ConvertToDelegate("PIN0 * 0.9", new Dictionary<string, Guid>() { { "Pin0", pins[0] } }); // the key is small letter to test if it fixes it
            var inputLightStrength = new Complex(1, 0);
            Complex[] inputValues = new Complex[] { inputLightStrength, new(0, 0), new(0, 0), new(0, 0) };

            Dictionary<(Guid inFlowPin, Guid outFlowPin), ConnectionFunction> nonLinearConnections = new()
            {
                { (pins[0],pins[1]) , new( x=> x[0] * Complex.FromPolarCoordinates(0.9,Math.PI) , new List<Guid>(){ pins[0] } )},
                { (pins[1],pins[2]) , new ( x=> new Complex(Math.Sin(x[0].Real),Math.Cos(x[0].Imaginary)) , new List<Guid>(){pins[1] } ) },
            };

            SMatrix sMatrix = new(pins);
            MathNet.Numerics.LinearAlgebra.Vector<Complex> inputVector = MathNet.Numerics.LinearAlgebra.Vector<Complex>.Build.Dense(inputValues);
            sMatrix.SetNonLinearConnectionFunctions(nonLinearConnections);
            var outputLight = sMatrix.GetLightPropagation(inputVector, 10);

            var exptectedResult = nonLinearConnections[(pins[0], pins[1])].CalcConnectionWeight(new List<Complex>() { inputLightStrength });

            outputLight[pins[3]].ShouldBe(exptectedResult);
        }
        [Fact]
        public void FindParametersTest()
        {
            var paramName = MathExpressionReader.PinParameterIdentifier + "0";
            var parameters = MathExpressionReader.FindParametersInExpression(paramName + " * 0.9");

            parameters.Single().Name.ShouldBe(paramName);
        }
        [Fact] 
        public void ParseLambdaTests()
        {
            string expression = "PIN1 + PIN2";
            var parameters = MathExpressionReader.FindParametersInExpression(expression);
            var lambda = DynamicExpressionParser.ParseLambda(parameters.ToArray(), null, expression);
            var result = lambda.Compile().DynamicInvoke(12, 13);
            result.ShouldBe(25);
        }
        [Fact]
        public void ConvertToDelegate_ShouldCreateValidConnectionFunction()
        {
            // Arrange
            string expression = "PIN1 + PIN2";
            var pinPlaceholdersToGuids = new Dictionary<string, Guid>
            {
                { "PIN1", Guid.NewGuid() },
                { "PIN2", Guid.NewGuid() }
            };

            // Act
            var connectionFunction = MathExpressionReader.ConvertToDelegate(expression, pinPlaceholdersToGuids);

            // Assert
            connectionFunction.ParameterPinGuids.ShouldBe(pinPlaceholdersToGuids.Select(p => p.Value).ToList());

            // Create sample complex parameters
            var complexParameters = new List<Complex>
            {
                new (1.0, 2.0), // Complex number for Pin1
                new (3.0, 4.0)  // Complex number for Pin2
            };

            // Invoke the connection function
            var result = connectionFunction.CalcConnectionWeight(complexParameters);

            // Verify the result
            var expected = new Complex(4.0, 6.0); // Expected result of (1+3) + (2+4)i
            result.ShouldBe(expected);
        }
    }
}
