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
        private GridViewModel ViewModel;
        public bool lightPropagationIsPressed;
        public const string GridSaveFileExtensionPatterns = "*.pic|*.PIC|*.txt|*.TXT";

        public ILogger Logger { get; private set; }

        public override void _Ready()
        {
            base._Ready();
            this.CheckForNull(x => DragDropProxy);
            this.CheckForNull(x => ComponentViewFactory);
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

        private void _on_btn_export_nazca_pressed()
        {
            SaveFileDialog.Save(this, path =>
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await ViewModel.ExportToNazcaCommand.ExecuteAsync(new ExportNazcaParameters(path));
                        NotificationManager.Instance.Notify("Successfully saved file");
                    }
                    catch (Exception ex)
                    {
                        NotificationManager.Instance.Notify($"{ex.Message}", true);
                        Logger.PrintErr(ex.Message);
                    }
                });
            });
        }

        private void _on_btn_show_light_propagation_toggled(bool button_pressed)
        {
            lightPropagationIsPressed = button_pressed;

            try
            {
                if (button_pressed)
                {
                    ViewModel.HideLightPropagation();
                    ViewModel.ShowLightPropagation();
                }
                else
                {
                    ViewModel.HideLightPropagation();
                }
            }
            catch (Exception ex)
            {
                Logger.PrintErr(ex.Message);
            }
        }
        private void _on_btn_save_pressed()
        {
            SaveFileDialog.Save(this, path =>
            {
                try
                {
                    ViewModel.SaveGridCommand.ExecuteAsync(new SaveGridParameters(path));
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
            SaveFileDialog.Open(this, path =>
            {
                try
                {
                    ViewModel.LoadGridCommand.ExecuteAsync(new LoadGridParameters(path));
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
                if (component.IsPlacedOnGrid())
                {
                    model = ViewModel.Grid.GetComponentAt(component.GridX, component.GridY, component.WidthInTiles, component.HeightInTiles);
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
                if (!componentView.IsPlacedOnGrid())
                {
                    ViewModel.CreateComponentCommand.ExecuteAsync(new CreateComponentArgs(componentView.TypeNumber, GridXY.X, GridXY.Y, (DiscreteRotation)(componentView.RotationDegrees / 90)));
                }
                else
                {
                    ViewModel.MoveComponentCommand.ExecuteAsync(new MoveComponentArgs(componentView.GridX, componentView.GridY, GridXY.X, GridXY.Y));
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
