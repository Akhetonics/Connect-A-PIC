using Antlr4.Runtime.Misc;
using CAP_Contracts.Logger;
using CAP_Core.Components;
using CAP_Core.ExternalPorts;
using CAP_Core.Grid;
using CAP_Core.Helpers;
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
using System.Reflection.Metadata.Ecma335;
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
        public Vector2I StartGridXY { get; private set; }

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
            DragDropProxy.OnGetDragData = _GetDragData;
            DragDropProxy.OnCanDropData = _CanDropData;
            DragDropProxy.OnDropData = _DropData;
            DragDropProxy.Initialize(viewModel.Width, viewModel.Height);
            DragDropProxy.InputReceived += (object sender, InputEvent e) => {
                if(e is InputEventMouseButton eventMouseButton)
                {
                    if(eventMouseButton.ButtonIndex == MouseButton.Left)
                    {
                        if(eventMouseButton.Pressed == false)
                        {
                            ClearDragPreview();
                        }
                    }
                }
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
            foreach (var componentModel in ViewModel.Grid.TileManager.GetAllComponents())
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
            bool canDropData = false;
            var deltaGridXY = (LocalToMap(position) - StartGridXY).ToIntVector();
            var args = ConvertGodotListToMoveComponentArgs(data, deltaGridXY);
            if (args != null)
            {
                canDropData = ViewModel.MoveComponentCommand.CanExecute(args);
            }

            if (canDropData == true && data.Obj is Godot.Collections.Array transitions)
            {
                ShowComponentDragPreview(position, transitions, canDropData);
            }
            
            if (canDropData == false)
            {
                ClearDragPreview();
            }
            return canDropData;
        }
        public void _DropData(Godot.Vector2 atPosition, Variant data)
        {
            var deltaGridXY = (LocalToMap(atPosition) - StartGridXY).ToIntVector();
            var args = ConvertGodotListToMoveComponentArgs(data, deltaGridXY);
            if (args != null)
            {
                ViewModel.MoveComponentCommand.ExecuteAsync(args).Wait();
            }
            ClearDragPreview();
        }

        private static MoveComponentArgs ConvertGodotListToMoveComponentArgs(Variant data, IntVector deltaGridXY)
        {
            if (data.Obj is Godot.Collections.Array componentPositionsVariant)
            {
                List<(IntVector Source, IntVector Target)> translations = new();
                foreach (var componentPositionVariant in componentPositionsVariant)
                {
                    if (componentPositionVariant.VariantType != Variant.Type.Vector2I) continue;
                    var componentPosition = (Vector2I)componentPositionVariant;
                    var source = (componentPosition).ToIntVector();
                    translations.Add((source, source + deltaGridXY));
                }
                return new MoveComponentArgs(translations);
            }
            return null;
        }

        public void ShowComponentDragPreview(Godot.Vector2 position, Godot.Collections.Array transitions, bool canDropData )
        {

            var deltaGridXY = (LocalToMap(position) - StartGridXY);
            Color previewColor = canDropData ? new Color(0.5f, 1, 0.5f) : new Color(1, 0, 0); // Light green for valid, red for invalid.
            Logger.Print("candropData: " + canDropData);
            foreach (Variant componentPositionVariant in transitions)
            {
                if (componentPositionVariant.VariantType != Variant.Type.Vector2I) continue;
                var componentGridPosition = (Vector2I)componentPositionVariant;
                var targetGridPosition = componentGridPosition + deltaGridXY;

                // Assuming CreatePreviewComponent is a method that creates a visual representation of the component.
                CreateOrUpdatePreviewComponent(componentGridPosition, targetGridPosition, previewColor);
            }
        }
        private Dictionary<Vector2I, ComponentView> previewComponents = new Dictionary<Vector2I, ComponentView>();

        public void CreateOrUpdatePreviewComponent(Vector2I originalGridPosition, Vector2I targetGridPosition, Color color)
        {
            ComponentView previewComponent;
            if (previewComponents.TryGetValue(originalGridPosition, out previewComponent))
            {
                // Preview component already exists, update its position and color
                previewComponent.Position = MapToLocalCorrected(targetGridPosition); // Assuming a method to convert grid position to local position
                previewComponent.Modulate = color;
                previewComponent.Show();
            }
            else
            {
                // Create a new preview component
                var brush = GridComponentViews[targetGridPosition.X, targetGridPosition.Y];
                if (brush == null) 
                    return;
                previewComponent = brush.Duplicate(); // Or however you instantiate your preview
                previewComponent.Position = MapToLocalCorrected(targetGridPosition);
                previewComponent.Modulate = color;
                previewComponent.MouseFilter = Control.MouseFilterEnum.Ignore;
                previewComponent.GetChildren();
                foreach(var child in previewComponent.GetChildren() )
                {
                    if(child is Control node){
                        node.MouseFilter = Control.MouseFilterEnum.Ignore;
                    }
                }
                AddChild(previewComponent); // Add to the scene

                previewComponents[originalGridPosition] = previewComponent;
            }
        }

        private Godot.Vector2 MapToLocalCorrected(Vector2I targetGridPosition)
        {
            var tileSize = new Godot.Vector2(GameManager.TilePixelSize, GameManager.TilePixelSize);
            return MapToLocal(targetGridPosition) - 0.5f * tileSize;
        }

        public void ClearDragPreview()
        {
            foreach (var entry in previewComponents)
            {
                // Optionally, instead of freeing, you could hide them or reuse them later
                entry.Value.QueueFree();
            }
            previewComponents.Clear();
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
            StartGridXY = LocalToMap(position);
            var selectionMgr = ViewModel.SelectionGroupManager.SelectionManager;
            var selections = selectionMgr.Selections;
            var cursorComponent = GridComponentViews[StartGridXY.X, StartGridXY.Y];
            var componentLocations = new Godot.Collections.Array<Vector2I>();
            if (cursorComponent == null) return componentLocations;
            var cursorComponentPos = new Vector2I(cursorComponent.ViewModel.GridX, cursorComponent.ViewModel.GridY);
            var isComponentInSelection = selectionMgr.Selections.Contains(cursorComponentPos.ToIntVector());
            if (isComponentInSelection)
            {
                foreach (var selection in selections)
                {
                    componentLocations.Add(new Vector2I(selection.X, selection.Y));
                }
            }
            else
            {
                componentLocations.Add(cursorComponentPos);
            }
            return componentLocations;
        }
    }
}
