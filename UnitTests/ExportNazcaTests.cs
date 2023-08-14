using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using ConnectAPIC.Scenes.Compiler;
using Tiles;
using Model;
using ConnectAPIC.LayoutWindow.Model.ExternalPorts;
using System.Linq;

namespace UnitTests
{
    public class ExportNazcaTests
    {
        [Fact]
        public void NazcaCompilerTest()
        {
            Grid grid = new(24,12);
            var inputs = grid.ExternalPorts.Where(p => p.GetType() == typeof(StandardInput)).ToList();
            var inputHeight = inputs.FirstOrDefault().TilePositionY;
            var firstStraightLine = new StraightWaveGuide();
            // add grid components and tiles
            grid.PlaceComponent(0, inputHeight, firstStraightLine);
            var secondComponent = new StraightWaveGuide();
            var GridXSecondComponent = firstStraightLine.GridXMainTile + firstStraightLine.WidthInTiles;
            grid.PlaceComponent(GridXSecondComponent, inputHeight, secondComponent);
            NazcaCompiler compiler = new(grid);
            // test if parameters in NazcaFunctionParameters work - like the DirectionalCoupler
            var neighboursOfComponentOne = grid.GetConnectedNeighbours(grid.Tiles[1,inputHeight]);
            neighboursOfComponentOne.Contains(secondComponent);
            Assert.True(neighboursOfComponentOne.Count > 0);
            var output = compiler.Compile();

        }

        [Fact]
        public void GetConnectedNeighboursTest()
        {
            Grid grid = new(24, 12);
            var inputs = grid.ExternalPorts.Where(p => p.GetType() == typeof(StandardInput)).ToList();
            var inputHeight = inputs.FirstOrDefault().TilePositionY;
            var firstStraightLine = new StraightWaveGuide();
            // add grid components and tiles
            grid.PlaceComponent(0, inputHeight, firstStraightLine);
            var secondComponent = new DirectionalCoupler();
            var GridXSecondComponent = firstStraightLine.GridXMainTile + firstStraightLine.WidthInTiles;
            grid.PlaceComponent(GridXSecondComponent, inputHeight, secondComponent);

            var Comp1ConnectionPart = firstStraightLine.GetPartAt(1, 0);
            var Comp2ConnectionPart = secondComponent.GetPartAt(0, 0);

            NazcaCompiler compiler = new(grid);
            // test if parameters in NazcaFunctionParameters work - like the DirectionalCoupler
            var neighboursOfComponentOne = grid.GetConnectedNeighbours(grid.Tiles[1, inputHeight]);
            neighboursOfComponentOne.Contains()
            Assert.True(neighboursOfComponentOne.Count > 0);



        }

    }
}