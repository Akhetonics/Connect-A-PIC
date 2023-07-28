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
            Grid.GridView = width * height;
            TileViews = new TileView[Columns, Columns];
            RemoveAllTiles();
            CreateEmptyField();
        }
        public void CreateEmptyField()
        {
            if (DefaultTile == null) throw new ArgumentException("TileTemplate is not set in PICArea. Please tell the developer to fix that.");
            for (int i = 0; i < MaxTileCount; i++)
            {
                int gridX = i % this.Columns;
                int gridY = i / this.Columns;
                DefaultTile._Ready();
                var newTile = (TileView)DefaultTile.Duplicate();
                newTile._Ready();
                newTile.Visible = true;
                newTile.SetPositionInGrid(gridX, gridY);
                this.AddChild(newTile);
                TileViews[gridX, gridY] = newTile;
                TileViews[gridX, gridY].OnDeletionRequested += Grid_OnDeletionRequested;
                TileViews[gridX, gridY].OnRotationRequested += Grid_OnRotationRequested;
                TileViews[gridX, gridY].OnCreateNewComponentRequested += Grid_OnCreateNewComponent;
                TileViews[gridX, gridY].OnMoveComponentRequested += Grid_OnMoveExistingComponent;
            }
        }

        private void RemoveAllTiles()
        {
            var i = 0;
            foreach (Node n in this.FindChildren("*"))
            {
                UnregisterComponentAt(i % this.Columns, i / this.Columns);
                this.RemoveChild(n);
                i++;
            }
        }

        
        public bool IsInGrid(int x, int y, int width, int height)
        {
            return x >= 0 && y >= 0 && x + width <= Columns && y + height <= Columns;
        }
        public ComponentBase GetComponentAt(int x, int y)
        {
            if (IsInGrid(x, y, 1, 1) == false)
            {
                return null;
            }

            return TileViews[x, y].ComponentView;
        }

        public void PlaceExistingComponent(int x, int y, ComponentBase component)
        {
            if (IsColliding(x, y, component.WidthInTiles, component.HeightInTiles))
            {
                throw new ComponentCannotBePlacedException(component);
            }
            component.RegisterPositionInGrid(x, y);
            for (int i = 0; i < component.WidthInTiles; i++)
            {
                for (int j = 0; j < component.HeightInTiles; j++)
                {
                    int gridX = x + i;
                    int gridY = y + j;
                    Part part = component.GetPartAt(i, j);
                    TileViews[gridX, gridY].ResetToDefault(DefaultTile.Texture);
                    TileViews[gridX, gridY].ComponentView = component;
                    TileViews[gridX, gridY].Texture = part.Texture;
                    TileViews[gridX, gridY].AddChild(part);
                    TileViews[gridX, gridY].Rotation90 = component.Rotation90;
                }
            }
        }
        public void UnregisterComponentAt(int x, int y)
        {
            ComponentBase item = GetComponentAt(x, y);
            if (item == null)
            {
                return;
            }
            x = item.GridXMainTile;
            y = item.GridYMainTile;
            for (int i = 0; i < item.WidthInTiles; i++)
            {
                for (int j = 0; j < item.HeightInTiles; j++)
                {
                    TileViews[x + i, y + j].RemoveChild(item.GetPartAt(i, j));
                    TileViews[x + i, y + j].ResetToDefault(DefaultTile.Texture);
                }
            }
            item.ClearGridData();
        }
        public ComponentBase CreateAndPlaceComponent(int x, int y, Type componentType)
        {
            ComponentBase item = ComponentFactory.Instance.CreateComponent(componentType);
            if (IsColliding(x, y, item.WidthInTiles, item.HeightInTiles))
            {
                item.QueueFree();
                return null;
            }
            PlaceExistingComponent(x, y, item);
            return item;
        }

        private void Grid_OnRotationRequested(TileView tile)
        {
            if (tile == null || tile.ComponentView == null) return;

            var rotatedComponent = tile.ComponentView;
            int x = tile.ComponentView.GridXMainTile;
            int y = tile.ComponentView.GridYMainTile;
            UnregisterComponentAt(tile.GridX, tile.GridY);
            rotatedComponent.RotateBy90();
            try
            {
                PlaceExistingComponent(x, y, rotatedComponent);
            }
            catch (ComponentCannotBePlacedException)
            {
                rotatedComponent.Rotation90 = rotatedComponent.Rotation90 - 1;
                PlaceExistingComponent(x, y, rotatedComponent);
            }

        }
        private void Grid_OnDeletionRequested(TileView tile)
        {
            if (tile.ComponentView != null)
                UnregisterComponentAt(tile.ComponentView.GridXMainTile, tile.ComponentView.GridYMainTile);
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
