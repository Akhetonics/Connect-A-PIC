using CAP_Contracts.Logger;
using CAP_Core.CodeExporter;
using CAP_Core.Components;
using CAP_Core.Components.Creation;
using CAP_Core.Grid;
using CAP_Core.Helpers;
using CAP_Core.LightCalculation;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using ConnectAPIC.Scenes.RightClickMenu;
using ConnectAPIC.Scripts.View.ComponentFactory;
using ConnectAPIC.Scripts.ViewModel;
using ConnectAPIC.Scripts.ViewModel.CommandFactory;
using ConnectAPIC.Scripts.ViewModel.Commands;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Threading;
using static ConnectAPIC.Scripts.View.ToolBox.SelectionTool;

namespace ConnectAPIC.LayoutWindow.ViewModel
{
    public class GridViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        public ICommandFactory CommandFactory { get; private set; }
        public SelectionGroupManager SelectionGroupManager;
        public delegate void ComponentCreatedEventHandler(Component component, int gridX, int gridY);
        public event ComponentCreatedEventHandler ComponentCreated;
        public event ComponentCreatedEventHandler ComponentRemoved;
        public int Width => Grid.TileManager.Width;
        public int Height => Grid.TileManager.Height;
        public GridManager Grid { get; set; }
        public ILogger Logger { get; }
        public ComponentFactory ComponentModelFactory { get; }
        public ToolViewModel ToolViewModel { get; private set; }
        public LightCalculationService LightCalculator { get; private set; }
        private ComponentViewModel[,] ComponentViewModels { get; set; }
        public int MaxTileCount { get => Width * Height; }
        private bool isLightOn = false;
        public bool IsLightOn {
            get => isLightOn;
            set { isLightOn = value; OnPropertyChanged(); }
        }
        public LightManager LightManager { get; private set; }
        public ControlMenuViewModel ControlMenuViewModel { get; internal set; }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        public GridViewModel( GridManager grid, ILogger logger, ComponentFactory componentModelFactory, LightCalculationService lightCalculator )
        {
            this.Grid = grid;
            LightCalculator = lightCalculator;
            Logger = logger;
            this.ComponentModelFactory = componentModelFactory;
            LightManager = grid.LightManager;
            this.LightManager.OnLightSwitched += (object sender, bool e) => IsLightOn = e;
            SelectionGroupManager = new(this, new SelectionManager(grid));
            CommandFactory = new CommandFactory(Grid, componentModelFactory, SelectionGroupManager.SelectionManager, Logger, lightCalculator, this);
            this.Grid.ComponentMover.OnComponentPlacedOnTile += Grid_OnComponentPlacedOnTile;
            this.Grid.ComponentMover.OnComponentRemoved += (Component component, int x , int y ) => ComponentRemoved?.Invoke(component, x, y);
            this.ToolViewModel = new ToolViewModel(grid);
            ControlMenuViewModel = new ControlMenuViewModel(Grid, LightCalculator, CommandFactory);
        }
        public ComponentViewModel GetComponentViewModel(int gridX, int gridY)
        {
            return ComponentViewModels[gridX, gridY];
        }
        public void RegisterComponentViewModel(int gridX, int gridY, ComponentViewModel componentViewModel)
        {
            ComponentViewModels[gridX, gridY] = componentViewModel;
        }
        public void UnRegisterComponentViewModel( int gridX, int gridY)
        {
            ComponentViewModels[gridX, gridY] = null;
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
