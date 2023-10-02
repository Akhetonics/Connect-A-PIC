using CAP_Core;
using CAP_Core.Component.ComponentHelpers;
using CAP_Core.ExternalPorts;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using Godot;
using System;
using System.Collections.Generic;

namespace ConnectAPIC.LayoutWindow.View
{

	public partial class GridView : TileMap
	{

		[Export] public DragDropProxy DragDropProxy;
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
		private void _on_btn_show_light_propagation_toggled(bool button_pressed)
		{
			if (button_pressed)
			{
				var LightVectorRed = ViewModel.GetLightVector(LightColor.Red);
				var LightVectorGreen = ViewModel.GetLightVector(LightColor.Green);
				var LightVectorBlue = ViewModel.GetLightVector(LightColor.Blue);
				ViewModel.ShowLightPropagation(LightVectorRed, LightVectorGreen, LightVectorBlue);
			} else
			{
				ViewModel.HideLightPropagation();
			}
		}

		public bool _CanDropData(Vector2 position, Variant data)
		{
			if (data.Obj is ComponentBaseView component)
			{
				GridViewModel.ShowMultiTileDragPreview(position, component, DragDropProxy);
			}

			return true;
		}
		public void _DropData(Vector2 atPosition, Variant data)
		{
			Vector2I GridXY = LocalToMap(atPosition);
			if (data.Obj is ComponentBaseView componentView)
			{
				if (!componentView.Visible)
				{
					ViewModel.CreateComponentCommand.Execute(new CreateComponentArgs(componentView.GetType(), GridXY.X, GridXY.Y, (DiscreteRotation)(componentView.RotationDegrees /90)));
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
