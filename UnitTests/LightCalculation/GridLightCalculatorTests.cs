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
using UnitTests.Grid;

namespace UnitTests.LightCalculation
{
    public class GridLightCalculatorTests
    {
        [Fact]
        public async Task MMILightCalculationTest ()
        {
            var grid = new GridManager(40,40);
            var component = TestComponentFactory.CreateComponent(TestComponentFactory.MMI3x3);
            component.HeightInTiles.ShouldBe(3);
            // make all external Ports red so we have three red inputLasers
            grid.ExternalPortManager.ExternalPorts[0] = new ExternalInput(grid.ExternalPortManager.ExternalPorts[0].PinName, LaserType.Red, grid.ExternalPortManager.ExternalPorts[0].TilePositionY, 1.0 / 3.0);
            grid.ExternalPortManager.ExternalPorts[1] = new ExternalInput(grid.ExternalPortManager.ExternalPorts[1].PinName, LaserType.Red, grid.ExternalPortManager.ExternalPorts[1].TilePositionY, 1.0 / 3.0);
            grid.ExternalPortManager.ExternalPorts[2] = new ExternalInput(grid.ExternalPortManager.ExternalPorts[2].PinName, LaserType.Red, grid.ExternalPortManager.ExternalPorts[2].TilePositionY, 1.0 / 3.0);

            var FirstRedLaserYPos = grid.ExternalPortManager.ExternalPorts[0].TilePositionY;
            grid.ComponentMover.PlaceComponent(0, FirstRedLaserYPos, component);

            // create systemMatrix of this two components
            SystemMatrixBuilder builder = new SystemMatrixBuilder(grid);
            var systemMatrix = builder.GetSystemSMatrix(LaserType.Red.WaveLengthInNm);
            var allConnections = systemMatrix.GetNonNullValues();
            var pinLeftUp = (Guid)component.PinIdLeftIn();
            var pinRightUp = (Guid)component.PinIdRightOut(2, 0);
            var pinRightMiddle = (Guid)component.PinIdRightOut(2, 1);
            var pinRightDown = (Guid)component.PinIdRightOut(2, 2);
            // start light calculation
            var calculator = new GridLightCalculator(builder, grid);
            var fieldVector = await calculator.CalculateFieldPropagationAsync(new CancellationTokenSource(), LaserType.Red.WaveLengthInNm);
            var expectedField = Math.Sqrt(1.0 );

            var usedInputs = grid.ExternalPortManager.GetUsedExternalInputs()
                                 .Where(i => i.Input.LaserType.WaveLengthInNm == LaserType.Red.WaveLengthInNm)
                                 .ToConcurrentBag();
            var inputVector = UsedInputConverter.ToVectorOfFields(usedInputs, calculator.SystemSMatrix);
            var inputAfterSteps = calculator.SystemSMatrix.SMat * inputVector + inputVector;

            fieldVector[pinRightUp].Magnitude.ShouldBe(expectedField, 0.000000001);
            fieldVector[pinRightMiddle].Magnitude.ShouldBe(expectedField, 0.000000001);
            fieldVector[pinRightDown].Magnitude.ShouldBe(expectedField, 0.000000001);
        }
        
    }
}
