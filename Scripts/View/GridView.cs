using CAP_Contracts.Logger;
using CAP_Core;
using CAP_Core.Components;
using CAP_Core.Components.ComponentHelpers;
using CAP_Core.ExternalPorts;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using ConnectAPIC.Scripts.Helpers;
using Godot;
using System;
using System.Collections.Generic;
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

        private GridViewModel ViewModel;
        public event EventHandler<bool> LightSwitched;
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
            LightSwitched.Invoke(this,button_pressed);
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
        public bool _CanDropData(Vector2 position, Variant data)
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
        public void _DropData(Vector2 atPosition, Variant data)
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

        public Variant _GetDragData(Vector2 position)
        {
            Vector2I GridXY = LocalToMap(position);
            return ViewModel.GridComponentViews[GridXY.X, GridXY.Y];
        }
    }
}
