﻿using CAP_Core.Components;
using CAP_Core.Grid.FormulaReading;
using CAP_Core.Tiles;
using MathNet.Numerics;
using NCalc;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    public class MathExpressionReaderTests
    {

        [Fact]
        public void TestMathExpression()
        {
            string expression =  "Add(Add(PIN1,PIN1),PIN2)";
            var valueForPin1 = new Complex(2,4);
            var valueForPin2 = new Complex(4,2.2);
            var parametersFound = MathExpressionReader.FindParametersInExpression(expression);
            var parameterNumbers = parametersFound.Select(p => MathExpressionReader.ExtractPinNumber(p.Name)).ToList();
            List<Pin> pins = new List<Pin>() {
                new("",parameterNumbers[0],MatterType.Light,RectSide.Right),
                new("",parameterNumbers[1],MatterType.Light,RectSide.Left),
            };
            var computedDelegate = MathExpressionReader.ConvertToDelegate(expression, pins);
            var formulaResult = computedDelegate.Value.CalcConnectionWeight(new List<Complex>() { valueForPin1, valueForPin2 });

            formulaResult.ShouldBe(new Complex(8,10.2));
        }

        [Fact]
        public void ExtractPinNumber_ValidPinIdentifier_ReturnsCorrectNumber()
        {
            // Arrange
            string pinIdentifier = "PIN123";

            // Act
            int result = MathExpressionReader.ExtractPinNumber(pinIdentifier);

            // Assert
            Assert.Equal(123, result);
        }

        [Fact]
        public void ExtractPinNumber_InvalidPinIdentifier_ThrowsInvalidParameterException()
        {
            // Arrange
            string pinIdentifier = "Invalid123";

            // Act & Assert
            Assert.Throws<InvalidParameterException>(() => MathExpressionReader.ExtractPinNumber(pinIdentifier));
        }

        [Fact]
        public void FixPlaceHolderNames_LowerCasePlaceHolders_ConvertsToUpper()
        {
            // Arrange
            string expression = "pin1 + pin2";

            // Act
            string result = MathExpressionReader.FixPlaceHolderNames(expression);

            // Assert
            Assert.Equal("PIN1 + PIN2", result);
        }
        [Fact]
        public void ComplexMath_Add_ValidComplexNumbers_ReturnsCorrectSum()
        {
            // Arrange
            var a = new Complex(1, 2);
            var b = new Complex(3, 4);

            // Act
            var result = MathExpressionReader.ComplexMath.Add(a, b);

            // Assert
            Assert.Equal(new Complex(4, 6), result);
        }
    }
}
