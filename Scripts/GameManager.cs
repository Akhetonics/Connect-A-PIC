using CAP_Core;
using CAP_Core.ExternalPorts;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel;
using Godot;
using System.Collections.Generic;

namespace ConnectAPic.LayoutWindow
{
	public partial class GameManager : Node
	{
		[Export] public NodePath GridViewPath { get; set; }
		[Export] public int FieldWidth { get; set; } = 24;

		[Export] public int FieldHeight { get; set; } = 12;
		[Export] public TextureRect ExternalOutputTemplate { get; set; }
		[Export] public TextureRect ExternalInputRedTemplate { get; set; }
		[Export] public TextureRect ExternalInputGreenTemplate { get; set; }
		[Export] public TextureRect ExternalInputBlueTemplate { get; set; }
		public static int TilePixelSize { get; private set; } = 62;
		public GridView GridView { get; set; }
		public Grid Grid { get; set; }
		public static GameManager instance;
		public GridViewModel GridViewModel { get; private set; }
		public static GameManager Instance
		{
			get { return instance; }
		}

		public override void _Ready()
		{
			if (instance == null)
			{
				instance = this;
				GridView = GetNode<GridView>(GridViewPath);
				Grid = new Grid(FieldWidth, FieldHeight);
				GridViewModel = new GridViewModel(GridView, Grid);
				GridView.Initialize(GridViewModel);
				InitializeExternalPortViews(Grid.ExternalPorts);
			}
			else
			{
				QueueFree(); // delete this object as there is already another GameManager in the scene
			}
		}

		private void InitializeExternalPortViews(List<ExternalPort> StandardPorts)
		{
			ExternalInputRedTemplate.Visible = false;
			ExternalInputGreenTemplate.Visible = false;
			ExternalInputBlueTemplate.Visible = false;
			ExternalOutputTemplate.Visible = false;

			foreach (var port in StandardPorts)
			{
				TextureRect view;
				if (port is StandardInput input)
				{
					if (input.Color == LightColor.Red)
					{
						view = (TextureRect)ExternalInputRedTemplate.Duplicate();
					}
					else if (input.Color == LightColor.Green)
					{
						view = (TextureRect)ExternalInputGreenTemplate.Duplicate();
					}
					else
					{
						view = (TextureRect)ExternalInputBlueTemplate.Duplicate();
					}
				}
				else
				{
					view = (TextureRect)ExternalOutputTemplate.Duplicate();
				}
				view.Visible = true;
				GridViewModel.GridView.DragDropProxy.AddChild(view);
				view.Position = new Vector2(view.Position.X - GridView.GlobalPosition.X, (GameManager.TilePixelSize) * port.TilePositionY);
			}
		}
	}
}
