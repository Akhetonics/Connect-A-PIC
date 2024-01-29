using CAP_Core.Components;
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

namespace UnitTests.Components.FormulaReading
{
    public class MathExpressionReaderTests
    {

        [Fact]
        public void TestMathExpression()
        {
            string expression = "Add(Add(PIN1,PIN1),PIN2)";
            var valueForPin1 = new Complex(2, 4);
            var valueForPin2 = new Complex(4, 2.2);
            var parametersFound = new List<string> {"PIN1", "PIN2" };
            var parameterNumbers = parametersFound.Select(p => MathExpressionReader.ExtractIdentifierNumber(p)).ToList();
            List<Pin> pins = new List<Pin>() {
                new("",parameterNumbers[0],MatterType.Light,RectSide.Right),
                new("",parameterNumbers[1],MatterType.Light,RectSide.Left),
            };
            var computedDelegate = MathExpressionReader.ConvertToDelegate(expression, pins, new());
            var formulaResult = computedDelegate.Value.CalcConnectionWeight(new List<object>() { valueForPin1, valueForPin2 });

            formulaResult.ShouldBe(new Complex(8, 10.2));
        }
        [Fact]
        public void TestMixedMathExpressions()
        {
            string expression = "Add(ToComplexFromPolar( 2-1.5 , 3.0774785178021489) , ToComplex(1,1.2))";
            var computedDelegate = MathExpressionReader.ConvertToDelegate(expression, new List<Pin>(), new());
            var formulaResult = computedDelegate.Value.CalcConnectionWeight(new List<object>());
            formulaResult.ShouldBe(Complex.FromPolarCoordinates(0.5, 3.0774785178021489) + new Complex(1, 1.2));
        }

        [Fact]
        public void ExtractPinNumber_ValidPinIdentifier_ReturnsCorrectNumber()
        {
            // Arrange
            string pinIdentifier = "PIN123";

            // Act
            int result = MathExpressionReader.ExtractIdentifierNumber(pinIdentifier);

            // Assert
            Assert.Equal(123, result);
        }

        [Fact]
        public void ExtractPinNumber_InvalidPinIdentifier_ThrowsInvalidParameterException()
        {
            // Arrange
            string pinIdentifier = "Invalid 123";

            // Act & Assert
            Assert.Throws<InvalidParameterException>(() => MathExpressionReader.ExtractIdentifierNumber(pinIdentifier));
        }

        [Fact]
        public void FixPlaceHolderNames_LowerCasePlaceHolders_ConvertsToUpper()
        {
            // Arrange
            string expression = "pin1 + pin2";

            // Act
            string result = MathExpressionReader.MakePlaceHoldersUpperCase(expression);

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
