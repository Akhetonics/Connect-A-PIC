using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Tiles;

namespace ConnectAPIC.LayoutWindow.ViewModel
{
    public class GridViewModel :INotifyPropertyChanged
    {
        public TileView[,] TileViews { get; private set; }
        public int Width { get => TileViews.GetLength(0); }
        public int Height { get => TileViews.GetLength(1); }
        public Grid Grid { get; set; }
        public GridView GridView { get; set; }
        public int MaxTileCount { get => Width*Height; }
        public event PropertyChangedEventHandler PropertyChanged;
        public GridViewModel(GridView gridview, Grid grid )
        {
            
            this.GridView = gridview;
            this.Grid = grid;
            this.GridView.OnNewTileDropped += GridView_CreateNewComponent;
            this.GridView.OnTileMiddleMouseClicked += tile => Grid.UnregisterComponentAt(tile.GridX, tile.GridY);
            this.GridView.OnExistingTileDropped += (tile, componentView) => Grid.MoveComponent(tile.GridX, tile.GridY, componentView.GridX,componentView.GridY);
            this.GridView.OnTileRightClicked += tile => Grid.RotateComponentBy90(tile.GridX, tile.GridY);
            DeleteAllTiles();
            CreateEmptyField(grid.Width, grid.Height);
            this.Grid.OnComponentPlacedOnTile += Grid_OnComponentPlacedOnTile;
            this.Grid.OnComponentRemoved += Grid_OnComponentRemoved;
        }

        public void ResetAllTileViews(int width, int height)
        {
            TileViews = new TileView[width, height];
        }

        private void GridView_CreateNewComponent(TileView tile, ComponentBaseView componentView)
        {
            Type componentType = null;
            if (componentView is StraightWaveGuideView)
            {
                componentType = typeof(StraightWaveGuide);
            }
            Grid.PlaceComponentByType(tile.GridX, tile.GridY, componentType);
        }

        private void Grid_OnComponentRemoved(ComponentBase component, int x, int y)
        {
            ResetTilesAt(x, y, component.WidthInTiles, component.HeightInTiles);
        }
        public void SetTileTexture(int x, int y, Texture2D texture, float rotationDegrees)
        {
            if (!IsInGrid(x, y, 1, 1)) return;
            TileViews[x, y].ResetToDefault(texture);
            TileViews[x, y].Texture = texture;
            TileViews[x, y].RotationDegrees = rotationDegrees;
        }
        private void Grid_OnComponentPlacedOnTile(ComponentBase component, int gridX, int gridY)
        {
            Type componentViewType = null;
            if(component is StraightWaveGuide)
            {
                componentViewType = typeof(StraightWaveGuideView);
            }
            CreateComponentViewByType(gridX, gridY, component.Rotation90, componentViewType, component);
        }
        public bool IsInGrid(int x, int y, int width, int height)
        {
            return x >= 0 && y >= 0 && x + width <= this.Width && y + height <= this.Height;
        }
        

        public void DeleteAllTiles()
        {
            foreach (TileView t in TileViews)
            {
                GridView.RemoveChild(t);
            }
        }
        public void CreateEmptyField(int width, int height)
        {
            if (width <= 0 || height <= 0) return;
            this.GridView.Columns = width;
            if (this.Width != width || this.Height != height)
            {
                this.TileViews = new TileView[width, height];
            }
            for (int gridy = 0; gridy < width; gridy++)
            {
                for (int gridx = 0; gridx < height; gridx++)
                {
                    TileViews[gridx, gridy] = GridView.DefaultTile.Duplicate();
                    TileViews[gridx, gridy].Visible = true;
                    TileViews[gridx, gridy].OnMiddleClicked += TileView => Grid.UnregisterComponentAt(TileView.GridX, TileView.GridY);
                    TileViews[gridx, gridy].OnRightClicked += tile => Grid.RotateComponentBy90(tile.GridX, tile.GridY);
                    TileViews[gridx, gridy].OnNewTileDropped += GridView_CreateNewComponent;
                    TileViews[gridx, gridy].OnExistingTileDropped += (tile, componentView) => Grid.MoveComponent(tile.GridX, tile.GridY, componentView.GridX, componentView.GridY); ;
                    TileViews[gridx, gridy].SetPositionInGrid(gridx, gridy);
                    GridView.AddChild(TileViews[gridx, gridy]);
                }
            }
        }

        public void ResetTilesAt(int x, int y, int width, int height)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (!IsInGrid(i + x, j + y, 1, 1)) continue;
                    TileViews[x + i, y + j].ResetToDefault(GridView.DefaultTile.Texture);
                }
            }
        }
        public ComponentBaseView CreateComponentViewByType(int x, int y, DiscreteRotation rotation, Type componentViewType, ComponentBase componentModel)
        {
            var ComponentView = ComponentViewFactory.Instance.CreateComponentView(componentViewType);
            ComponentView.Rotation90 = rotation;
            int width = ComponentView.WidthInTiles;
            int height = ComponentView.HeightInTiles;
            ComponentView.Show(x, y);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int gridX = x + i;
                    int gridY = y + j;
                    SetTileTexture(gridX, gridY, ComponentView.GetTexture(i, j).Duplicate() as Texture2D, (float)ComponentView.Rotation90 * 90f);
                    TileViews[gridX, gridY].ComponentView = ComponentView;
                    var part = componentModel.Parts[i, j];
                    var PinRightAbsoluteEdgePos = RectangleSide.Right.RotateSideCounterClockwise(part.Rotation90);
                    var PinDownAbsoluteEdgePos = RectangleSide.Down.RotateSideCounterClockwise(part.Rotation90);
                    var PinLeftAbsoluteEdgePos = RectangleSide.Left.RotateSideCounterClockwise(part.Rotation90);
                    var PinUpAbsoluteEdgePos = RectangleSide.Up.RotateSideCounterClockwise(part.Rotation90);
                    TileViews[gridX, gridY].PinRight.SetMatterType(part.GetPinAt(PinRightAbsoluteEdgePos).MatterType);
                    TileViews[gridX, gridY].PinDown.SetMatterType(part.GetPinAt(PinDownAbsoluteEdgePos).MatterType);
                    TileViews[gridX, gridY].PinLeft.SetMatterType(part.GetPinAt(PinLeftAbsoluteEdgePos).MatterType);
                    TileViews[gridX, gridY].PinUp.SetMatterType(part.GetPinAt(PinUpAbsoluteEdgePos).MatterType);
                }
            }
            return ComponentView;
        }
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
