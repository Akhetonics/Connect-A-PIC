using CAP_Core;
using CAP_Core.CodeExporter;
using CAP_Core.Component.ComponentHelpers;
using CAP_Core.ExternalPorts;
using CAP_Core.Helpers;
using CAP_Core.LightFlow;
using CAP_Core.Tiles;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Windows.Input;

namespace ConnectAPIC.LayoutWindow.ViewModel
{
    public class GridViewModel 
    {
        public ICommand CreateComponentCommand { get; set; }
        public ICommand MoveComponentCommand { get; set; }
        public ICommand DeleteComponentCommand { get; set; }
        public ICommand RotateComponentCommand { get; set; }
        public ICommand ExportToNazcaCommand { get; set; }
        public TileView[,] TileViews { get; private set; }
        public int Width { get => TileViews.GetLength(0); }
        public int Height { get => TileViews.GetLength(1); }
        public Grid Grid { get; set; }
        public GridView GridView { get; set; }
        private GridSMatrixAnalyzer MatrixAnalyzer;
        public int MaxTileCount { get => Width*Height; }
        public GridViewModel(GridView gridview, Grid grid )
        {
            this.GridView = gridview;
            this.Grid = grid;
            //this.GridView.Columns = grid.Width;
            this.TileViews = new TileView[grid.Width, grid.Height];
            CreateComponentCommand = new CreateComponentCommand(grid);
            DeleteComponentCommand = new DeleteComponentCommand(grid);
            RotateComponentCommand = new RotateComponentCommand(grid);
            MoveComponentCommand = new MoveComponentCommand(grid);
            ExportToNazcaCommand = new ExportNazcaCommand(new NazcaExporter(), grid);
            CreateEmptyField(grid.Width, grid.Height);
            this.Grid.OnComponentPlacedOnTile += Grid_OnComponentPlacedOnTile;
            this.Grid.OnComponentRemoved += Grid_OnComponentRemoved;
        }

        private void Grid_OnComponentRemoved(ComponentBase component, int x, int y)
        {
            ResetTilesAt(x, y, component.WidthInTiles, component.HeightInTiles);
        }
        public void SetTileTexture(int x, int y, Texture2D texture, float rotationDegreesCounterClockwise)
        {
            if (!IsInGrid(x, y, 1, 1)) return;
            TileViews[x, y].ResetToDefault(texture);
            TileViews[x, y].Texture = texture;
            TileViews[x, y].RotationDegrees = RotationHelper.ToClockwise(rotationDegreesCounterClockwise);
        }
        private void Grid_OnComponentPlacedOnTile(ComponentBase component, int gridX, int gridY)
        {
            Type componentViewType = ComponentViewModelTypeConverter.ToView(component.GetType());
            CreateComponentViewByType(gridX, gridY, component.Rotation90CounterClock, componentViewType, component);
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
            DeleteAllTiles();
            //this.GridView.Columns = width;
            if (this.Width != width || this.Height != height)
            {
                this.TileViews = new TileView[width, height];
            }
            for (int gridy = 0; gridy < height; gridy++)
            {
                for (int gridx = 0; gridx < width; gridx++)
                {
                    TileViews[gridx, gridy] = GridView.DefaultTile.Duplicate();
                    TileViews[gridx, gridy].Visible = true;
                    TileViews[gridx, gridy].Initialize(this);
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
            ComponentView.Rotation90CounterClock = rotation;
            int width = ComponentView.WidthInTiles;
            int height = ComponentView.HeightInTiles;
            ComponentView.Show(x, y);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int gridX = x + i;
                    int gridY = y + j;
                    SetTileTexture(gridX, gridY, ComponentView.GetTexture(i, j).Duplicate() as Texture2D, (float)ComponentView.Rotation90CounterClock * 90f);
                    TileViews[gridX, gridY].ComponentView = ComponentView;
                    var part = componentModel.Parts[i, j];
                    var PinRightAbsoluteEdgePos = RectSide.Right.RotateSideCounterClockwise(part.Rotation90);
                    var PinDownAbsoluteEdgePos = RectSide.Down.RotateSideCounterClockwise(part.Rotation90);
                    var PinLeftAbsoluteEdgePos = RectSide.Left.RotateSideCounterClockwise(part.Rotation90);
                    var PinUpAbsoluteEdgePos = RectSide.Up.RotateSideCounterClockwise(part.Rotation90);
                    TileViews[gridX, gridY].PinRight.SetMatterType(part.GetPinAt(PinRightAbsoluteEdgePos)?.MatterType);
                    TileViews[gridX, gridY].PinDown.SetMatterType(part.GetPinAt(PinDownAbsoluteEdgePos)?.MatterType);
                    TileViews[gridX, gridY].PinLeft.SetMatterType(part.GetPinAt(PinLeftAbsoluteEdgePos)?.MatterType);
                    TileViews[gridX, gridY].PinUp.SetMatterType(part.GetPinAt(PinUpAbsoluteEdgePos)?.MatterType);
                }
            }
            return ComponentView;
        }

        public Dictionary<Guid, Complex> GetLightVector(LightColor color)
        {
            MatrixAnalyzer ??= new GridSMatrixAnalyzer(this.Grid);
            return MatrixAnalyzer.CalculateLightPropagation(color);
        }

        public void ShowLightPropagation(Dictionary<Guid, Complex> lightVectorRed, Dictionary<Guid, Complex> lightVectorGreen, Dictionary<Guid, Complex> lightVectorBlue)
        {
            // go through the whole grid and send all 
            AssignLightToComponentViews(lightVectorRed, LightColor.Red);
            AssignLightToComponentViews(lightVectorGreen, LightColor.Green);
            AssignLightToComponentViews(lightVectorBlue, LightColor.Blue);
        }

        private void AssignLightToComponentViews(Dictionary<Guid, Complex> lightVectorRed , LightColor color)
        {
            for (int x = 0; x < Grid.Width; x++)
            {
                for (int y = 0; y < Grid.Height; y++)
                {
                    var componentModel = Grid.GetComponentAt(x, y);
                    if (componentModel == null) continue;
                    var part = componentModel.GetPartAtGridXY(x, y);
                    foreach (var side in Enum.GetValues(typeof(RectSide)).OfType<RectSide>())
                    {
                        var pin = part.GetPinAt(side);
                        var lightIntensityIn = lightVectorRed[pin.IDInFlow].Real;
                        var lightPhaseIn = lightVectorRed[pin.IDInFlow].Phase;
                        var lightIntensityOut = lightVectorRed[pin.IDOutFlow].Real;
                        var lightPhaseOut = lightVectorRed[pin.IDOutFlow].Phase;
                        var pinView = TileViews[x, y].GetPinAt(side);

                        pinView.LightIn[color] = new Complex(lightIntensityIn, lightPhaseIn);
                        pinView.LightOut[color] = new Complex(lightIntensityOut, lightPhaseOut);
                    }
                    TileViews[x, y].ComponentView.DisplayLightVector();
                }
            }
        }

        public void HideLightPropagation()
        {
            for (int x = 0; x < Grid.Width; x++)
            {
                for (int y = 0; y < Grid.Height; y++)
                {
                    TileViews[x, y].ComponentView.HideLightVector();
                }
            }
        }
    }
}
