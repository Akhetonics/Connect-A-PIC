using CAP_Core;
using CAP_Core.Components;
using CAP_Core.Components.FormulaReading;
using CAP_Core.ExternalPorts;
using CAP_Core.Tiles;
using CAP_DataAccess.Components.ComponentDraftMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
            ;
            var converter = new ComponentDraftConverter(new Logger());
            var reader = new ComponentDraftFileReader(new DummyDataAccessor(TestComponentFactory.StraightWGJsonString));
            var draftOrError = reader.TryReadJson("");
            if (String.IsNullOrEmpty(draftOrError.error) == false ||draftOrError.draft == null) throw new Exception("Error reading Json from TestComponentFactory.Straight" +draftOrError.error);
            var component = converter.ToComponentModels(new() { draftOrError.draft }).First();
            var inputVector = MathNet.Numerics.LinearAlgebra.Vector<Complex>.Build.Dense(new Complex[]
            {
                
            });
            component.LaserWaveLengthToSMatrixMap[LaserType.Red.WaveLengthInNm].GetLightPropagation(inputVector, 1000);

        }
    }
}
