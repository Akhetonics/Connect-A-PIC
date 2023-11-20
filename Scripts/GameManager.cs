using CAP_Core;
using CAP_Core.Component.ComponentHelpers;
using CAP_Core.ExternalPorts;
using CAP_Core.LightFlow;
using CAP_Core.Logger;
using CAP_DataAccess.Components.ComponentDraftMapper;
using CAP_DataAccess.Helpers;
using Components.ComponentDraftMapper;
using Components.ComponentDraftMapper.DTOs;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.Scripts.Helpers;
using ConnectAPIC.Scripts.View.ComponentFactory;
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
		private PCKLoader PCKLoader { get; set; }
		public static int TilePixelSize { get; private set; } = 62;
		public static int TileBorderLeftDown { get; private set; } = 2;
		public GridView GridView { get; set; }
		public Grid Grid { get; set; }
		public static GameManager instance;
		public Logger LoggerModel { get; set; }
		public GridViewModel GridViewModel { get; private set; }
		public static GameManager Instance
		{
			get { return instance; }
		}
		public const string ComponentFolderPath = "res://Scenes/Components";
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
				PCKLoader = new(ComponentFolderPath);
				CallDeferred(nameof(DeferredInitialization));
			}
			else
			{
				QueueFree(); // delete this object as there is already another GameManager in the scene
			}
		}

		private void DeferredInitialization()
		{
			PCKLoader.LoadStandardPCKs();
			List<ComponentDraft> componentDrafts = EquipViewComponentFactoryWithJSONDrafts();
			ToolBox.SetAvailableTools(GridView.ComponentViewFactory);
			List<Component> modelComponents = new ComponentDraftConverter(Logger.Inst).ToComponentModels(componentDrafts);
			ComponentFactory.Instance.InitializeComponentDrafts(modelComponents);
		}

		private List<ComponentDraft> EquipViewComponentFactoryWithJSONDrafts()
		{
			var draftsAndErrors = new ComponentJSONFinder(new DirectoryAccessGodot(), new DataAccessorGodot())
				.ReadComponentJSONDrafts(ComponentFolderPath);
			var componentDrafts = draftsAndErrors
				.Where(d => String.IsNullOrEmpty(d.error) == true)
				.Select(d => d.draft)
				.ToList();
			PrintLoadingErrorsToConsole(draftsAndErrors);

			GridView.ComponentViewFactory.InitializeComponentDrafts(componentDrafts);
			return componentDrafts;
		}

		private static void PrintLoadingErrorsToConsole(List<(ComponentDraft draft, string error)> draftsAndErrors)
		{
			draftsAndErrors
				.Where(d => d.error != null)
				.ToList()
				.ForEach(d => Logger.Inst.PrintErr(d.error));
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
