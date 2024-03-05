using CAP_Contracts.Logger;
using CAP_Core.Components;
using CAP_Core.Components.Creation;
using CAP_Core.ExternalPorts;
using CAP_DataAccess;
using CAP_DataAccess.Components.ComponentDraftMapper;
using Chickensoft.AutoInject;
using CAP_DataAccess.Components.ComponentDraftMapper.DTOs;
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
using CAP_Core.Grid;
using CAP_Core.LightCalculation;
using ConnectAPIC.Scripts.View.PowerMeter;
using ConnectAPIC.Scripts.ViewModel;

namespace ConnectAPic.LayoutWindow
{
    [SuperNode(typeof(Provider))]
    public partial class GameManager : Node,
        IProvide<ToolBox>, IProvide<ILogger>, IProvide<GridView>, IProvide<GridManager>, IProvide<GridViewModel>, IProvide<GameManager>,
        IProvide<GameConsole>, IProvide<System.Version>, IProvide<ComponentViewFactory>
    {
        #region Dependency Injection
        public override partial void _Notification(int what);
        ToolBox IProvide<ToolBox>.Value() => MainToolBox;
        ILogger IProvide<ILogger>.Value() => Logger;
        GridView IProvide<GridView>.Value() => GridView;
        GridManager IProvide<GridManager>.Value() => Grid;
        GridViewModel IProvide<GridViewModel>.Value() => GridViewModel;
        GameManager IProvide<GameManager>.Value() => this;
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
        [Export] public Node2D ExternalInputRedTemplate { get; set; }
        [Export] public Node2D ExternalInputGreenTemplate { get; set; }
        [Export] public Node2D ExternalInputBlueTemplate { get; set; }
        [Export] public GameConsole InGameConsole { get; set; }
        private PCKLoader PCKLoader { get; set; }
        public static int TilePixelSize { get; private set; } = 62;
        public static int TileBorderLeftDown { get; private set; } = 2;
        public GridView GridView { get; set; }
        public static System.Version Version => Assembly.GetExecutingAssembly().GetName().Version; // Get the version from the assembly
        public GridManager Grid { get; set; }
        public LightCalculationService LightCalculator { get; private set; }
        public ILogger Logger { get; set; }
        private LogSaver LogSaver { get; set; }
        private List<(String log, bool isError)> InitializationLogs = new();
        public GridViewModel GridViewModel { get; private set; }
        public ComponentFactory ComponentModelFactory { get; set; }
        public const string ComponentFolderPath = "res://Scenes/Components";

        public override void _EnterTree()
        {
            base._EnterTree();
            try
            {
                InitializeLoggingSystemAndConsole();
                InitializationLogs.Add(("Program Version: " + Version, false));
                ComponentModelFactory = new ComponentFactory();
                InitializeGridAndGridView(ComponentModelFactory);
                InitializeExternalPortViews(Grid.ExternalPorts);
                PCKLoader = new(ComponentFolderPath, Logger);
            }
            catch (Exception ex)
            {
                GD.PrintErr(ex.Message);
                GD.PrintErr(ex);
                InitializationLogs.Add((ex.Message, true));
            }
        }

        public override void _Ready()
        {
            try
            {
                PCKLoader.LoadStandardPCKs();
                List<ComponentDraft> componentDrafts = EquipViewComponentFactoryWithJSONDrafts();
                this.CheckForNull(x => x.ToolBoxPath);
                List<Component> modelComponents = new ComponentDraftConverter(Logger).ToComponentModels(componentDrafts);
                ComponentModelFactory.InitializeComponentDrafts(modelComponents);
                InitializationLogs.Add(("Initialized ComponentDrafts", false));

                MainToolBox = GetNode<ToolBox>(ToolBoxPath);
                this.CheckForNull(x => x.MainToolBox);
            }
            catch (Exception ex)
            {
                InitializationLogs.Add((ex.Message, true));
            }

            ProvideDependenciesOrLogError();
            InitializationLogs.ForEach(l =>
            {
                if (l.isError)
                {
                    Logger.PrintErr(l.log);
                }
                else
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
            InitializationLogs.Add(("Initialized LoggingSystem", false));
        }

        private void InitializeGridAndGridView(ComponentFactory componentFactory)
        {
            GridView = GetNode<GridView>(GridViewPath);
            this.CheckForNull(x => GridView);
            Grid = new GridManager(FieldWidth, FieldHeight);
            LightCalculator = new CAP_Core.LightCalculation.LightCalculationService(Grid.GetAllExternalInputs(), new GridLightCalculator(new SystemMatrixBuilder(Grid), Grid));
            GridViewModel = new GridViewModel(GridView, Grid, Logger, componentFactory, LightCalculator);
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
                InitializationLogs.Add((d.error, true));
        }

        private void InitializeExternalPortViews(List<ExternalPort> ExternalPorts)
        {
            ExternalInputRedTemplate.Visible = false;
            ExternalInputGreenTemplate.Visible = false;
            ExternalInputBlueTemplate.Visible = false;
            Node2D view;
            foreach (var port in ExternalPorts)
            {

                if (port is ExternalInput input)
                {
                    if (input.LaserType == LaserType.Red)
                    {
                        view = (Node2D)ExternalInputRedTemplate.Duplicate();
                    }
                    else if (input.LaserType == LaserType.Green)
                    {
                        view = (Node2D)ExternalInputGreenTemplate.Duplicate();
                    }
                    else
                    {
                        view = (Node2D)ExternalInputBlueTemplate.Duplicate();
                    }

                }
                else
                {
                    view = (PowerMeterView)ExternalOutputTemplate.Instantiate();
                    view.GlobalPosition = new Vector2(GridView.DragDropProxy.GlobalPosition.X, 0); // y will be overridden below
                    var powerMeterView = (PowerMeterView)view;
                    var powerMeterViewModel = new PowerMeterViewModel(Grid, port.TilePositionY, GridViewModel.LightCalculator);
                    powerMeterView.Initialize(powerMeterViewModel);
                }
                view.Visible = true;
                GridViewModel.GridView.DragDropProxy.AddChild(view);
                view.Position = new Godot.Vector2(view.Position.X - GridView.GlobalPosition.X, (GameManager.TilePixelSize) * port.TilePositionY);
            }
        }

    }
}
