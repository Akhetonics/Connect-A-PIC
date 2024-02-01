using CAP_Core;
using CAP_Core.Components;
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
        const string formula = "ToComplexFromPolar(Sub(1.0, SLIDER0), 1.2161003820350373)";
        [Fact]
        public void SliderCalculationTest()
        {
            // Arrange
            var converter = new ComponentDraftConverter(new Logger());
            var reader = new ComponentDraftFileReader(new DummyDataAccessor(TestComponentFactory.StraightWGJson));
            var draftOrError = reader.TryReadJson("");
            
            Assert.True(string.IsNullOrEmpty(draftOrError.error), $"Error reading Json: {draftOrError.error}");
            Assert.NotNull(draftOrError.draft);

            var component = converter.ToComponentModels(new List<ComponentDraft> { draftOrError.draft }).First();
            SetUpNonLinearConnection(component);
            var outFlowID = component.Parts[0, 0].GetPinAt(CAP_Core.Tiles.RectSide.Right).IDOutFlow;
            var inputVector = MathNet.Numerics.LinearAlgebra.Vector<Complex>.Build.Dense(new Complex[] { 1, 0, 0, 0 });
            component.GetSlider(0).Value = 0.5;

            // Act
            var outputVector = component.WaveLengthToSMatrixMap[LaserType.Red.WaveLengthInNm].GetLightPropagationAsync(inputVector, 1000);
            var outputPinLightVal = outputVector[outFlowID];

            // Assert
            var expectedValue = Complex.FromPolarCoordinates(0.5, 1.2161003820350373);
            Assert.Equal(expectedValue, outputPinLightVal);
        }

        private void SetUpNonLinearConnection(Component component)
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
