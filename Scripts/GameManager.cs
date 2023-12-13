using CAP_Core;
using CAP_Core.Component.ComponentHelpers;
using CAP_Core.ExternalPorts;
using CAP_Core.LightFlow;
using CAP_DataAccess;
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
using YamlDotNet.Core;

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
		[Export] public Console InGameConsole { get; set; }
		private PCKLoader PCKLoader { get; set; }
		public static int TilePixelSize { get; private set; } = 62;
		public static int TileBorderLeftDown { get; private set; } = 2;
		public GridView GridView { get; set; }
		public System.Version Version => Assembly.GetExecutingAssembly().GetName().Version; // Get the version from the assembly
		public Grid Grid { get; set; }
		public static GameManager instance;
		public CAP_Core.Logger Logger { get; set; }
		public LogSaver LogSaver { get; set; }
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
				try
				{
					instance = this;
					InitializeLoggingSystemAndConsole();
					InitializeGridAndGridView();
					InitializeExternalPortViews(Grid.ExternalPorts);
					PCKLoader = new(ComponentFolderPath, Logger);
					CallDeferred(nameof(DeferredInitialization));
				} catch (Exception ex)
				{
					GD.PrintErr(ex.Message);
					GD.PrintErr(ex);
				}		
			}
			else
			{
				QueueFree(); // delete this object as there is already another GameManager in the scene
			}
		}
		private void InitializeLoggingSystemAndConsole()
		{
			Logger = new CAP_Core.Logger();
			LogSaver = new LogSaver(Logger);
			this.CheckForNull(x => x.InGameConsole);
			InGameConsole.Initialize(Logger);
			Logger.AddLog(CAP_Contracts.Logger.LogLevel.Debug, "Initialized LoggingSystem");
			Logger.AddLog(CAP_Contracts.Logger.LogLevel.Debug, "Program Version: " + Version);
		}

		private void InitializeToolBox(ComponentViewFactory componentViewFactory)
		{
			this.CheckForNull(x => x.ToolBoxPath);
			ToolBox = GetNode<ToolBox>(ToolBoxPath);
			this.CheckForNull(x => x.ToolBox);
			ToolBox.SetAvailableTools(componentViewFactory, Logger);
			Logger?.AddLog(CAP_Contracts.Logger.LogLevel.Debug, "Initialized ToolBox");
		}
		private void InitializeGridAndGridView()
		{
			GridView = GetNode<GridView>(GridViewPath);
			this.CheckForNull(x => GridView);
			Grid = new Grid(FieldWidth, FieldHeight);
			GridViewModel = new GridViewModel(GridView, Grid, Logger);
			GridView.Initialize(GridViewModel, Logger);
			Logger?.AddLog(CAP_Contracts.Logger.LogLevel.Debug, "Initialized GridView and Grid and GridViewModel");
		}

		private void DeferredInitialization()
		{
			try
			{
				PCKLoader.LoadStandardPCKs();
				List<ComponentDraft> componentDrafts = EquipViewComponentFactoryWithJSONDrafts();
				InitializeToolBox(this.GridView.ComponentViewFactory);
				List<Component> modelComponents = new ComponentDraftConverter(Logger).ToComponentModels(componentDrafts);
				ComponentFactory.Instance.InitializeComponentDrafts(modelComponents);
				Logger?.AddLog(CAP_Contracts.Logger.LogLevel.Debug, "Initialized ComponentDrafts");
			} catch (Exception ex)
			{
				Logger?.AddLog(CAP_Contracts.Logger.LogLevel.Error, ex.Message);
			}
			
		}

		private List<ComponentDraft> EquipViewComponentFactoryWithJSONDrafts()
		{
			var draftsAndErrors = new ComponentJSONFinder(new DirectoryAccessGodot(), new DataAccessorGodot())
				.ReadComponentJSONDrafts(ComponentFolderPath);
			var componentDrafts = draftsAndErrors
				.Where(d => String.IsNullOrEmpty(d.error) == true)
				.Select(d => d.draft)
				.ToList();
			LogComponentLoadingErrors(draftsAndErrors);
			GridView.ComponentViewFactory.InitializeComponentDrafts(componentDrafts, Logger);
			return componentDrafts;
		}

		private void LogComponentLoadingErrors(List<(ComponentDraft draft, string error)> draftsAndErrors)
		{
			draftsAndErrors = draftsAndErrors.Where(d => String.IsNullOrEmpty(d.error) == false).ToList();
			foreach (var d in draftsAndErrors)
				Logger.PrintErr(d.error);
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
