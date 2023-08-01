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
        [Export] public GridView GridView { get; set; }
        public static int MaxTileCount { get; private set; }
        public GridViewModel(GridView gridView)
        {
            this.GridView = gridView;
            this.Grid = new Grid(gridView.Columns, gridView.Columns);
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
        }

        private void Grid_OnComponentRemoved(ComponentBase component, int x, int y)
        {
            component.ComponentView.Hide();
            GridView.ResetTilesAt(x, y, component.WidthInTiles, component.HeightInTiles);
        }

        private void Grid_OnComponentPlacedOnTile(ComponentBase component, int x, int y)
        {
            component.ComponentView.Show(x, y);
            int compWidth = component.WidthInTiles;
            int compHeight = component.HeightInTiles;
            for(int i = 0; i < compWidth; i++)
            {
                for (int j = 0; j < compWidth; j++)
                {
                    var part = component.GetPartAt(i, j);
                    GridView.SetTileTexture(i+x, j+y, component.ComponentView.GetTexture(i,j), (int)component.Rotation90*90);
                }
            }
        }
        
        
    }
}
