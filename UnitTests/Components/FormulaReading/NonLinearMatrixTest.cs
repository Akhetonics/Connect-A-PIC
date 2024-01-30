using CAP_Core.Components;
using CAP_Core.Components.FormulaReading;
using CAP_Core.Grid.FormulaReading;
using MathNet.Numerics.LinearAlgebra;
using Shouldly;
using System.Linq.Dynamic.Core;
using System.Numerics;

namespace UnitTests.Components.FormulaReading
{
    public class NonLinearMatrixTest
    {
        [Fact]
        public void NonLinearMatrixCalculationTest()
        {
            // initialize Pins
            List<Guid> pins = new() { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

            // defining the complex value for the input light
            var inputLightStrength = new Complex(1, 0);
            // defining a linear factor defining how much light will be lost in the linear connections
            var linearFactor = 0.9;

            // defining the intensity and pinPosition of light that we input into the circuit
            Complex[] InputVectorIntensities = new Complex[] { inputLightStrength, new(0, 0), new(0, 0), new(0, 0) };
            MathNet.Numerics.LinearAlgebra.Vector<Complex> inputVector = MathNet.Numerics.LinearAlgebra.Vector<Complex>.Build.Dense(InputVectorIntensities);

            // defining our non-linear test function
            var nonLinearLightFunction = new ConnectionFunction(
                inputVectorValuesAtPinIDs => // the input is the complex value of the input vector at the index of the GUID of the pins of the next parameter
                (Complex)inputVectorValuesAtPinIDs[0] * new Complex(0.9, 0.32),
                "", // as we don't parse the rawfunction, we can leave it empty
                new List<Guid>() { pins[0] }, // the Pin-GUIDs are used so that we know at which index of the inputVector we have to get the numbers from. Those Complex numbers will be fed into the connection-Function-Lambda in that given order as a list.
                true
            );

            // defining the dictionary that contains all non-linear functions
            Dictionary<(Guid inFlowPin, Guid outFlowPin), ConnectionFunction> nonLinearConnections = new()
            {
                {
                    (pins[0],pins[1]) , // the non-linear connection starts at pin0 and goes to pin1
                    nonLinearLightFunction
                }
            };

            // we also define all linear functions with our linearFactor so that we lose 10% of light throughput
            Dictionary<(Guid inFlowPin, Guid outFlowPin), Complex> linearConnections = new()
            {
                { (pins[1] , pins[2]) , 1},
                { (pins[2] , pins[3]) , linearFactor}
            };

            SMatrix sMatrix = new(pins, new());
            sMatrix.NonLinearConnections = nonLinearConnections;
            sMatrix.SetValues(linearConnections);
            var outputLight = sMatrix.GetLightPropagation(inputVector, 10);

            var expectedResult = linearFactor * nonLinearConnections[(pins[0], pins[1])].CalcConnectionWeight(new List<object>() { inputLightStrength });
            var tolerance = 1e-12;
            expectedResult.Real.ShouldBe(0.81, tolerance);
            expectedResult.Imaginary.ShouldBe(0.288, tolerance);
            outputLight[pins[3]].ShouldBe(expectedResult);
        }

        [Fact]
        public void ConvertToDelegate_ShouldCreateValidConnectionFunction()
        {
            // Arrange
            string expression = "Add(PIN1,PIN2)";
            string wrongExpression = "PIN1 + PIN2"; // the plus parameter does not work with complex values
            var usedPins = new List<Pin>()
            {
               new Pin("",1,CAP_Core.Tiles.RectSide.Left),
               new Pin("",2,CAP_Core.Tiles.RectSide.Right)
            };
            var usedPinInFlowGuids = usedPins.Select(p => p.IDInFlow).ToList();
            // Act
            var connectionFunction = (ConnectionFunction)MathExpressionReader.ConvertToDelegate(expression, usedPins, new());
            var wrongFunction = (ConnectionFunction)MathExpressionReader.ConvertToDelegate(wrongExpression, usedPins,new());

            // Assert
            connectionFunction.UsedParameterGuids.ShouldBe(usedPinInFlowGuids);

            // Create sample complex parameters
            var complexParameters = new List<object>
            {
                new Complex (1.0, 2.0), // Complex number for Pin1
                new Complex(3.0, 4.0)  // Complex number for Pin2
            };

            // Invoke the connection function
            var result = connectionFunction.CalcConnectionWeight(complexParameters);
            try
            {
                var wrongResult = wrongFunction.CalcConnectionWeight(complexParameters);
            }
            catch (InvalidOperationException ex)
            {
                ex.ShouldNotBeNull();
            }


            // Verify the result
            var expected = new Complex(4.0, 6.0); // Expected result of (1+3) + (2+4)i
            result.ShouldBe(expected);
        }
    }
}
