using CAP_Core.CodeExporter;
using CAP_Core.Components;
using CAP_Core.Components.ComponentHelpers;
using CAP_Core.ExternalPorts;
using CAP_Core.Grid;
using CAP_Core.Tiles;
using UnitTests.Grid;

namespace UnitTests
{
    public class ExportNazcaTests
    {
        [Fact]
        public void NazcaCompilerTest()
        {
            GridManager grid = GridHelpers.InitializeGridWithComponents(24, 12);
            var inputs = grid.ExternalPortManager.GetAllExternalInputs();
            int inputHeight = inputs.FirstOrDefault()?.TilePositionY ?? throw new Exception("there is no StandardInput defined");
            var firstComponent = TestComponentFactory.CreateDirectionalCoupler();
            grid.ComponentMover.PlaceComponent(0, inputHeight, firstComponent);
            var secondComponent = PlaceAndConcatenateComponent(grid, firstComponent);
            var thirdComponent = PlaceAndConcatenateComponent(grid, secondComponent);
            var fourthComponent = PlaceAndConcatenateComponent(grid, thirdComponent);
            var orphan = TestComponentFactory.CreateDirectionalCoupler();
            grid.ComponentMover.PlaceComponent(10, 5, orphan);
            
            NazcaExporter exporter = new();
            var output = exporter.Export(grid);
            var firstCellName = grid.TileManager.Tiles[firstComponent.GridXMainTile, firstComponent.GridYMainTile].GetComponentCellName();
            var secondCellName = grid.TileManager.Tiles[secondComponent.GridXMainTile, secondComponent.GridYMainTile].GetComponentCellName();
            var thirdCellName = grid.TileManager.Tiles[thirdComponent.GridXMainTile, thirdComponent.GridYMainTile].GetComponentCellName();
            var fourthCellName = grid.TileManager.Tiles[fourthComponent.GridXMainTile, fourthComponent.GridYMainTile].GetComponentCellName();
            var orphanCellName = grid.TileManager.Tiles[orphan.GridXMainTile, orphan.GridYMainTile].GetComponentCellName();
            
            // assert if all components are in the string output
            Assert.Contains(firstCellName, output);
            Assert.Contains(secondCellName, output);
            Assert.Contains(thirdCellName, output);
            Assert.Contains(fourthCellName, output);
            Assert.Contains(orphanCellName, output);
        }
        
        [Fact]
        public void GetConnectedNeighborsTest()
        {
            GridManager grid = GridHelpers.InitializeGridWithComponents(24,12);
            var inputs = grid.ExternalPortManager.GetAllExternalInputs();
            var firstInput = inputs.FirstOrDefault();
            if (firstInput == null) throw new Exception("Inputs not found, they seem not to be declared in the grid. Please do that now");
            var inputHeight = firstInput.TilePositionY;
            var firstComponent = TestComponentFactory.CreateStraightWaveGuide();
            // add grid components and tiles
            grid.ComponentMover.PlaceComponent(0, inputHeight, firstComponent);
            var secondComponent = PlaceAndConcatenateComponent(grid, firstComponent);
            var thirdComponent = PlaceAndConcatenateComponent(grid, secondComponent);
            
            // test if parameters in NazcaFunctionParameters work - like the DirectionalCoupler
            Tile firstComponentMainTile = grid.TileManager.Tiles[firstComponent.GridXMainTile, inputHeight];
            Tile secondComponentMainTile = grid.TileManager.Tiles[secondComponent.GridXMainTile, secondComponent.GridYMainTile];
            Tile thirdComponentMainTile = grid.TileManager.Tiles[thirdComponent.GridXMainTile, thirdComponent.GridYMainTile];
            
            Assert.Contains(secondComponentMainTile, grid.ComponentRelationshipManager.GetConnectedNeighborsOfComponent(firstComponent).Select(b=>b.Child));
            Assert.Contains(thirdComponentMainTile, grid.ComponentRelationshipManager.GetConnectedNeighborsOfComponent(secondComponent).Select(b => b.Child));
            Assert.Contains(firstComponentMainTile, grid.ComponentRelationshipManager.GetConnectedNeighborsOfComponent(secondComponent).Select(b => b.Child));
        }
        
        public static Component PlaceAndConcatenateComponent(GridManager grid, Component parentComponent)
        {
            Component newComponent = TestComponentFactory.CreateStraightWaveGuide();
            var GridXSecondComponent = parentComponent.GridXMainTile + parentComponent.WidthInTiles;
            var GridYSecondComponent = parentComponent.GridYMainTile + parentComponent.HeightInTiles-1;
            grid.ComponentMover.PlaceComponent(GridXSecondComponent, GridYSecondComponent, newComponent);
            return newComponent;
        }
    }
}
