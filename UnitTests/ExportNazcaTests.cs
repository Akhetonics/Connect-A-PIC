using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using ConnectAPIC.Scenes.Compiler;
using Tiles;
using Model;
using ConnectAPIC.LayoutWindow.Model.ExternalPorts;
using System.Linq;
using System.Runtime.ExceptionServices;

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
            var firstComponent = new DirectionalCoupler();
            grid.PlaceComponent(0, inputHeight, firstComponent);
            var secondComponent = PlaceAndConcatenateComponent(grid, firstComponent);
            var thirdComponent = PlaceAndConcatenateComponent(grid, secondComponent);
            var fourthComponent = PlaceAndConcatenateComponent(grid, thirdComponent);
            
            NazcaCompiler compiler = new(grid);
            // test if parameters in NazcaFunctionParameters work - like the DirectionalCoupler
            var neighboursOfComponentOne = grid.GetConnectedNeighboursOfComponent(firstComponent);
            
            Assert.True(neighboursOfComponentOne.Count > 0);
            var output = compiler.Compile();
            Assert.Contains(firstComponent.NazcaFunctionName, output);
            Assert.Contains(secondComponent.NazcaFunctionName, output);
            Assert.Contains(thirdComponent.NazcaFunctionName, output);
            Assert.Contains(fourthComponent.NazcaFunctionName, output);
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
            
            // test if parameters in NazcaFunctionParameters work - like the DirectionalCoupler
            Tile firstComponentMainTile = grid.Tiles[firstComponent.GridXMainTile, inputHeight];
            Tile secondComponentMainTile = grid.Tiles[secondComponent.GridXMainTile, secondComponent.GridYMainTile];
            Tile thirdComponentMainTile = grid.Tiles[thirdComponent.GridXMainTile, thirdComponent.GridYMainTile];

            Assert.Contains(secondComponentMainTile, grid.GetConnectedNeighboursOfComponent(firstComponent));
            Assert.Contains(thirdComponentMainTile, grid.GetConnectedNeighboursOfComponent(secondComponent));
            Assert.Contains(firstComponentMainTile, grid.GetConnectedNeighboursOfComponent(secondComponent));
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