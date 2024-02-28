using CAP_Core.ExternalPorts;
using CAP_Core.Grid;
using CAP_Core.Components.ComponentHelpers;
using CAP_Core.LightCalculation;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAP_Core.Helpers;

namespace UnitTests.LightCalculation
{
    public class GridLightCalculatorTests
    {
        [Fact]
        public async Task MMILightCalculationTest ()
        {
            GridManager grid = new GridManager(40, 40);
            var component = TestComponentFactory.CreateComponent(TestComponentFactory.MMI3x3);
            component.HeightInTiles.ShouldBe(3);
            // make all external Ports red so we have three red inputLasers
            grid.ExternalPorts[0] = new ExternalInput(grid.ExternalPorts[0].PinName, LaserType.Red, grid.ExternalPorts[0].TilePositionY, 1.0 / 3);
            grid.ExternalPorts[1] = new ExternalInput(grid.ExternalPorts[1].PinName, LaserType.Red, grid.ExternalPorts[1].TilePositionY, 1.0 / 3);
            grid.ExternalPorts[2] = new ExternalInput(grid.ExternalPorts[2].PinName, LaserType.Red, grid.ExternalPorts[2].TilePositionY, 1.0 / 3);

            var RedLaserYPos = grid.GetAllExternalInputs().FirstOrDefault(i => i.LaserType == LaserType.Red).TilePositionY;
            grid.PlaceComponent(0, RedLaserYPos, component);

            // create systemmatrix of this two components
            SystemMatrixBuilder builder = new SystemMatrixBuilder(grid);
            var systemMatrix = builder.GetSystemSMatrix(LaserType.Red.WaveLengthInNm);
            var allConnections = systemMatrix.GetNonNullValues();
            var pinLeftUp = (Guid)component.PinIdLeftIn();
            var pinRightUp = (Guid)component.PinIdRightOut(2, 0);
            var expectedField = Math.Sqrt(1.0 / 3.0);

            // start light calculation
            var calculator = new GridLightCalculator(builder, grid);
            var fieldVector = await calculator.CalculateFieldPropagationAsync(new CancellationTokenSource(), LaserType.Red.WaveLengthInNm);

            var usedInputs = grid.GetUsedExternalInputs()
                                 .Where(i => i.Input.LaserType.WaveLengthInNm == LaserType.Red.WaveLengthInNm)
                                 .ToList();
            var inputVector = UsedInputConverter.ToVectorOfFields(usedInputs, calculator.SystemSMatrix);
            var inputAfterSteps = calculator.SystemSMatrix.SMat * inputVector + inputVector;

            fieldVector[pinRightUp].Magnitude.ShouldBe(expectedField, 0.000000001);
            fieldVector[pinRightUp].Magnitude.ShouldBe(expectedField, 0.000000001);
            fieldVector[pinRightUp].Magnitude.ShouldBe(expectedField, 0.000000001);
            fieldVector[pinRightUp].Magnitude.ShouldBe(expectedField, 0.000000001);

        }
        
    }
}
