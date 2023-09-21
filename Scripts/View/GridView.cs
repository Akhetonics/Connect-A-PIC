using CAP_Core;
using CAP_Core.ExternalPorts;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using Godot;
using System;

namespace ConnectAPIC.LayoutWindow.View
{

    public partial class GridView : TileMap
	{
		public delegate void GridActionHandler(TileView tile);
		public delegate void GridActionComponentHandler(TileView tile);

		[Export] private NodePath DefaultTilePath;
		private TileView _defaultTile;
		private GridViewModel ViewModel;

		public TileView DefaultTile
		{
			get
			{
				if (_defaultTile != null) return _defaultTile;
				_defaultTile = this.GetNode<TileView>(DefaultTilePath);
				return _defaultTile;
			}
		}

		public GridView()
		{
			if (string.IsNullOrEmpty(DefaultTilePath))
			{
				GD.PrintErr($"{nameof(DefaultTilePath)} is not assigned");
			}
		}
		public void Initialize(GridViewModel viewModel)
		{
			this.ViewModel = viewModel;
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

        public override bool _CanDropData(Vector2 position, Variant data)
        {
            // extract all tiles from the component that is about to be dropped here at position and SetDragPreview them
            if (data.Obj is ComponentBaseView component)
            {
                ShowMultiTileDragPreview(position, component);
            }

            return true;
        }
        protected void ShowMultiTileDragPreview(Vector2 position, ComponentBaseView component)
        {
            var previewGrid = new GridContainer();
            previewGrid.PivotOffset = previewGrid.Size / 2f;
            var oldRotation = component.Rotation90CounterClock;
            component.Rotation90CounterClock = 0;
            previewGrid.Columns = component.WidthInTiles;
            for (int y = 0; y < component.HeightInTiles; y++)
            {
                for (int x = 0; x < component.WidthInTiles; x++)
                {
                    var previewtile = this.Duplicate();
                    previewtile._Ready();
                    previewtile.Texture = component.GetTexture(x, y).Duplicate() as Texture2D;
                    previewtile.Visible = true;
                    previewGrid.AddChild(previewtile);
                }
            }
            previewGrid.RotationDegrees = (int)oldRotation * 90;
            component.Rotation90CounterClock = oldRotation;
            this.SetDragPreview(previewGrid);
        }
        public override void _DropData(Vector2 atPosition, Variant data)
        {
            if (data.Obj is ComponentBaseView componentView)
            {
                if (!componentView.Visible)
                {
                    ViewModel.CreateComponentCommand.Execute(new CreateComponentArgs(componentView.GetType(), GridX, GridY, componentView.Rotation90CounterClock));
                }
                else
                {
                    ViewModel.MoveComponentCommand.Execute(new MoveComponentArgs(componentView.GridX, componentView.GridY, GridX, GridY));
                }
            }
        }

        public override Variant _GetDragData(Vector2 position)
        {
            return this.ComponentView;
        }

    }
}
