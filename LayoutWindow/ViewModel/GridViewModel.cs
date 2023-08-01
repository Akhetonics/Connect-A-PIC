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
            this.GridView.OnNewTileDropped += (tile, component) => Grid.PlaceComponent(tile.GridX,tile.GridY, component);
            this.GridView.OntileMiddleMouseClicked += tile=>Grid.UnregisterComponentAt(tile.GridX,tile.GridY);
            this.GridView.OnExistingTileDropped+= (tile, component) => Grid.MoveComponent(tile.GridX, tile.GridY, component);
            this.GridView.OnTileRightClicked += tile => Grid.RotateComponentBy90(tile.GridX, tile.GridY);
            this.GridView.DeleteAllTiles();
            this.GridView.CreateEmptyField(grid.Width, grid.Height);
            this.Grid.OnComponentPlacedOnTile += Grid_OnComponentPlacedOnTile;
            this.Grid.OnComponentRemoved += Grid_OnComponentRemoved;
            this.Grid.OnComponentMoved += Grid_OnComponentMoved;
            this.Grid.OnComponentRotated += Grid_OnComponentRotated;
        }

        private void Grid_OnComponentRotated(ComponentBase component, int x, int y)
        {
            throw new NotImplementedException();
        }

        private void Grid_OnComponentMoved(ComponentBase component, int x, int y)
        {
            throw new NotImplementedException();
        }

        private void Grid_OnComponentRemoved(ComponentBase component, int x, int y)
        {
            throw new NotImplementedException();
        }

        private void Grid_OnComponentPlacedOnTile(ComponentBase component, int x, int y)
        {
            int compWidth = component.WidthInTiles;
            int compHeight = component.HeightInTiles;
            for(int i = 0; i < compWidth; i++)
            {
                for (int j = 0; j < compWidth; j++)
                {
                    var part = component.GetPartAt(i, j);
                    GridView.SetTileTexture(x, y, part.Texture, (int)part.Rotation90*90);
                }
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
