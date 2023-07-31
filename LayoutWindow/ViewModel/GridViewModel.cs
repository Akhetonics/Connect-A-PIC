using ConnectAPIC.Scenes.Component;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles;

namespace ConnectAPIC.LayoutWindow.ViewModel
{
    public class GridViewModel
    {
        public Grid Grid { get; set; }
        public GridView GridView { get; set; }
        public static int MaxTileCount { get; private set; }
        public GridViewModel(GridView gridView)
        {
            this.Grid = new Grid(12,12);
            this.GridView = gridView;
            
            // get note when a component was dragged into the grid so we can create a new component
            // get note when middle key was clicked on a tile -> request for deletion on tilex tiley
            // get note when rightclick was detected -> request for rotation on tilex tiley
            
            
        }
        public GridViewModel(GridView gridview, Grid grid )
        {
            this.GridView = gridview;
            this.Grid = grid;
            this.GridView.OnNewTileDropped += Grid_OnCreateNewComponent;
            this.GridView.OntileMiddleMouseClicked += tile=>Grid.UnregisterComponentAt(tile.GridX,tile.GridY);
            this.GridView.OnExistingTileDropped+= Grid_OnMoveExistingComponent;
            this.GridView.OnTileRightClicked += tile => Grid.TryRotateComponentBy90(tile.GridX, tile.GridY);
            this.GridView.DeleteAllTiles();
            this.GridView.CreateEmptyField(grid.Width, grid.Height);
            this.Grid.OnGridCreated += Grid_OnGridCreated;
            this.Grid.OnComponentPlacedOnTile += Grid_OnComponentPlacedOnTile;
            this.Grid.OnComponentRemoved += Grid_OnComponentRemoved;
        }

        private void Grid_OnComponentRemoved(ComponentBase component, int x, int y)
        {
            throw new NotImplementedException();
        }

        private void Grid_OnComponentPlacedOnTile(ComponentBase component, int x, int y)
        {
            throw new NotImplementedException();
        }

        private void Grid_OnGridCreated(Tile[,] Tiles)
        {
            throw new NotImplementedException();
        }
        
        private void Grid_OnCreateNewComponent(TileView tile, ComponentBase componentBlueprint)
        {
            if (CanComponentBePlaced(tile.GridX, tile.GridY, componentBlueprint))
            {
                CreateAndPlaceComponent(tile.GridX, tile.GridY, componentBlueprint.GetType());
            }
        }

        private void Grid_OnMoveExistingComponent(TileView tile, ComponentBase component)
        {
            int oldMainGridx = component.GridXMainTile;
            int oldMainGridy = component.GridYMainTile;
            UnregisterComponentAt(component.GridXMainTile, component.GridYMainTile); // to avoid blocking itself from moving only one tile into its own subtiles
            try
            {
                PlaceExistingComponent(tile.GridX, tile.GridY, component);
            }
            catch (ComponentCannotBePlacedException)
            {
                PlaceExistingComponent(oldMainGridx, oldMainGridy, component);
            }
        }

        public void Save(string Path)
        {

        }

        public void Export(string Path)
        {

        }
        public bool CanComponentBePlaced(int gridX, int gridY, ComponentBase component)
        {
            return !IsColliding(gridX, gridY, component.WidthInTiles, component.HeightInTiles);
        }
        public void UpdateGlobalLightDistribution()
        {

        }
        
    }
}
