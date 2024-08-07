using CAP_Contracts.Logger;
using CAP_Core.Components;
using CAP_Core.ExternalPorts;
using CAP_Core.LightCalculation;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using ConnectAPIC.Scripts.Helpers;
using ConnectAPIC.Scripts.ViewModel;
using ConnectAPIC.Scripts.ViewModel.CommandFactory;
using ConnectAPIC.Scripts.ViewModel.Commands;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                Exception ex = (Exception)args.ExceptionObject;
                Logger.PrintErr("Unhandled Exception: " + ex.Message);
            };

            this.CheckForNull(x => DragDropProxy, Logger);
            this.CheckForNull(x => ComponentViewFactory, Logger);
            this.CheckForNull(x => LightOnTexture, Logger);
            this.CheckForNull(x => LightOffTexture, Logger);
            this.CheckForNull(x => LightOnButton, Logger);
        }
        public void Initialize(GridViewModel viewModel, ILogger logger)
        {
            this.ViewModel = viewModel;
            this.Logger = logger;
            DragDropProxy.Initialize(this, viewModel, logger);
            viewModel.PropertyChanged += async (object sender, System.ComponentModel.PropertyChangedEventArgs e) =>
            {
                if (e.PropertyName != nameof(GridViewModel.IsLightOn))
                    return;
               
                if (ViewModel.IsLightOn == true)
                {
                    await ShowLightPropagation();
                }
                else
                {
                    await HideLightPropagation();
                }

                SetLightButtonOn(ViewModel.IsLightOn);
            };
            viewModel.ComponentCreated += async (Component component, int gridX, int gridY) =>
            {
                var cmpViewModel = new ComponentViewModel(component, viewModel);
                CreateComponentView(cmpViewModel, gridX, gridY, component.Rotation90CounterClock, component.TypeNumber, component.GetAllSliders());
                await RecalculateLightIfOn();
            };
            viewModel.ComponentRemoved += async (Component component, int gridX, int gridY) =>
            {
                ResetTilesAt(gridX, gridY, component.WidthInTiles, component.HeightInTiles);
                await RecalculateLightIfOn();
            };
            viewModel.LightCalculator.LightCalculationUpdated += (object sender, LightCalculationUpdated e)
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
            foreach (var componentModel in ViewModel.Grid.TileManager.GetAllComponents())
            {
                var componentView = GridComponentViews[componentModel.GridXMainTile, componentModel.GridYMainTile];
                if (componentView == null) return;
                List<LightAtPin> lightAtPins = LightCalculationHelpers.ConvertToLightAtPins(lightFieldVector, laserType, componentModel);
                componentView.ViewModel.DisplayLightVector(lightAtPins);
            }
        }

        public ComponentView CreateComponentView(ComponentViewModel cmpViewModel, int gridX, int gridY, DiscreteRotation rotationCounterClockwise, int componentTypeNumber, List<Slider> slidersInUse)
        {
            var ComponentView = ComponentViewFactory.CreateComponentView(componentTypeNumber, cmpViewModel);

            ComponentView.ViewModel.RegisterInGrid(ViewModel.Grid, gridX, gridY, rotationCounterClockwise);
            ComponentView.ViewModel.SliderModelChanged += async (int sliderNumber, double newVal) =>
            {
                await RecalculateLightIfOn();
            };
            RegisterComponentViewInGridView(ComponentView);
            DragDropProxy.AddChild(ComponentView); // it has to be the child of the DragDropArea to be displayed
                                                   // set sliders initial values
            SetInitialSliderValueInVM(slidersInUse, ComponentView);

            return ComponentView;
        }

        private static void SetInitialSliderValueInVM(List<Slider> slidersInUse, ComponentView ComponentView)
        {
            slidersInUse.ForEach(s =>
            {
                var vmSlider = ComponentView.ViewModel.SliderData.Single(data => data.Number == s.Number);
                vmSlider.MinVal = s.MinValue;
                vmSlider.MaxVal = s.MaxValue;
                vmSlider.Value = s.Value;
            });
        }

        public void SetLightButtonOn(bool isLightButtonOn)
        {
            LightOnButton.SetPressedNoSignal(isLightButtonOn);
            SetLightButtonIcon(isLightButtonOn);
            Logger.Print(LightOnButton.ButtonPressed.ToString());
        }

        public override void _Input(InputEvent @event)
        {
            if (@event.IsActionPressed("ui_undo"))
            {
                _on_btn_undo_pressed();
                @event.Dispose(); // this event should not be propagated any further
            }
            else if (@event.IsActionPressed("ui_redo"))
            {
                _on_btn_redo_pressed();
                @event.Dispose();
            }
        }

        private void _on_btn_export_nazca_pressed()
        {
            OpenSaveDialog(this);
        }
        public void OpenSaveDialog(Node node)
        {
            SaveFileDialog.Save(node, path =>
            {
                try
                {
                    var command = ViewModel.CommandFactory.CreateCommand(CommandType.ExportNazca) as ExportNazcaCommand;
                    command.Executed += (s, e) =>
                    {
                        DisplayNotificationAccordingly(s, e, node);
                        command.ClearErrors();
                    };
                    command.ExecuteAsync(new ExportNazcaParameters(path));
                }
                catch (Exception ex)
                {
                    NotificationManager.Instance.Notify($"{ex.Message}", true);
                    Logger.PrintErr(ex.Message);
                }
            });
        }

        private void DisplayNotificationAccordingly(object sender, EventArgs arg, Node node)
        {
            var args = arg as ExecutionResult;
            if (args.Errors.Count > 0)
            {
                NotificationManager.Instance.Notify("Couldn't export pdk", true);
                foreach (var error in args.Errors)
                {
                    Logger.PrintErr(error.Message);
                }
                OpenSaveDialog(node);
            }
            else
            {
                NotificationManager.Instance.Notify("Successfully saved file");
            }
        }

        private void _on_btn_undo_pressed()
        {
            ViewModel.CommandFactory.Undo();
        }
        private void _on_btn_redo_pressed()
        {
            ViewModel.CommandFactory.Redo();
        }
        private void _on_btn_show_light_propagation_toggled(bool button_pressed)
        {
            ViewModel.CommandFactory.CreateCommand(CommandType.SwitchOnLight).ExecuteAsync(button_pressed).Wait();
            SetLightButtonIcon(button_pressed);
        }


        private void _on_btn_save_pressed()
        {
            SaveFileDialog.Save(this, async path =>
            {
                try
                {
                    await ViewModel.CommandFactory.CreateCommand(CommandType.SaveGrid).ExecuteAsync(new SaveGridParameters(path));
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
                    await ViewModel.CommandFactory.CreateCommand(CommandType.LoadGrid).ExecuteAsync(new LoadGridParameters(path));
                    NotificationManager.Instance.Notify("Successfully loaded file");
                }
                catch (Exception ex)
                {
                    NotificationManager.Instance.Notify($"{ex.Message}", true);
                    Logger.PrintErr(ex.Message);
                }

            }, GridSaveFileExtensionPatterns);
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

        private void SetLightButtonIcon(bool isLightOn)
        {
            if (isLightOn)
            {
                LightOnButton.Icon = LightOnTexture;
            }
            else
            {
                LightOnButton.Icon = LightOffTexture;
            }
        }

    }
}
