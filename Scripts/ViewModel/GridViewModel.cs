using CAP_Contracts.Logger;
using CAP_Core.CodeExporter;
using CAP_Core.Components;
using CAP_Core.Components.Creation;
using CAP_Core.Grid;
using CAP_Core.Helpers;
using CAP_Core.LightCalculation;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using ConnectAPIC.Scripts.View.ComponentFactory;
using ConnectAPIC.Scripts.ViewModel;
using ConnectAPIC.Scripts.ViewModel.Commands;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using static ConnectAPIC.Scripts.View.ToolBox.SelectionTool;

namespace ConnectAPIC.LayoutWindow.ViewModel
{
    public class GridViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        public ICommand SwitchOnLightCommand { get; set; }
        public ICommand CreateComponentCommand { get; set; }
        public ICommand MoveComponentCommand { get; set; }
        public ICommand ExportToNazcaCommand { get; set; }
        public ICommand SaveGridCommand { get; internal set; }
        public ICommand LoadGridCommand { get; internal set; }
        public ICommand MoveSliderCommand { get; internal set; }
        public ICommand DeleteComponentCommand { get; internal set; }
        public SelectionGroupManager SelectionGroupManager;
        public delegate void ComponentCreatedEventHandler(Component component, int gridX, int gridY);
        public event ComponentCreatedEventHandler ComponentCreated;
        public event ComponentCreatedEventHandler ComponentRemoved;
        public int Width => Grid.Width;
        public int Height => Grid.Height;
        public GridManager Grid { get; set; }
        public ILogger Logger { get; }
        public ComponentFactory ComponentModelFactory { get; }
        public ToolViewModel ToolViewModel { get; private set; }
        public LightCalculationService LightCalculator { get; private set; }
        public int MaxTileCount { get => Width * Height; }
        private bool isLightOn = false;
        public bool IsLightOn {
            get => isLightOn;
            set { isLightOn = value; OnPropertyChanged(); }
        }
        public LightManager LightManager { get; private set; }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        public GridViewModel( GridManager grid, ILogger logger, ComponentFactory componentModelFactory, LightCalculationService lightCalculator , LightManager lightManager)
        {
            this.Grid = grid;
            LightCalculator = lightCalculator;
            Logger = logger;
            this.ComponentModelFactory = componentModelFactory;
            this.LightManager = lightManager;
            this.LightManager.OnLightSwitched += (object sender, bool e) => IsLightOn = e;
            CreateComponentCommand = new CreateComponentCommand(grid, componentModelFactory);
            MoveComponentCommand = new MoveComponentCommand(grid);
            SaveGridCommand = new SaveGridCommand(grid, new FileDataAccessor());
            LoadGridCommand = new LoadGridCommand(grid, new FileDataAccessor(), componentModelFactory, this);
            MoveSliderCommand = new MoveSliderCommand(grid);
            ExportToNazcaCommand = new ExportNazcaCommand(new NazcaExporter(), grid, new DataAccessorGodot());
            SwitchOnLightCommand = new SwitchOnLightCommand(lightManager);
            DeleteComponentCommand = new DeleteComponentCommand(grid);
            SelectionGroupManager = new(this, new SelectionManager(grid));

            this.Grid.OnComponentPlacedOnTile += Grid_OnComponentPlacedOnTile;
            this.Grid.OnComponentRemoved += (Component component, int x , int y ) => ComponentRemoved?.Invoke(component, x, y);
            this.ToolViewModel = new ToolViewModel(grid);
        }

        private void Grid_OnComponentPlacedOnTile(Component component, int gridX, int gridY)
        {
            ComponentCreated?.Invoke(component, gridX, gridY);
        }
        public bool IsInGrid(int x, int y, int width, int height) => x >= 0 && y >= 0 && x + width <= this.Width && y + height <= this.Height;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}
