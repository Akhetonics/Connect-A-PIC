using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using Model;
using System;
using System.ComponentModel;
using System.Drawing;
using Tiles;

namespace ConnectAPIC.LayoutWindow.View
{

	public partial class GridView : GridContainer
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
				// recalculate SMatrix in Model
				// calculate the SMatrices for the exponent 1-20 and then sum up the vectors 

				// assign a value to every Pin
				// display the Values here.
			}

			// es soll nun ein shader ueber alle tiles gelegt werden 
			// Eine Animation mit den passenden Bildern soll ueber jedem Tile dargestellt sein
			// Der Alphakanal der Animationsbilder soll der Lichtintensitaet entsprechen, oder?
			// Light-flow-richtung kann ignoriert werden
			// animations-folge soll ge-offsettet werden nach der Phase des lichtes.

		}
		

	}
}
