using CAP_Contracts.Logger;
using CAP_Core;
using CAP_Core.Component.ComponentHelpers;
using CAP_Core.ExternalPorts;
using CAP_Core.LightFlow;
using CAP_DataAccess;
using CAP_DataAccess.Components.ComponentDraftMapper;
using CAP_DataAccess.Helpers;
using Chickensoft.AutoInject;
using Components.ComponentDraftMapper;
using Components.ComponentDraftMapper.DTOs;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.Scripts.Helpers;
using ConnectAPIC.Scripts.View.ComponentFactory;
using Godot;
using SuperNodes.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using YamlDotNet.Core;

namespace ConnectAPic.LayoutWindow
{
	[SuperNode (typeof(Provider))]
	public partial class GameManager : Node, 
		IProvide<ToolBox> , IProvide<ILogger>, IProvide<GridView>, IProvide<Grid>, IProvide<GridViewModel>, IProvide<GameManager>,
		IProvide<Console> , IProvide<System.Version> , IProvide<ComponentViewFactory> 
    {
        #region Dependency Injection
        public override partial void _Notification(int what);
		ToolBox IProvide<ToolBox>.Value() => MainToolBox;
        ILogger IProvide<ILogger>.Value() => Logger;
		GridView IProvide<GridView>.Value() => GridView;
		Grid IProvide<Grid>.Value() => Grid;
		GridViewModel IProvide<GridViewModel>.Value() => GridViewModel;
		GameManager IProvide<GameManager>.Value() => Instance;
        Console IProvide<Console>.Value() => InGameConsole;
        ComponentViewFactory IProvide<ComponentViewFactory>.Value() => GridView.ComponentViewFactory;
        System.Version IProvide<System.Version>.Value() => Version;
        #endregion 
        
		[Export] public NodePath GridViewPath { get; set; }
		public ToolBox MainToolBox { get; set; }
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
		public ILogger Logger { get; set; }
		private LogSaver LogSaver { get; set; }
		private StringBuilder InitializaionLogs = new();
		public GridViewModel GridViewModel { get; private set; }
		public static GameManager Instance
		{
			get { return instance; }
		}
		public const string ComponentFolderPath = "res://Scenes/Components";

        public override void _EnterTree()
        {
            base._EnterTree();
            if (instance == null)
            {
                try
                {
                    instance = this;
                    InitializeLoggingSystemAndConsole();
                    InitializaionLogs.AppendLine("Program Version: " + Version);
                    InitializeGridAndGridView();
                    InitializeExternalPortViews(Grid.ExternalPorts);
                    PCKLoader = new(ComponentFolderPath, Logger);
                }
                catch (Exception ex)
                {
                    GD.PrintErr(ex.Message);
                    GD.PrintErr(ex);
					InitializaionLogs.AppendLine(ex.Message);
                }
            }
            else
            {
                QueueFree(); // delete this object as there is already another GameManager in the scene
            }
        }
		
        public override void _Ready()
		{
            try
            {
                PCKLoader.LoadStandardPCKs();
                List<ComponentDraft> componentDrafts = EquipViewComponentFactoryWithJSONDrafts();
                this.CheckForNull(x => x.ToolBoxPath);
                MainToolBox = GetNode<ToolBox>(ToolBoxPath);
                this.CheckForNull(x => x.MainToolBox);
                List<Component> modelComponents = new ComponentDraftConverter(Logger).ToComponentModels(componentDrafts);
                ComponentFactory.Instance.InitializeComponentDrafts(modelComponents);
                InitializaionLogs.AppendLine("Initialized ComponentDrafts");
            }
            catch (Exception ex)
            {
                InitializaionLogs.AppendLine(ex.Message);
            }
			// Provide all DI Objects -> might throw if types don't match up
			try
			{
                Provide();
			}
			catch (Exception ex)
			{
				GD.PrintErr(ex.Message); // we don't know if the Console is set up already properly
				Logger.PrintErr(ex.Message);
			}
			Logger.Print(InitializaionLogs.ToString());
        }

		private void InitializeLoggingSystemAndConsole()
		{
			Logger = new CAP_Core.Logger();
			LogSaver = new LogSaver(Logger);
			this.CheckForNull(x => x.InGameConsole);
            InitializaionLogs.AppendLine("Initialized LoggingSystem");
		}

		
		private void InitializeGridAndGridView()
		{
			GridView = GetNode<GridView>(GridViewPath);
			this.CheckForNull(x => GridView);
			Grid = new Grid(FieldWidth, FieldHeight);
			GridViewModel = new GridViewModel(GridView, Grid, Logger);
			GridView.Initialize(GridViewModel, Logger);
            InitializaionLogs.AppendLine("Initialized GridView and Grid and GridViewModel");
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
                InitializaionLogs.AppendLine("ERR " + d.error);
		}

		private void InitializeExternalPortViews(List<ExternalPort> ExternalPorts)
		{
			ExternalInputRedTemplate.Visible = false;
			ExternalInputGreenTemplate.Visible = false;
			ExternalInputBlueTemplate.Visible = false;
			ExternalOutputTemplate.Visible = false;
			foreach (var port in ExternalPorts)
			{
				TextureRect view;
				if (port is ExternalInput input)
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
