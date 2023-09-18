using ConnectAPIC.LayoutWindow.Model.ExternalPorts;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel;
using Godot;
using Model;
using System;
using System.Collections.Generic;
using Tiles;

namespace ConnectAPic.LayoutWindow
{
	public partial class GameManager : Node
	{
		[Export] public NodePath GridViewPath { get; set; }
		[Export] public int FieldWidth { get; set; } = 24;

		[Export] public int FieldHeight { get; set; } = 12;
		[Export] public TextureRect ExternalOutputTemplate{get;set;}
		[Export] public TextureRect ExternalInputRedTemplate{get;set;}
		[Export] public TextureRect ExternalInputGreenTemplate{get;set;}
		[Export] public TextureRect ExternalInputBlueTemplate{get;set;}
		[Export] public Control Background{ get; set; }
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
			foreach (var port in StandardPorts)
			{
				TextureRect view;
				if (port is StandardInput input)
				{
					if(input.Color == LightColor.Red)
					{
						view = ExternalInputRedTemplate.Duplicate() as TextureRect;
					} else if(input.Color == LightColor.Green)
					{
						view = ExternalInputGreenTemplate.Duplicate() as TextureRect;
					}
					else
					{
						view = ExternalInputBlueTemplate.Duplicate() as TextureRect;
					}
				} else
				{
					view = ExternalOutputTemplate.Duplicate() as TextureRect;
				}
				view.Visible = true;
				this.AddChild(view);
				view.Position = new Vector2(view.Position.X , GridView.GlobalPosition.Y+ (TileView.TilePixelSize) * port.TilePositionY);
			}
		}
	}
}
