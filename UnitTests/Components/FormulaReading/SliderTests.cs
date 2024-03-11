using CAP_Core;
using CAP_Core.Components;
using CAP_Core.Components.ComponentHelpers;
using CAP_Core.Components.FormulaReading;
using CAP_Core.ExternalPorts;
using CAP_Core.Grid.FormulaReading;
using CAP_DataAccess.Components.ComponentDraftMapper;
using CAP_DataAccess.Components.ComponentDraftMapper.DTOs;
using Shouldly;
using System.Numerics;

namespace UnitTests.Components.FormulaReading
{
    public class SliderTests
    {
        const string formula1 = "ToComplexFromPolar( Sqrt(1.0 -SLIDER0), 1.2161003820350373)";
        const string formula2 = "PhaseShiftFromWGLength(Slider0 * 2000 + 125000 * 2 * 3.1415926535897931, 1550)";
        [Fact]
        public async void SliderCalculationTest()
        {
            // Arrange
            var component = TestComponentFactory.CreateComponent(TestComponentFactory.StraightWGJson);
            component.AddSlider(0, new Slider(Guid.NewGuid(), 0, 0.5, 1, 0));
            var sliderValue1 = 0.5;
            var sliderValue2 = LaserType.Red.WaveLengthInNm / 1000 / 2;
            Complex outputPinLightValFormula1 = await CalculateFormulaUsingComponent(component, sliderValue1, formula1);
            Complex outputPinLightValFormula2 = await CalculateFormulaUsingComponent(component, sliderValue2, formula2);

            // Assert
            var expectedFieldValue1 = Complex.FromPolarCoordinates(Math.Sqrt(1-sliderValue1), 1.2161003820350373);
            var expectedFieldValue2 = PhaseShiftCalculator.CalculateWave(sliderValue2 * 2000 + 125000 * 2 * Math.PI , LaserType.Red.WaveLengthInNm);
            Assert.Equal(expectedFieldValue1, outputPinLightValFormula1);
            Assert.Equal(expectedFieldValue2, outputPinLightValFormula2);
        }

        private async Task<Complex> CalculateFormulaUsingComponent(Component component , double sliderValue, string formula)
        {
            SetUpNonLinearConnection(component, formula);
            var outFlowID = component.Parts[0, 0].GetPinAt(CAP_Core.Tiles.RectSide.Right).IDOutFlow;
            var inputVector = MathNet.Numerics.LinearAlgebra.Vector<Complex>.Build.Dense(new Complex[] { 1, 0, 0, 0 });
            component.GetSlider(0).Value = sliderValue;

            // Act
            var outputVector = await component.WaveLengthToSMatrixMap[LaserType.Red.WaveLengthInNm].CalcFieldAtPinsAfterStepsAsync(inputVector, 1000, new());
            var outputPinLightVal = outputVector[outFlowID];
            return outputPinLightVal;
        }

        private void SetUpNonLinearConnection(Component component, string formula)
        {
            var mainConnection = (component.Parts[0,0].GetPinAt(CAP_Core.Tiles.RectSide.Left).IDInFlow, component.Parts[0,0].GetPinAt(CAP_Core.Tiles.RectSide.Right).IDOutFlow);
            var nonLinConnections = component.WaveLengthToSMatrixMap[LaserType.Red.WaveLengthInNm].NonLinearConnections;

            var allSliders = component.GetAllSliders();
            var function = (ConnectionFunction)MathExpressionReader.ConvertToDelegate(formula, component.GetAllPins(), allSliders);
            if (nonLinConnections.ContainsKey(mainConnection))
            {
                nonLinConnections[mainConnection] = function;
            } else
            {
                nonLinConnections.Add(mainConnection, function);
            }
        }
    }
}
