using CAP_Core.CodeExporter;
using CAP_Core.Component;
using CAP_Core.ExternalPorts;
using CAP_Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using CAP_Core.Component.ComponentHelpers;

namespace UnitTests
{
    public class PhaseShiftTests
    {

        [Fact]
        public static void ComplexFactorTest()
        {
            Complex factor1 = PhaseShiftCalculator.GetDegrees(PhaseShiftCalculator.TileWidthInNM, PhaseShiftCalculator.laserWaveLengthRedNM); // widthInTiles is 1 for the first component
            Complex factor2 = PhaseShiftCalculator.GetDegrees(PhaseShiftCalculator.TileWidthInNM, PhaseShiftCalculator.laserWaveLengthRedNM); // widthInTiles is 1 also for the second component

            Complex result = factor1 * factor2; 

        }
        [Fact]
        public void NazcaCompilerTest()
        {
            Grid grid = new(24, 12);
            var inputs = grid.ExternalPorts.Where(p => p.GetType() == typeof(StandardInput)).ToList();
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
