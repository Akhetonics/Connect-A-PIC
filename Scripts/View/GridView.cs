using CAP_Contracts.Logger;
using CAP_Core;
using CAP_Core.Components;
using CAP_Core.Components.ComponentHelpers;
using CAP_Core.ExternalPorts;
using CAP_Core.LightCalculation;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using ConnectAPIC.Scripts.Helpers;
using ConnectAPIC.Scripts.View.ComponentViews;
using ConnectAPIC.Scripts.ViewModel.Commands;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace ConnectAPIC.LayoutWindow.View
{

    public partial class GridView : TileMap
    {

        [Export] public DragDropProxy DragDropProxy;
        [Export] public ComponentViewFactory ComponentViewFactory;
        [Export] public Texture2D LightOnTexture;
        [Export] private Texture2D LightOffTexture;
        [Export] private Button LightOnButton;

        public GridViewModel ViewModel;
        public ComponentView[,] GridComponentViews { get; private set; }
        public const string GridSaveFileExtensionPatterns = "*.pic";

        public ILogger Logger { get; private set; }

        public override void _Ready()
        {
            base._Ready();
            this.CheckForNull(x => DragDropProxy);
            this.CheckForNull(x => ComponentViewFactory);
            this.CheckForNull(x => LightOnTexture);
            this.CheckForNull(x => LightOffTexture);
            this.CheckForNull(x => LightOnButton);
        }
        public void Initialize(GridViewModel viewModel, ILogger logger)
        {
            this.ViewModel = viewModel;
            this.Logger = logger;
            DragDropProxy.OnGetDragData += _GetDragData;
            DragDropProxy.OnCanDropData += _CanDropData;
            DragDropProxy.OnDropData += _DropData;
            DragDropProxy.Initialize(viewModel.Width, viewModel.Height);
            DragDropProxy.InputReceived += (object sender, InputEvent e) => {
            };
            viewModel.PropertyChanged += async (object sender, System.ComponentModel.PropertyChangedEventArgs e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(GridViewModel.IsLightOn):
                        if (ViewModel.IsLightOn == true)
                        {
                            await ShowLightPropagation();
                        }
                        else
                        {
                            await HideLightPropagation();
                        }
                        break;
                }
            };
            viewModel.ComponentCreated += async (Component component, int gridX, int gridY) =>
            {
                CreateComponentView(gridX, gridY, component.Rotation90CounterClock, component.TypeNumber, component.GetAllSliders());
                await RecalculateLightIfOn();
            };
            viewModel.ComponentRemoved += async (Component component, int gridX, int gridY) => {
                ResetTilesAt(gridX, gridY, component.WidthInTiles, component.HeightInTiles);
                await RecalculateLightIfOn();
            };
            viewModel.LightCalculator.LightCalculationChanged += (object sender, LightCalculationChangeEventArgs e)
                => AssignLightFieldsToComponentViews(e.LightFieldVector, e.LaserInUse);
            this.GridComponentViews = new ComponentView[viewModel.Width, viewModel.Height];
            CreateEmptyField();
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
            for (int x = componentView.ViewModel.GridX; x < componentView.ViewModel.GridX + componentView.WidthInTiles; x++)
            {
                for (int y = componentView.ViewModel.GridY; y < componentView.ViewModel.GridY + componentView.HeightInTiles; y++)
                {
                    if (!ViewModel.IsInGrid(x, y, 1, 1)) continue;
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
                    if (!ViewModel.IsInGrid(x, y, 1, 1)) continue;
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
        private void AssignLightFieldsToComponentViews(Dictionary<Guid, Complex> lightFieldVector, LaserType laserType)
        {
            foreach (var componentModel in ViewModel.Grid.GetAllComponents())
            {
                var componentView = GridComponentViews[componentModel.GridXMainTile, componentModel.GridYMainTile];
                if (componentView == null) return;
                List<LightAtPin> lightAtPins = LightCalculationHelpers.ConvertToLightAtPins(lightFieldVector, laserType, componentModel);
                componentView.ViewModel.DisplayLightVector(lightAtPins);
            }
        }

        public ComponentView CreateComponentView(int gridX, int gridY, DiscreteRotation rotationCounterClockwise, int componentTypeNumber, List<Slider> slidersInUse)
        {
            var ComponentView = ComponentViewFactory.CreateComponentView(componentTypeNumber);
            ComponentView.ViewModel.RegisterInGrid(ViewModel.Grid, gridX, gridY, rotationCounterClockwise);
            ComponentView.ViewModel.SliderChanged += async (int sliderNumber, double newVal) => {
                await ViewModel.MoveSliderCommand.ExecuteAsync(new MoveSliderCommandArgs(ComponentView.ViewModel.GridX, ComponentView.ViewModel.GridY, sliderNumber, newVal));
                await RecalculateLightIfOn();
            };
            RegisterComponentViewInGridView(ComponentView);
            DragDropProxy.AddChild(ComponentView); // it has to be the child of the DragDropArea to be displayed
                                                            // set sliders initial values
            List<SliderViewData> SliderInitialData = slidersInUse.Select(s => {
                var vmSlider = ComponentView.ViewModel.SliderData.Single(data => data.Number == s.Number);
                return new SliderViewData(
                    vmSlider.GodotSliderLabelName, vmSlider.GodotSliderName, s.MinValue, s.MaxValue, s.Value, vmSlider.Steps, s.Number);
            }).ToList();
            foreach (var slider in SliderInitialData)
            {
                ComponentView.ViewModel.SetSliderValue(slider.Number, slider.Value, true);
            }
            return ComponentView;
        }

        public void SetLightButtonOn(bool isLightButtonOn)
        {
            LightOnButton.ButtonPressed = isLightButtonOn;
        }

        private void _on_btn_export_nazca_pressed()
        {
            SaveFileDialog.Save(this, path =>
            {
                try
                {
                    ViewModel.ExportToNazcaCommand.ExecuteAsync(new ExportNazcaParameters(path));
                    NotificationManager.Instance.Notify("Successfully saved file");
                }
                catch (Exception ex)
                {
                    NotificationManager.Instance.Notify($"{ex.Message}", true);
                    Logger.PrintErr(ex.Message);
                }
            });
        }

        private void _on_btn_show_light_propagation_toggled(bool button_pressed)
        {
            ViewModel.SwitchOnLightCommand.ExecuteAsync(button_pressed).Wait();
            if (button_pressed)
            {
                LightOnButton.Icon = LightOnTexture;
            }
            else
            {
                LightOnButton.Icon = LightOffTexture;
            }
        }
        
        private void _on_btn_save_pressed()
        {
            SaveFileDialog.Save(this, async path =>
            {
                try
                {
                    await ViewModel.SaveGridCommand.ExecuteAsync(new SaveGridParameters(path));
                    NotificationManager.Instance.Notify("Successfully saved file");
                }
                catch (Exception ex)
                {
                    NotificationManager.Instance.Notify($"{ex.Message}", true);
                    Logger.PrintErr(ex.Message);
                }
            }, GridSaveFileExtensionPatterns);

        }
        private void _on_btn_load_pressed()
        {
            SaveFileDialog.Open(this, async path =>
            {
                try
                {
                    await ViewModel.LoadGridCommand.ExecuteAsync(new LoadGridParameters(path));
                    NotificationManager.Instance.Notify("Successfully loaded file");
                }
                catch (Exception ex)
                {
                    NotificationManager.Instance.Notify($"{ex.Message}", true);
                    Logger.PrintErr(ex.Message);
                }

            }, GridSaveFileExtensionPatterns);
        }
        public bool _CanDropData(Godot.Vector2 position, Variant data)
        {
            if (data.Obj is ComponentView component)
            {
                int gridX = (int)position.X / GameManager.TilePixelSize;
                int gridY = (int)position.Y / GameManager.TilePixelSize;
                Component model = null;
                if (component.ViewModel.IsPlacedInGrid)
                {
                    model = ViewModel.Grid.GetComponentAt(component.ViewModel.GridX, component.ViewModel.GridY, component.WidthInTiles, component.HeightInTiles);
                }
                bool canDropData = !ViewModel.Grid.IsColliding(gridX, gridY, component.WidthInTiles, component.HeightInTiles, model);

                return canDropData;
            }

            return false;
        }
        public void _DropData(Godot.Vector2 atPosition, Variant data)
        {
            Vector2I GridXY = LocalToMap(atPosition);
            if (data.Obj is ComponentView componentView)
            {
                if (!componentView.ViewModel.IsPlacedInGrid)
                {
                    ViewModel.CreateComponentCommand.ExecuteAsync(new CreateComponentArgs(componentView.ViewModel.TypeNumber, GridXY.X, GridXY.Y, (DiscreteRotation)(componentView.RotationDegrees / 90)));
                }
                else
                {
                    ViewModel.MoveComponentCommand.ExecuteAsync(new MoveComponentArgs(componentView.ViewModel.GridX, componentView.ViewModel.GridY, GridXY.X, GridXY.Y));
                }
            }
        }
        public async Task ShowLightPropagation() =>
            await ViewModel.LightCalculator.ShowLightPropagationAsync();

        public async Task HideLightPropagation()
        {
            await ViewModel.LightCalculator.CancelLightCalculation();

            for (int x = 0; x < ViewModel.Width; x++)
            {
                for (int y = 0; y < ViewModel.Height; y++)
                {
                    GridComponentViews[x, y]?.HideLightVector();
                }
            }
        }
        public async Task RecalculateLightIfOn()
        {
            if (ViewModel.IsLightOn)
            {
                await ShowLightPropagation();
            }
        }
        public Variant _GetDragData(Godot.Vector2 position)
        {
            Vector2I GridXY = LocalToMap(position);
            return GridComponentViews[GridXY.X, GridXY.Y];
        }
    }
}
