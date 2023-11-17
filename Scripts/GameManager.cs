using CAP_Core;
using CAP_Core.Component.ComponentHelpers;
using CAP_Core.ExternalPorts;
using CAP_Core.LightFlow;
using Components.ComponentDraftMapper;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.Scripts.Helpers;
using ConnectAPIC.Scripts.ViewModel.ComponentDraftMapper;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace ConnectAPic.LayoutWindow
{
	public partial class GameManager : Node
	{
		[Export] public NodePath GridViewPath { get; set; }
		private ToolBox ToolBox { get; set; }
		[Export] private NodePath ToolBoxPath { get; set; }
		[Export] public int FieldWidth { get; set; } = 24;

		[Export] public int FieldHeight { get; set; } = 12;
		[Export] public TextureRect ExternalOutputTemplate { get; set; }
		[Export] public TextureRect ExternalInputRedTemplate { get; set; }
		[Export] public TextureRect ExternalInputGreenTemplate { get; set; }
		[Export] public TextureRect ExternalInputBlueTemplate { get; set; }
		private ComponentImporter ComponentImporter { get; set; }
		public static int TilePixelSize { get; private set; } = 62;
		public static int TileBorderLeftDown { get; private set; } = 2;
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
				this.CheckForNull(x => GridView);
				Grid = new Grid(FieldWidth, FieldHeight);
				GridViewModel = new GridViewModel(GridView, Grid);
				GridView.Initialize(GridViewModel);
				InitializeExternalPortViews(Grid.ExternalPorts);
				this.CheckForNull(x => x.ToolBoxPath);
				ToolBox = GetNode<ToolBox>(ToolBoxPath);
				this.CheckForNull(x => x.ToolBox);
				ComponentImporter = (ComponentImporter)this.FindChild("ComponentImporter", true, false) ?? new ComponentImporter();
				this.CheckForNull(x => x.ComponentImporter);

				CallDeferred(nameof(DeferredInitialization));
			}
			else
			{
				QueueFree(); // delete this object as there is already another GameManager in the scene
			}
		}
		
		private void DeferredInitialization()
		{
			ComponentImporter.ImportInternalPCKFiles();
			var componentDrafts = ComponentImporter.ReadComponentJSONDrafts();
			GridView.ComponentViewFactory.InitializeComponentDrafts(componentDrafts);
			ToolBox.SetAvailableTools(GridView.ComponentViewFactory);
			List<Component> modelComponents = new ComponentDraftConverter(CustomLogger.inst).ToComponentModels(componentDrafts);
			ComponentFactory.Instance.InitializeComponentDrafts(modelComponents);
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
				view.Position = new Godot.Vector2(view.Position.X - GridView.GlobalPosition.X, (GameManager.TilePixelSize) * port.TilePositionY);
			}
		}
	}
}
