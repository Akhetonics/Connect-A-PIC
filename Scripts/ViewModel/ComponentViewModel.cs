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
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.Scripts.ViewModel.CommandFactory;
using System;

namespace ConnectAPIC.Scripts.ViewModel
{
    public class ComponentViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        public ILogger Logger { get; private set; }
        public int GridX { get; set; }
        public int GridY { get; set; }
        private ObservableFixedCollection<SliderViewData> sliderData = new();
        public ObservableFixedCollection<SliderViewData> SliderData
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
        public event SliderChangedEventHandler SliderModelChanged;
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        public const int SliderDebounceTimeMs = 50;
        private bool isPlacedInGrid;
        public bool IsPlacedInGrid { get => isPlacedInGrid; set { isPlacedInGrid = value; OnPropertyChanged(); } }

        public Component ComponentModel { get; }
        public GridViewModel GridViewModel { get; }
        public Guid MoveSliderDragGuid { get; private set; } = Guid.NewGuid();

        public ComponentViewModel(Component componentModel, GridViewModel gridViewModel)
        {
            var sliders = componentModel.GetAllSliders();
            
            ComponentModel = componentModel;
            GridViewModel = gridViewModel;
            ComponentModel.SliderValueChanged += ComponentModel_SliderValueChanged;
        }
        public ComponentViewModel()
        {
            
        }
        private void ComponentModel_SliderValueChanged(object sender, System.EventArgs e)
        {
            if(sender is Slider slider)
            {
                SliderData[slider.Number].Value = slider.Value;
                SliderModelChanged?.Invoke((int)slider.Number, slider.Value);
            }
        }

        public void InitializeComponent(int componentTypeNumber, List<SliderViewData> sliderDataSets,  ILogger logger )
        {
            Logger = logger;
            this.TypeNumber = componentTypeNumber;
            RotationCC = rotationCC;
            RegisterSliderDebounceTimer(sliderDataSets);
        }

        public void UpdateDragGuid()
        {
            MoveSliderDragGuid = Guid.NewGuid();
        }
        private void RegisterSliderDebounceTimer(List<SliderViewData> sliderDataSets)
        {
            if (sliderDataSets.Count == 0) return;
            foreach (var sliderDataSet in sliderDataSets)
            {
                sliderDataSet.SliderDebounceTimer = new(SliderDebounceTimeMs);
                sliderDataSet.SliderDebounceTimer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
                {
                    sliderDataSet.SliderDebounceTimer.Stop();
                    var sliderFound = this.SliderData.Single(s => s.Number == sliderDataSet.Number);
                    // change the slider model
                    if(GridViewModel != null)
                    {
                        GridViewModel.CommandFactory
                        .CreateCommand(CommandType.MoveSlider)
                        .ExecuteAsync(new MoveSliderCommandArgs(GridX, GridY, sliderFound.Number, sliderFound.Value, MoveSliderDragGuid))
                        .Wait();
                    }
                };
                this.SliderData.Add(sliderDataSet);
            }
        }

        public void ChangeSliderValueThroughTimer(int sliderNumber, double newVal)
        {
            var slider = SliderData.Single(s => s.Number == sliderNumber);
            if(slider.Value != newVal)
            {
                slider.SliderDebounceTimer.Stop();
                slider.SliderDebounceTimer.Start();
                slider.SliderDebounceTimer.AutoReset = false;
            }
            slider.Value = newVal;
        }
        public void TreeExited()
        {
            if(ComponentModel != null)
            {
                ComponentModel.SliderValueChanged -= ComponentModel_SliderValueChanged;
            }

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
            this.GridX = gridX;
            this.GridY = gridY;
            this.RotationCC = rotationCounterClockwise;
            this.Position = new IntVector(this.GridX * GameManager.TilePixelSize, this.GridY * GameManager.TilePixelSize);
            this.Visible = true;
            IsPlacedInGrid = true;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}
