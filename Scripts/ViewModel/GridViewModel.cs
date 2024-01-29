using CAP_Contracts.Logger;
using CAP_Core.CodeExporter;
using CAP_Core.Components;
using CAP_Core.Components.ComponentHelpers;
using CAP_Core.Components.Creation;
using CAP_Core.ExternalPorts;
using CAP_Core.Grid;
using CAP_Core.Helpers;
using CAP_Core.Tiles;
using CAP_Core.Tiles.Grid;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using ConnectAPIC.Scripts.Helpers;
using ConnectAPIC.Scripts.View.ComponentFactory;
using ConnectAPIC.Scripts.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace ConnectAPIC.LayoutWindow.ViewModel
{
    public class GridViewModel
    {
        public ICommand CreateComponentCommand { get; set; }
        public ICommand MoveComponentCommand { get; set; }
        public ICommand DeleteComponentCommand { get; set; }
        public ICommand RotateComponentCommand { get; set; }
        public ICommand ExportToNazcaCommand { get; set; }
        public ICommand SaveGridCommand { get; internal set; }
        public ICommand LoadGridCommand { get; internal set; }
        public ComponentView[,] GridComponentViews { get; private set; }
        public int Width { get => GridComponentViews.GetLength(0); }
        public int Height { get => GridComponentViews.GetLength(1); }
        public GridManager Grid { get; set; }
        public ILogger Logger { get; }
        public ComponentFactory ComponentModelFactory { get; }
        public GridView GridView { get; set; }
        public GridSMatrixAnalyzer MatrixAnalyzer { get; private set; }
        public int MaxTileCount { get => Width * Height; }

        public GridViewModel(GridView gridView, GridManager grid, ILogger logger, ComponentFactory componentModelFactory)
        {
            this.GridView = gridView;
            this.Grid = grid;
            Logger = logger;
            this.ComponentModelFactory = componentModelFactory;
            //this.GridView.Columns = grid.Width;
            this.GridComponentViews = new ComponentView[grid.Width, grid.Height];
            CreateComponentCommand = new CreateComponentCommand(grid, componentModelFactory);
            DeleteComponentCommand = new DeleteComponentCommand(grid);
            RotateComponentCommand = new RotateComponentCommand(grid);
            MoveComponentCommand = new MoveComponentCommand(grid);
            SaveGridCommand = new SaveGridCommand(grid, new FileDataAccessor());
            LoadGridCommand = new LoadGridCommand(grid, new FileDataAccessor(), componentModelFactory, this);
            ExportToNazcaCommand = new ExportNazcaCommand(new NazcaExporter(), grid, new DataAccessorGodot());
            CreateEmptyField();
            this.Grid.OnComponentPlacedOnTile += Grid_OnComponentPlacedOnTile;
            this.Grid.OnComponentRemoved += Grid_OnComponentRemoved;
            MatrixAnalyzer = new GridSMatrixAnalyzer(this.Grid, StandardWaveLengths.RedNM);
        }

        private void Grid_OnComponentRemoved(Component component, int x, int y)
        {
            ResetTilesAt(x, y, component.WidthInTiles, component.HeightInTiles);
            RecalculateLightPropagation();
        }

        private void RecalculateLightPropagation()
        {
            if (GridView.lightPropagationIsPressed)
            {
                HideLightPropagation();
                ShowLightPropagation();
            }
        }

        private void Grid_OnComponentPlacedOnTile(Component component, int gridX, int gridY)
        {
            CreateComponentView(gridX, gridY, component.Rotation90CounterClock, component.TypeNumber);
            RecalculateLightPropagation();
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
            ComponentView.SliderChanged += (ComponentView view, Guid sliderId, double newVal) => {
                var componentModel = Grid.GetComponentAt(view.GridX,view.GridY);
                foreach (var item in componentModel.LaserWaveLengthToSMatrixMap.Values)
                {
                    item.SliderReference[sliderId]
                }
                RecalculateLightPropagation();

            };
            RegisterComponentViewInGridView(ComponentView);
            GridView.DragDropProxy.AddChild(ComponentView); // it has to be the child of the DragDropArea to be displayed
            return ComponentView;
        }

        public async Task ShowLightPropagation()
        {
            var inputPorts = Grid.GetUsedExternalInputs();
            foreach (var port in inputPorts)
            {
                MatrixAnalyzer = new GridSMatrixAnalyzer(this.Grid, port.Input.LaserType.WaveLengthInNm);
                var resultLightVector = await MatrixAnalyzer.CalculateLightPropagation();
                Logger.Print(resultLightVector.ToCustomString());
                AssignLightToComponentViews(resultLightVector, port.Input.LaserType);
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
                    List<LightAtPin> lightAtPins = ConvertToLightAtPins(lightVector, laserType, componentModel);
                    componentView.DisplayLightVector(lightAtPins);
                }
                catch (Exception ex)
                {
                    Logger.PrintErr(ex.Message);
                }

            }
        }

        public static List<LightAtPin> ConvertToLightAtPins(Dictionary<Guid, Complex> lightVector, LaserType laserType, Component componentModel)
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
