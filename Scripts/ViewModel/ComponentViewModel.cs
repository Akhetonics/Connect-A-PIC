using CAP_Core.Components;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.Scripts.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CAP_Core.ExternalPorts;
using CAP_Core.Helpers;
using System.Threading;
using CAP_Core.Tiles;
using ConnectAPIC.LayoutWindow.View;
using CAP_Core;
using ConnectAPIC.Scripts.View.ComponentViews;
using CAP_Contracts.Logger;
using ConnectAPIC.Scripts.Helpers;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using CAP_Core.Grid;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using Godot;

namespace ConnectAPIC.Scripts.ViewModel
{
    public class ComponentViewModel : INotifyPropertyChanged
    {
        public ILogger Logger { get; private set; }
        public ICommand DeleteComponentCommand { get; set; }
        public ICommand RotateComponentCommand { get; set; }
        public int GridX { get; set; }
        public int GridY { get; set; }
        private List<SliderViewData> _sliderData;
        public List<SliderViewData> SliderData
        {
            get { return _sliderData; }
            set { _sliderData = value; OnPropertyChanged(); }
        }
        private DiscreteRotation _rotationCC;
        public DiscreteRotation RotationCC
        {
            get => _rotationCC;
            set { _rotationCC = value; OnPropertyChanged(); }
        }
        private List<LightAtPin> _lightsAtPins;
        public List<LightAtPin> LightsAtPins
        {
            get => _lightsAtPins;
            set { _lightsAtPins = value; OnPropertyChanged(); }
        }
        private bool _isVisible;
        public bool Visible
        {
            get => _isVisible;
            set { _isVisible = value; OnPropertyChanged(); }
        }
        private IntVector _rawPosition;
        public IntVector Position
        {
            get => _rawPosition;
            set { _rawPosition = value; OnPropertyChanged(); }
        }
        public int TypeNumber { get; set; }
        public delegate void SliderChangedEventHandler(int sliderNumber, double newVal);
        public event SliderChangedEventHandler SliderChanged;
        public event PropertyChangedEventHandler PropertyChanged;
        private System.Timers.Timer SliderDebounceTimer = new(100);
        public bool IsPlacedInGrid { get; set; }

        public ComponentViewModel()
        {
        }
        public void InitializeComponent(int componentTypeNumber, List<SliderViewData> sliderDataSets,  ILogger logger)
        {
            Logger = logger;
            this.SliderData = sliderDataSets;
            this.TypeNumber = componentTypeNumber;
            RotationCC = _rotationCC;
        }

        public void SliderValueChanged(int sliderNumber, double newVal)
        {
            SliderDebounceTimer.AutoReset = false;
            SliderDebounceTimer.Stop();
            SliderDebounceTimer.Start();
            SliderDebounceTimer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
            {
                SliderChanged?.Invoke(sliderNumber, newVal);
                SliderDebounceTimer.Stop();
            };
        }
        public void TreeExited()
        {
            SliderDebounceTimer.Stop();
            SliderDebounceTimer.Dispose();
            SliderDebounceTimer = null;
        }

        public void DisplayLightVector(List<LightAtPin> lightsAtPins)
        {
            this.LightsAtPins = lightsAtPins;
        }

        public void RegisterInGrid(GridManager grid, int gridX, int gridY, DiscreteRotation rotationCounterClockwise)
        {
            DeleteComponentCommand = new DeleteComponentCommand(grid);
            RotateComponentCommand = new RotateComponentCommand(grid);
            this.GridX = gridX;
            this.GridY = gridY;
            this.RotationCC = rotationCounterClockwise;
            this.Position = new IntVector(this.GridX * GameManager.TilePixelSize, this.GridY * GameManager.TilePixelSize);
            this.Visible = true;
            IsPlacedInGrid = true;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
