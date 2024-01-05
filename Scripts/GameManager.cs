using CAP_Contracts.Logger;
using CAP_Core;
using CAP_Core.Components;
using CAP_Core.Components.Creation;
using CAP_Core.ExternalPorts;
using CAP_DataAccess;
using CAP_DataAccess.Components.ComponentDraftMapper;
using Chickensoft.AutoInject;
using Components.ComponentDraftMapper;
using Components.ComponentDraftMapper.DTOs;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.Scenes.ToolBox;
using ConnectAPIC.Scenes.InGameConsole;
using ConnectAPIC.Scripts.Helpers;
using ConnectAPIC.Scripts.View.ComponentFactory;
using Godot;
using SuperNodes.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConnectAPic.LayoutWindow
{
    [SuperNode (typeof(Provider))]
	public partial class GameManager : Node, 
		IProvide<ToolBox> , IProvide<ILogger>, IProvide<GridView>, IProvide<Grid>, IProvide<GridViewModel>, IProvide<GameManager>,
		IProvide<GameConsole> , IProvide<System.Version> , IProvide<ComponentViewFactory> 
    {
        #region Dependency Injection
        public override partial void _Notification(int what);
		ToolBox IProvide<ToolBox>.Value() => MainToolBox;
        ILogger IProvide<ILogger>.Value() => Logger;
		GridView IProvide<GridView>.Value() => GridView;
		Grid IProvide<Grid>.Value() => Grid;
		GridViewModel IProvide<GridViewModel>.Value() => GridViewModel;
        GameManager IProvide<GameManager>.Value() => Instance;
        GameConsole IProvide<GameConsole>.Value() => InGameConsole;
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
		[Export] public GameConsole InGameConsole { get; set; }
		private PCKLoader PCKLoader { get; set; }
		public static int TilePixelSize { get; private set; } = 62;
		public static int TileBorderLeftDown { get; private set; } = 2;
		public GridView GridView { get; set; }
		public System.Version Version => Assembly.GetExecutingAssembly().GetName().Version; // Get the version from the assembly
		public Grid Grid { get; set; }
		public static GameManager instance;
		public ILogger Logger { get; set; }
		private LogSaver LogSaver { get; set; }
		private List<(String log, bool isError)> InitializationLogs = new();
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
                    InitializationLogs.Add(("Program Version: " + Version, false));
                    InitializeGridAndGridView();
                    InitializeExternalPortViews(Grid.ExternalPorts);
                    PCKLoader = new(ComponentFolderPath, Logger);
                }
                catch (Exception ex)
                {
                    GD.PrintErr(ex.Message);
                    GD.PrintErr(ex);
					InitializationLogs.Add((ex.Message,true));
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
                InitializationLogs.Add(("Initialized ComponentDrafts",false));
            }
            catch (Exception ex)
            {
                InitializationLogs.Add((ex.Message,true));
            }

            ProvideDependenciesOrLogError();
			InitializationLogs.ForEach(l =>
			{
				if (l.isError)
				{
					Logger.PrintErr(l.log);
				} else
				{
                    Logger.Print(l.log);
                }
			});
        }

        private void ProvideDependenciesOrLogError()
        {
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
        }

        private void InitializeLoggingSystemAndConsole()
		{
            Logger = new CAP_Core.Logger();
			LogSaver = new LogSaver(Logger);
            InGameConsole.Initialize(Logger);
            InitializationLogs.Add(("Initialized LoggingSystem",false));
		}

		private void InitializeGridAndGridView()
		{
			GridView = GetNode<GridView>(GridViewPath);
			this.CheckForNull(x => GridView);
			Grid = new Grid(FieldWidth, FieldHeight);
			GridViewModel = new GridViewModel(GridView, Grid, Logger);
			GridView.Initialize(GridViewModel, Logger);
            InitializationLogs.Add(("Initialized GridView and Grid and GridViewModel", false));
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
                InitializationLogs.Add(( d.error,true));
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
					if (input.LaserType.WaveLengthInNm == LaserType.Red.WaveLengthInNm)
					{
						view = (TextureRect)ExternalInputRedTemplate.Duplicate();
					}
					else if (input.LaserType.WaveLengthInNm == LaserType.Green.WaveLengthInNm)
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
