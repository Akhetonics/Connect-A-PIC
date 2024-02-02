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
using ConnectAPIC.Scripts.View.ComponentViews;
using ConnectAPIC.Scripts.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Numerics;
using System.Threading;
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
        public ICommand MoveSliderCommand { get; internal set; }
        public ComponentView[,] GridComponentViews { get; private set; }
        public int Width { get => GridComponentViews.GetLength(0); }
        public int Height { get => GridComponentViews.GetLength(1); }
        private CancellationTokenSource CancelTokenLightCalc = new();
        public GridManager Grid { get; set; }
        public ILogger Logger { get; }
        public ComponentFactory ComponentModelFactory { get; }
        public GridView GridView { get; set; }
        public GridSMatrixAnalyzer MatrixAnalyzer { get; private set; }
        public int MaxTileCount { get => Width * Height; }
        public bool IsLightOn { get; set; } = false;

        public GridViewModel(GridView gridView, GridManager grid, ILogger logger, ComponentFactory componentModelFactory)
        {
            this.GridView = gridView;
            this.Grid = grid;
            GridView.LightSwitched += async (sender, isOn) =>
            {
                IsLightOn = isOn;
                try
                {
                    if (isOn)
                    {
                        await ShowLightPropagationAsync();
                    }
                    else
                    {
                        await HideLightPropagation();
                    }
                }
                catch (Exception ex)
                {
                    Logger.PrintErr(ex.Message);
                }
            };
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
            MoveSliderCommand = new MoveSliderCommand(grid);
            ExportToNazcaCommand = new ExportNazcaCommand(new NazcaExporter(), grid, new DataAccessorGodot());
            CreateEmptyField();
            this.Grid.OnComponentPlacedOnTile += Grid_OnComponentPlacedOnTile;
            this.Grid.OnComponentRemoved += Grid_OnComponentRemoved;
            MatrixAnalyzer = new GridSMatrixAnalyzer(this.Grid, StandardWaveLengths.RedNM);
        }

        private async void Grid_OnComponentRemoved(Component component, int x, int y)
        {
            ResetTilesAt(x, y, component.WidthInTiles, component.HeightInTiles);
            await RecalculateLightPropagation();
        }

        private async Task RecalculateLightPropagation()
        {
            await ShowLightPropagationAsync();
        }
        private async void Grid_OnComponentPlacedOnTile(Component component, int gridX, int gridY)
        {
            CreateComponentView(gridX, gridY, component.Rotation90CounterClock, component.TypeNumber, component.GetAllSliders());
            await RecalculateLightPropagation();
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
        public ComponentView CreateComponentView(int gridX, int gridY, DiscreteRotation rotationCounterClockwise, int componentTypeNumber, List<Slider> slidersInUse)
        {
            var ComponentView = GridView.ComponentViewFactory.CreateComponentView(componentTypeNumber);
            ComponentView.RegisterInGrid(gridX, gridY, rotationCounterClockwise, this);
            ComponentView.SliderChanged += async (ComponentView view, Godot.Slider godotSlider, double newVal) =>
            {
                await MoveSliderCommand.ExecuteAsync(new MoveSliderCommandArgs(view.GridX, view.GridY, (int)godotSlider.GetMeta(ComponentView.SliderNumberMetaID), newVal));
                await RecalculateLightPropagation();
            };
            RegisterComponentViewInGridView(ComponentView);
            GridView.DragDropProxy.AddChild(ComponentView); // it has to be the child of the DragDropArea to be displayed
            // set sliders initial values
            foreach (var slider in slidersInUse)
            {
                ComponentView.SetSliderValue(slider.Number, slider.Value);
            }
            return ComponentView;
        }
        private Task LightCalculationTask;
        public async Task ShowLightPropagationAsync()
        {
            if (IsLightOn == false) return;
            await CancelLightCalculation();
            try
            {
                var inputPorts = Grid.GetAllExternalInputs(); // go through all Inputs, so that the light can be turned off properly if e.g. red is turned off but some components have red light.
                foreach (var port in inputPorts)
                {
                    CancelTokenLightCalc.Token.ThrowIfCancellationRequested();
                    LightCalculationTask = Task.Run(async () =>
                    {
                        MatrixAnalyzer = new GridSMatrixAnalyzer(this.Grid, port.LaserType.WaveLengthInNm);
                        var resultLightVector = await MatrixAnalyzer.CalculateLightPropagationAsync(CancelTokenLightCalc);
                        AssignLightToComponentViews(resultLightVector, port.LaserType);
                    }, CancelTokenLightCalc.Token);
                    await LightCalculationTask.ConfigureAwait(false);
                }

            }
            catch (OperationCanceledException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
            finally
            {
                LightCalculationTask = null;
            }
        }

        private async Task CancelLightCalculation()
        {
            // Cancel the ongoing calculations
            CancelTokenLightCalc.Cancel();

            try
            {
                if (LightCalculationTask != null)
                {
                    await Task.WhenAll(new List<Task>() { LightCalculationTask }).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                // Alle Tasks wurden abgebrochen
            }
            // Dispose the old CancellationTokenSource and create a new one for the next calculations
            CancelTokenLightCalc.Dispose();
            CancelTokenLightCalc = new CancellationTokenSource();
        }

        private void AssignLightToComponentViews(Dictionary<Guid, Complex> lightVector, LaserType laserType)
        {
            List<Component> components = Grid.GetAllComponents();
            foreach (var componentModel in components)
            {
                try
                {
                    var componentView = GridComponentViews[componentModel.GridXMainTile, componentModel.GridYMainTile];
                    if (componentView == null) return;
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

        public async Task HideLightPropagation()
        {
            IsLightOn = false;
            GridView.SetLightButtonOn(false);
            await CancelLightCalculation();

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
