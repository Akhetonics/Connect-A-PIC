using CAP_Core;
using CAP_Core.Component.ComponentHelpers;
using CAP_Core.ExternalPorts;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using Godot;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ConnectAPIC.LayoutWindow.View
{

	public partial class GridView : TileMap
	{

		[Export] public DragDropProxy DragDropProxy;
		[Export] public ComponentViewFactory ComponentViewFactory;
		private GridViewModel ViewModel;

		public void Initialize(GridViewModel viewModel)
		{
			this.ViewModel = viewModel;
			DragDropProxy.OnGetDragData += _GetDragData;
			DragDropProxy.OnCanDropData += _CanDropData;
			DragDropProxy.OnDropData+= _DropData;
			DragDropProxy.Initialize(viewModel.Width , viewModel.Height);
		}

		private void _on_btn_export_nazca_pressed()
		{
			SaveFileDialog.Open(this, path =>
			{
				try
				{
					ViewModel.ExportToNazcaCommand.Execute(new ExportNazcaParameters(path));
					NotificationManager.Instance.Notify("Successfully saved file");
				}
				catch (Exception ex)
				{
					NotificationManager.Instance.Notify($"{ex.Message}",true);
				}
			});
		}
		public bool lightPropagationIsPressed;
		private void _on_btn_show_light_propagation_toggled(bool button_pressed)
		{
            lightPropagationIsPressed = button_pressed;

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
					ViewModel.CreateComponentCommand.Execute(new CreateComponentArgs(componentView.TypeNumber, GridXY.X, GridXY.Y, (DiscreteRotation)(componentView.RotationDegrees /90)));
				}
				else
				{
					ViewModel.MoveComponentCommand.Execute(new MoveComponentArgs(componentView.GridX, componentView.GridY, GridXY.X, GridXY.Y));
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