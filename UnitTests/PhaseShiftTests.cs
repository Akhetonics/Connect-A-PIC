using CAP_Core.CodeExporter;
using CAP_Core.ExternalPorts;
using System.Numerics;
using CAP_Core.Components.ComponentHelpers;
using CAP_Core.Grid;

namespace UnitTests
{
    public class PhaseShiftTests
    {

        [Fact]
        public static void ComplexFactorTest()
        {
            // I use this test to calculate the different imaginary values of the phase shift for given wirelengths to get realistic dummy values
            var imaginaryRed = PhaseShiftCalculator.CalculateImaginaryPart(250000, StandardWaveLengths.RedNM); 
            var imaginaryGreen = PhaseShiftCalculator.CalculateImaginaryPart(250000, StandardWaveLengths.GreenNM); 
            var imaginaryBlue = PhaseShiftCalculator.CalculateImaginaryPart(250000, StandardWaveLengths.BlueNM);
            Console.WriteLine();
        }
        [Fact]
        public void NazcaCompilerTest()
        {
            GridManager grid = new(24, 12);
            var inputs = grid.GetAllExternalInputs();
            int inputHeight = inputs.FirstOrDefault()?.TilePositionY ?? throw new Exception("there is no StandardInput defined");
            var firstComponent = TestComponentFactory.CreateStraightWaveGuide();
            grid.PlaceComponent(0, inputHeight, firstComponent);
            var secondComponent = ExportNazcaTests.PlaceAndConcatenateComponent(grid, firstComponent);
            var thirdComponent = ExportNazcaTests.PlaceAndConcatenateComponent(grid, secondComponent);
            var fourthComponent = ExportNazcaTests.PlaceAndConcatenateComponent(grid, thirdComponent);
            var orphan = TestComponentFactory.CreateStraightWaveGuide();
            grid.PlaceComponent(10, 5, orphan);

            NazcaExporter exporter = new();
            var output = exporter.Export(grid);
            var firstCellName = grid.Tiles[firstComponent.GridXMainTile, firstComponent.GridYMainTile].GetComponentCellName();
            var secondCellName = grid.Tiles[secondComponent.GridXMainTile, secondComponent.GridYMainTile].GetComponentCellName();
            var thirdCellName = grid.Tiles[thirdComponent.GridXMainTile, thirdComponent.GridYMainTile].GetComponentCellName();
            var fourthCellName = grid.Tiles[fourthComponent.GridXMainTile, fourthComponent.GridYMainTile].GetComponentCellName();
            var orphanCellName = grid.Tiles[orphan.GridXMainTile, orphan.GridYMainTile].GetComponentCellName();

            // assert if all components are in the string output
            Assert.Contains(firstCellName, output);
            Assert.Contains(secondCellName, output);
            Assert.Contains(thirdCellName, output);
            Assert.Contains(fourthCellName, output);
            Assert.Contains(orphanCellName, output);
        }
    }
}
