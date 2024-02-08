using CAP_Core.Components;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.Scripts.ViewModel.Commands;
using System.Collections.Generic;
using System.Linq;
using CAP_Core.Helpers;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.Scripts.View.ComponentViews;
using CAP_Contracts.Logger;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using CAP_Core.Grid;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace ConnectAPIC.Scripts.ViewModel
{
    public class ComponentViewModel : INotifyPropertyChanged
    {
        public ILogger Logger { get; private set; }
        public ICommand DeleteComponentCommand { get; set; }
        public ICommand RotateComponentCommand { get; set; }
        public int GridX { get; set; }
        public int GridY { get; set; }
        private ObservableCollection<SliderViewData> sliderData = new();
        public ObservableCollection<SliderViewData> SliderData
        {
            get { return sliderData; }
        }
        private DiscreteRotation rotationCC;
        public DiscreteRotation RotationCC
        {
            get => rotationCC;
            set { rotationCC = value; OnPropertyChanged(); }
        }
        private List<LightAtPin> lightsAtPins;
        public List<LightAtPin> LightsAtPins
        {
            get => lightsAtPins;
            set { lightsAtPins = value; OnPropertyChanged(); }
        }
        private bool isVisible;
        public bool Visible
        {
            get => isVisible;
            set { isVisible = value; OnPropertyChanged(); }
        }
        private IntVector rawPosition;
        public IntVector Position
        {
            get => rawPosition;
            set { rawPosition = value; OnPropertyChanged(); }
        }
        public int TypeNumber { get; set; }
        public delegate void SliderChangedEventHandler(int sliderNumber, double newVal);
        public event SliderChangedEventHandler SliderChanged;
        public event PropertyChangedEventHandler PropertyChanged;
        public const int SliderDebounceTimeMs = 100;
        private bool isPlacedInGrid;
        public bool IsPlacedInGrid { get => isPlacedInGrid; set { isPlacedInGrid = value; OnPropertyChanged(); } }

        public ComponentViewModel()
        {
        }
        public void InitializeComponent(int componentTypeNumber, List<SliderViewData> sliderDataSets,  ILogger logger)
        {
            Logger = logger;
            this.TypeNumber = componentTypeNumber;
            RotationCC = rotationCC;
            RegisterSliderDebounceTimer(sliderDataSets);
        }

        private void RegisterSliderDebounceTimer(List<SliderViewData> sliderDataSets)
        {
            if (sliderDataSets.Count == 0) return;
            foreach (var slider in sliderDataSets)
            {
                slider.SliderDebounceTimer = new(SliderDebounceTimeMs);
                slider.SliderDebounceTimer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
                {
                    slider.SliderDebounceTimer.Stop();
                    var sliderFound = this.SliderData.Single(s => s.Number == slider.Number);

                    SliderChanged?.Invoke((int)sliderFound.Number, sliderFound.Value);
                };
                this.SliderData.Add(slider);
            }
        }

        public void SetSliderValue(int sliderNumber, double newVal, bool isUpdateView = false)
        {
            var slider = SliderData.Single(s => s.Number == sliderNumber);
            if(slider.Value != newVal)
            {
                slider.SliderDebounceTimer.Stop();
                slider.SliderDebounceTimer.Start();
                slider.SliderDebounceTimer.AutoReset = false;
            }
            slider.Value = newVal;

            if (isUpdateView)
            {
                // remove and add the element to get the view updated
                sliderData.Remove(slider);
                sliderData.Add(slider);
            }
        }
        public void TreeExited()
        {
            foreach (var slider in SliderData)
            {
                if (slider.SliderDebounceTimer == null) continue;
                slider.SliderDebounceTimer.Stop();
                slider.SliderDebounceTimer.Dispose();
                slider.SliderDebounceTimer = null;
            }
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
