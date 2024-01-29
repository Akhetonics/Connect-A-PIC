using CAP_Core;
using CAP_Core.Components;
using CAP_Core.Components.FormulaReading;
using CAP_Core.ExternalPorts;
using CAP_Core.Grid.FormulaReading;
using CAP_DataAccess.Components.ComponentDraftMapper;
using Shouldly;
using System.Numerics;

namespace UnitTests.Components.FormulaReading
{
    public class SliderTests
    {
        [Fact]
        public void SliderCalculationTest()
        {
            // test if slider gets loaded properly from json together with nonLinearFormula
            // test if slider is only updated once when the Smatrix.GetLightPropagation() is being called
            // ok so I have a json with a slider, I read it in, I then have a model, then I place the model into the grid
            // I start the displayLightpropagation thing maybe even from the viewmodel
            // I check what values the smatrix has at the position of the slider of my component.
            
            var converter = new ComponentDraftConverter(new Logger());
            var reader = new ComponentDraftFileReader(new DummyDataAccessor(TestComponentFactory.StraightWGJsonString));
            var draftOrError = reader.TryReadJson("");
            if (String.IsNullOrEmpty(draftOrError.error) == false ||draftOrError.draft == null) throw new Exception("Error reading Json from TestComponentFactory.Straight" +draftOrError.error);
            var component = converter.ToComponentModels(new() { draftOrError.draft }).First();
            // insert the formula into the nonLinearConnection of the component to be sure it will be that one we test 
            var inFlowPinID = component.GetPartAt(0, 0).GetPinAt(CAP_Core.Tiles.RectSide.Left).IDInFlow;
            var outFlowPinID = component.GetPartAt(0, 0).GetPinAt(CAP_Core.Tiles.RectSide.Right).IDOutFlow;
            var mainConnection = (inFlowPinID, outFlowPinID);
            var formula = "ToComplexFromPolar( Sub(1.0 ,SLIDER0), 1.2161003820350373)";
            var nonLinConnections = component.WaveLengthToSMatrixMap[LaserType.Red.WaveLengthInNm].NonLinearConnections;
            if(nonLinConnections.ContainsKey(mainConnection))
            {
                nonLinConnections[mainConnection] = (ConnectionFunction)MathExpressionReader.ConvertToDelegate(formula, component.GetAllPins(),component.SliderMap.Values.ToList());
            }
            var inputVector = MathNet.Numerics.LinearAlgebra.Vector<Complex>.Build.Dense(new Complex[]
            {
                1 , 0 , 0 , 0
            });
            component.SliderMap[0].Value = 0.5;
            var outputVector = component.WaveLengthToSMatrixMap[LaserType.Red.WaveLengthInNm].GetLightPropagation(inputVector, 1000);
            var outputPinLightVal = outputVector[outFlowPinID];
            
            outputPinLightVal.ShouldBe(Complex.FromPolarCoordinates(1 - 0.5, 1.2161003820350373));
        }
    }
}
