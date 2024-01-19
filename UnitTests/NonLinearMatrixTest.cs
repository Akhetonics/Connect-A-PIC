﻿using CAP_Core.Grid.FormulaReading;
using CAP_Core.Tiles.Grid;
using Chickensoft.GoDotTest;
using MathNet.Numerics;
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
            // initialize Pins
            List<Guid> pins = new() { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

            // defining the comlex value for the input light
            var inputLightStrength = new Complex(1, 0);
            // defining a linear factor defining how much light will be lost in the linear connections
            var linearFactor = 0.9; 

            // defining the intensity and pinPosition of light that we input into the circuit
            Complex[] InputVectorIntensities = new Complex[] { inputLightStrength, new(0, 0), new(0, 0), new(0, 0) };
            MathNet.Numerics.LinearAlgebra.Vector<Complex> inputVector = MathNet.Numerics.LinearAlgebra.Vector<Complex>.Build.Dense(InputVectorIntensities);

            // defining our non-linear test function
            var nonLinearLightFunction = new ConnectionFunction(
                inputVectorValuesAtPinIDs => // the input is the complex value of the input vector at the index of the GUID of the pins of the next parameter
                inputVectorValuesAtPinIDs[0] * new Complex(0.9, 0.32),
                new List<Guid>() { pins[0] } // the Pin-GUIDs are used so that we know at which index of the inputVector we have to get the numbers from. Those Complex numbers will be fed into the connectionfunction-Lambda in that given order as a list.
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

            SMatrix sMatrix = new(pins);
            sMatrix.SetNonLinearConnectionFunctions(nonLinearConnections);
            sMatrix.SetValues(linearConnections); // becau
            var outputLight = sMatrix.GetLightPropagation(inputVector, 10);

            var expectedResult = linearFactor * nonLinearConnections[(pins[0], pins[1])].CalcConnectionWeight(new List<Complex>() { inputLightStrength });
            var tolerance = 1e-12;
            expectedResult.Real.ShouldBe(0.81, tolerance);
            expectedResult.Imaginary.ShouldBe(0.288, tolerance);
            outputLight[pins[3]].ShouldBe(expectedResult);
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
