using CAP_Contracts.Logger;
using CAP_Core;
using CAP_Core.CodeExporter;
using CAP_Core.Components;
using CAP_Core.Components.ComponentHelpers;
using CAP_Core.Components.Creation;
using CAP_Core.ExternalPorts;
using CAP_Core.LightFlow;
using CAP_Core.Tiles;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using ConnectAPIC.Scripts.Helpers;
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
        public ComponentView[,] GridComponentViews { get; private set; }
        public int Width { get => GridComponentViews.GetLength(0); }
        public int Height { get => GridComponentViews.GetLength(1); }
        public Grid Grid { get; set; }
        public ILogger Logger { get; }
        public GridView GridView { get; set; }
        public GridSMatrixAnalyzer MatrixAnalyzer { get; private set; }
        public int MaxTileCount { get => Width * Height; }

        public GridViewModel(GridView gridView, Grid grid, ILogger logger)
        {
            this.GridView = gridView;
            this.Grid = grid;
            Logger = logger;
            //this.GridView.Columns = grid.Width;
            this.GridComponentViews = new ComponentView[grid.Width, grid.Height];
            CreateComponentCommand = new CreateComponentCommand(grid, ComponentFactory.Instance);
            DeleteComponentCommand = new DeleteComponentCommand(grid);
            RotateComponentCommand = new RotateComponentCommand(grid);
            MoveComponentCommand = new MoveComponentCommand(grid);
            ExportToNazcaCommand = new ExportNazcaCommand(new NazcaExporter(), grid);
            CreateEmptyField();
            this.Grid.OnComponentPlacedOnTile += Grid_OnComponentPlacedOnTile;
            this.Grid.OnComponentRemoved += Grid_OnComponentRemoved;
            MatrixAnalyzer = new GridSMatrixAnalyzer(this.Grid, StandardWaveLengths.RedNM);
        }

        private void Grid_OnComponentRemoved(Component component, int x, int y)
        {
            ResetTilesAt(x, y, component.WidthInTiles, component.HeightInTiles);
            if (GridView.lightPropagationIsPressed)
            {
                HideLightPropagation();
                ShowLightPropagation();
            }
        }
        private void Grid_OnComponentPlacedOnTile(Component component, int gridX, int gridY)
        {
            CreateComponentView(gridX, gridY, component.Rotation90CounterClock, component.TypeNumber);
            if (GridView.lightPropagationIsPressed)
            {
                HideLightPropagation();
                ShowLightPropagation();
            }
        }
        public bool IsInGrid(int x, int y, int width, int height)
        {
            return x >= 0 && y >= 0 && x + width <= this.Width && y + height <= this.Height;
        }

        public void CreateEmptyField()
        {
            foreach (var componentView in GridComponentViews)
            {
                if (componentView?.IsQueuedForDeletion() == false)
                    componentView.QueueFree();
            }
        }

        public void RegisterComponentViewInGridView(ComponentView componentView)
        {
            for (int x = componentView.GridX; x < componentView.GridX + componentView.WidthInTiles; x++)
            {
                for (int y = componentView.GridY; y < componentView.GridY + componentView.HeightInTiles; y++)
                {
                    if (!IsInGrid(x, y, 1, 1)) continue;
                    GridComponentViews[x, y] = componentView;
                }
            }
        }
        public void ResetTilesAt(int startX, int startY, int width, int height)
        {
            for (int x = startX; x < startX + width; x++)
            {
                for (int y = startY; y < startY + height; y++)
                {
                    if (!IsInGrid(x, y, 1, 1)) continue;
                    var oldComponent = GridComponentViews[x, y];
                    if (oldComponent == null) continue;
                    if (!oldComponent.IsQueuedForDeletion())
                    {
                        oldComponent.QueueFree();
                    }
                    GridComponentViews[x, y] = null;
                }
            }
        }
        public ComponentView CreateComponentView(int gridX, int gridY, DiscreteRotation rotationCounterClockwise, int componentTypeNumber)
        {
            var ComponentView = GridView.ComponentViewFactory.CreateComponentView(componentTypeNumber);
            ComponentView.RegisterInGrid(gridX, gridY, rotationCounterClockwise, this);
            RegisterComponentViewInGridView(ComponentView);
            GridView.DragDropProxy.AddChild(ComponentView); // it has to be the child of the DragDropArea to be displayed
            return ComponentView;
        }

        public Dictionary<Guid, Complex> GetLightVector(LaserType inputLight)
        {
            MatrixAnalyzer = new GridSMatrixAnalyzer(this.Grid, inputLight.WaveLengthInNm);
            
            return MatrixAnalyzer.CalculateLightPropagation();
        }

        public void ShowLightPropagation()
        {
            var inputPorts = Grid.GetUsedExternalInputs();
            foreach (var port in inputPorts)
            {
                var inputLightVector = GetLightVector(port.Input.LaserType);
                // go through the whole grid and send all 
                AssignLightToComponentViews(inputLightVector, port.Input.LaserType);
            }
        }

        private void AssignLightToComponentViews(Dictionary<Guid, Complex> lightVector, LaserType laserType)
        {
            List<Component> components = Grid.GetAllComponents();
            foreach (var componentModel in components)
            {
                try
                {
                    var componentView = GridComponentViews[componentModel.GridXMainTile, componentModel.GridYMainTile];
                    List<LightAtPin> lightAtPins = CalculateLightAtPins(lightVector, laserType, componentModel);
                    componentView.DisplayLightVector(lightAtPins);
                }
                catch (Exception ex)
                {
                    Logger.PrintErr(ex.Message);
                }

            }
        }

        public static List<LightAtPin> CalculateLightAtPins(Dictionary<Guid, Complex> lightVector, LaserType laserType, Component componentModel)
        {
            List<LightAtPin> lightAtPins = new();

            for (int offsetX = 0; offsetX < componentModel.WidthInTiles; offsetX++)
            {
                for (int offsetY = 0; offsetY < componentModel.HeightInTiles; offsetY++)
                {
                    var part = componentModel.GetPartAt(offsetX, offsetY);
                    foreach (var localSide in Enum.GetValues(typeof(RectSide)).OfType<RectSide>())
                    {
                        var pin = part.GetPinAt(localSide);
                        if (pin == null) continue;
                        var lightFlow = new LightAtPin(
                            offsetX,
                            offsetY,
                            localSide,
                            laserType,
                            lightVector.TryGetVal(pin.IDInFlow),
                            lightVector.TryGetVal(pin.IDOutFlow)
                            );
                        lightAtPins.Add(lightFlow);
                    }
                }
            }

            return lightAtPins;
        }

        public void HideLightPropagation()
        {
            try
            {
                for (int x = 0; x < Grid.Width; x++)
                {
                    for (int y = 0; y < Grid.Height; y++)
                    {
                        GridComponentViews[x, y]?.HideLightVector();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.PrintErr(ex.Message);
            }

        }
    }
}
