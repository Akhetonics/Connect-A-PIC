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
            var thirdComponent = new StraightWaveGuide();
            var GridXThirdComponent = secondComponent.GridXMainTile + secondComponent.WidthInTiles;
            grid.PlaceComponent(GridXThirdComponent, inputHeight, thirdComponent);
            var fourthComponent = new StraightWaveGuide();
            var GridXFourthComponent = thirdComponent.GridXMainTile + thirdComponent.WidthInTiles;
            grid.PlaceComponent(GridXSecondComponent, inputHeight, secondComponent);
            NazcaCompiler compiler = new(grid);
            // test if parameters in NazcaFunctionParameters work - like the DirectionalCoupler
            Tile firstComponentMainTile = grid.Tiles[firstStraightLine.GridXMainTile, inputHeight];
            var neighboursOfComponentOne = grid.GetConnectedNeighbours(firstComponentMainTile);
            Tile secondComponentMainTile = grid.Tiles[secondComponent.GridXMainTile, inputHeight];
            Assert.True(neighboursOfComponentOne.Contains(secondComponentMainTile));
            Assert.True(neighboursOfComponentOne.Count > 0);
            var output = compiler.Compile();

        }

        private Tile GetComponentMainTile(Grid grid, ComponentBase component)
        {
            Tile secondComponentMainTile = grid.Tiles[firstStraightLine.GridXMainTile, inputHeight];
            var neighboursOfComponentOne = grid.GetConnectedNeighbours(firstComponentMainTile);
            Assert.True(neighboursOfComponentOne.Contains(secondComponentMainTile));
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
            var secondComponent = new StraightWaveGuide();
            var GridXSecondComponent = firstStraightLine.GridXMainTile + firstStraightLine.WidthInTiles;
            grid.PlaceComponent(GridXSecondComponent, inputHeight, secondComponent);
            var thirdComponent = new StraightWaveGuide();
            var GridXThirdComponent = secondComponent.GridXMainTile + secondComponent.WidthInTiles;
            grid.PlaceComponent(GridXThirdComponent, inputHeight, thirdComponent);
            var fourthComponent = new StraightWaveGuide();
            var GridXFourthComponent = thirdComponent.GridXMainTile + thirdComponent.WidthInTiles;
            grid.PlaceComponent(GridXSecondComponent, inputHeight, secondComponent);
            NazcaCompiler compiler = new(grid);
            // test if parameters in NazcaFunctionParameters work - like the DirectionalCoupler
            Tile firstComponentMainTile = grid.Tiles[firstStraightLine.GridXMainTile, inputHeight];
            Tile secondComponentMainTile = grid.Tiles[firstStraightLine.GridXMainTile, inputHeight];
            var neighboursOfComponentOne = grid.GetConnectedNeighbours(firstComponentMainTile);
            var neighboursOfComponentTwo = grid.GetConnectedNeighbours(secondComponentMainTile);
            Tile secondComponentMainTile = grid.Tiles[secondComponent.GridXMainTile, inputHeight];
            Tile secondComponentMainTile = grid.Tiles[secondComponent.GridXMainTile, inputHeight];
            Assert.True(neighboursOfComponentOne.Contains(secondComponentMainTile));
            Assert.True(neighboursOfComponentOne.Contains(secondComponentMainTile));
            Assert.True(neighboursOfComponentOne.Count > 0);
            var output = compiler.Compile();



        }

    }
}