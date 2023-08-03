using ConnectAPIC.LayoutWindow.View;
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
        public GridViewModel(GridView gridview, Grid grid )
        {
            this.GridView = gridview;
            this.Grid = grid;
            this.GridView.OnNewTileDropped += GridView_CreateNewComponent;
            this.GridView.OnTileMiddleMouseClicked += tile => Grid.UnregisterComponentAt(tile.GridX, tile.GridY);
            this.GridView.OnExistingTileDropped += (tile, componentView) => Grid.MoveComponent(tile.GridX, tile.GridY, componentView.GridX,componentView.GridY);
            this.GridView.OnTileRightClicked += tile => Grid.RotateComponentBy90(tile.GridX, tile.GridY);
            this.GridView.DeleteAllTiles();
            this.GridView.CreateEmptyField(grid.Width, grid.Height);
            this.Grid.OnComponentPlacedOnTile += Grid_OnComponentPlacedOnTile;
            this.Grid.OnComponentRemoved += Grid_OnComponentRemoved;
        }

        private void GridView_CreateNewComponent(TileView tile, ComponentBaseView componentView)
        {
            Type specificComponentToBePlaced = null;
            if (componentView.GetType().IsAssignableFrom(typeof(StraightWaveGuideView)))
            {
                specificComponentToBePlaced = typeof(StraightWaveGuide);
            }
            Grid.PlaceComponentByType(tile.GridX, tile.GridY, specificComponentToBePlaced);
        }

        private void Grid_OnComponentRemoved(ComponentBase component, int x, int y)
        {
            GridView.ResetTilesAt(x, y, component.WidthInTiles, component.HeightInTiles);
        }

        private void Grid_OnComponentPlacedOnTile(ComponentBase component, int gridX, int gridY)
        {
            int compWidth = component.WidthInTiles;
            int compHeight = component.HeightInTiles;
            var componentView = ComponentViewFactory.Instance.CreateComponentView(component.GetType());
            for(int i = 0; i < compWidth; i++)
            {
                for (int j = 0; j < compHeight; j++)
                {
                    GridView.SetTileTexture(i+gridX, j+gridY, componentView.GetTexture(i,j), (int)component.Rotation90*90);
                }
            }
        }
        
        
    }
}
