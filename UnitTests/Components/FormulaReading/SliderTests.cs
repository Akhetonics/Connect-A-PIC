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
        const string formula1 = "ToComplexFromPolar(Sub(1.0, SLIDER0), 1.2161003820350373)";
        const string formula2 = "PhaseShiftFromWGLength(Slider0 * 2000 + 125000 * 2 * 3.1415926535897931, 1550)";
        [Fact]
        public async void SliderCalculationTest()
        {
            // Arrange
            var converter = new ComponentDraftConverter(new Logger());
            var reader = new ComponentDraftFileReader(new DummyDataAccessor(TestComponentFactory.StraightWGJson));
            var draftOrError = reader.TryReadJson("");

            Assert.True(string.IsNullOrEmpty(draftOrError.error), $"Error reading Json: {draftOrError.error}");
            Assert.NotNull(draftOrError.draft);

            var component = converter.ToComponentModels(new List<ComponentDraft> { draftOrError.draft }).First();
            var sliderValue1 = 0.5;
            var sliderValue2 = LaserType.Red.WaveLengthInNm / 1000 / 2;
            Complex outputPinLightValFormula1 = await CalculateFormulaUsingComponent(component, sliderValue1, formula1);
            Complex outputPinLightValFormula2 = await CalculateFormulaUsingComponent(component, sliderValue2, formula2);

            // Assert
            var expectedValue1 = Complex.FromPolarCoordinates(sliderValue1, 1.2161003820350373);
            var expectedValue2 = PhaseShiftCalculator.CalculateWave(2 * Math.PI * 125000 + 2000 * sliderValue2, LaserType.Red.WaveLengthInNm);
            Assert.Equal(expectedValue1, outputPinLightValFormula1);
            Assert.Equal(expectedValue2, outputPinLightValFormula2);
        }

        private async Task<Complex> CalculateFormulaUsingComponent(Component component , double sliderValue, string formula)
        {
            SetUpNonLinearConnection(component, formula);
            var outFlowID = component.Parts[0, 0].GetPinAt(CAP_Core.Tiles.RectSide.Right).IDOutFlow;
            var inputVector = MathNet.Numerics.LinearAlgebra.Vector<Complex>.Build.Dense(new Complex[] { 1, 0, 0, 0 });
            component.GetSlider(0).Value = sliderValue;

            // Act
            var outputVector = await component.WaveLengthToSMatrixMap[LaserType.Red.WaveLengthInNm].GetLightPropagationAsync(inputVector, 1000, new());
            var outputPinLightVal = outputVector[outFlowID];
            return outputPinLightVal;
        }

        private void SetUpNonLinearConnection(Component component, string formula)
        {
            var mainConnection = (component.Parts[0,0].GetPinAt(CAP_Core.Tiles.RectSide.Left).IDInFlow, component.Parts[0,0].GetPinAt(CAP_Core.Tiles.RectSide.Right).IDOutFlow);
            var nonLinConnections = component.WaveLengthToSMatrixMap[LaserType.Red.WaveLengthInNm].NonLinearConnections;

            if (nonLinConnections.ContainsKey(mainConnection))
            {
                nonLinConnections[mainConnection] = (ConnectionFunction)MathExpressionReader.ConvertToDelegate(formula, component.GetAllPins(), component.GetAllSliders());
            }
        }
    }
}
