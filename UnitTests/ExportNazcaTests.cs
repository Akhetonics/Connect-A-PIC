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
            var firstComponent = new StraightWaveGuide();
            // add grid components and tiles
            grid.PlaceComponent(0, inputHeight, firstComponent);
            var secondComponent = new StraightWaveGuide();
            var GridXSecondComponent = firstComponent.GridXMainTile + firstComponent.WidthInTiles;
            grid.PlaceComponent(GridXSecondComponent, inputHeight, secondComponent);
            var thirdComponent = new StraightWaveGuide();
            var GridXThirdComponent = secondComponent.GridXMainTile + secondComponent.WidthInTiles;
            grid.PlaceComponent(GridXThirdComponent, inputHeight, thirdComponent);
            var fourthComponent = new StraightWaveGuide();
            var GridXFourthComponent = thirdComponent.GridXMainTile + thirdComponent.WidthInTiles;
            grid.PlaceComponent(GridXSecondComponent, inputHeight, secondComponent);
            NazcaCompiler compiler = new(grid);
            // test if parameters in NazcaFunctionParameters work - like the DirectionalCoupler
            Tile firstComponentMainTile = grid.Tiles[firstComponent.GridXMainTile, inputHeight];
            var neighboursOfComponentOne = grid.GetConnectedNeighboursOfComponent(firstComponent);
            Tile secondComponentMainTile = grid.Tiles[secondComponent.GridXMainTile, inputHeight];
            Assert.True(neighboursOfComponentOne.Contains(secondComponentMainTile));
            Assert.True(neighboursOfComponentOne.Count > 0);
            var output = compiler.Compile();

        }
        
        [Fact]
        public void GetConnectedNeighboursTest()
        {
            Grid grid = new(24, 12);
            var inputs = grid.ExternalPorts.Where(p => p.GetType() == typeof(StandardInput)).ToList();
            var inputHeight = inputs.FirstOrDefault().TilePositionY;
            var firstComponent = new StraightWaveGuide();
            // add grid components and tiles
            grid.PlaceComponent(0, inputHeight, firstComponent);
            var secondComponent = PlaceAndConcatenateComponent(grid, firstComponent);
            var thirdComponent = PlaceAndConcatenateComponent(grid, secondComponent);
            var fourthComponent = PlaceAndConcatenateComponent(grid, thirdComponent);
            NazcaCompiler compiler = new(grid);
            // test if parameters in NazcaFunctionParameters work - like the DirectionalCoupler
            Tile firstComponentMainTile = grid.Tiles[firstComponent.GridXMainTile, inputHeight];
            Tile secondComponentMainTile = grid.Tiles[secondComponent.GridXMainTile, secondComponent.GridYMainTile];
            Tile thirdComponentMainTile = grid.Tiles[thirdComponent.GridXMainTile, thirdComponent.GridYMainTile];

            Assert.True(grid.GetConnectedNeighboursOfComponent(firstComponent).Contains(secondComponentMainTile));
            Assert.True(grid.GetConnectedNeighboursOfComponent(secondComponent).Contains(thirdComponentMainTile));
            Assert.True(grid.GetConnectedNeighboursOfComponent(secondComponent).Contains(firstComponentMainTile));

        }

        private static ComponentBase PlaceAndConcatenateComponent(Grid grid, ComponentBase parentComponent)
        {
            ComponentBase newComponent = new StraightWaveGuide();
            var GridXSecondComponent = parentComponent.GridXMainTile + parentComponent.WidthInTiles;
            grid.PlaceComponent(GridXSecondComponent, parentComponent.GridYMainTile, newComponent);
            return newComponent;
        }
    }
}