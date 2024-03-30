using ConnectAPIC.Scripts.ViewModel;
using Godot;
using System;
using System.ComponentModel;
using System.Diagnostics;
using YamlDotNet.Core.Tokens;

namespace ConnectAPIC.Scenes.RightClickMenu.Sections {
    public partial class SliderSection : ISection {
        public HSlider Slider { get; private set; }
        private Timer timer;
        private float _minValue;
        private float _maxValue;
        public float MinValue {
            get => _minValue;
            set {
                _minValue = value;
                Slider.MinValue = value;
            }
        }
        public float MaxValue {
            get => _maxValue;
            set {
                _maxValue = value;
                Slider.MaxValue = value;
            }
        }

        /// <summary>
        /// How often we notify slider value change while dragging
        /// </summary>
        public float SliderUpdateInterval { get; set; } = 0.1f;

        bool sliding;
        double prevValue;

        public void Initialize(String title, Vector3 power, float minValue = 0, float maxValue = 1, float sliderUpdateInterval = 0.1f) {
            Title = title;
            Slider.Value = power.X + power.Y + power.Z; //power values should add up to max 1
            if (IsNodeReady())
                Value = Slider.Value.ToString();
        }

        public void Initialize(String title, float minValue = 0, float maxValue = 1, float sliderUpdateInterval = 0.1f) {
            Title = title;
            Slider.Value = MinValue;
        }

        public override void _Ready() {
            Slider = GetNode<HSlider>("%Slider");
            timer = GetNode<Timer>("%Timer");

            timer.Timeout += () => {
                if (prevValue != Slider.Value) {
                    OnPropertyChanged(this, new PropertyChangedEventArgs(Slider.Value.ToString()));
                    prevValue = Slider.Value;
                }

                if (sliding) timer.Start(SliderUpdateInterval);
            };

            Slider.DragStarted += () => {
                prevValue = Slider.Value;
                sliding = true;
                timer.Start(SliderUpdateInterval);
            };
            Slider.DragEnded += (bool valueChanged) => {
                sliding = false;
                timer.Stop();
                OnPropertyChanged(this, new PropertyChangedEventArgs(Slider.Value.ToString()));
            };

            base._Ready();
        }

        //TODO: can add own arguments for sections to make them more flexible
        public void ValueChangeSubscription(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(ExternalPortViewModel.Power)) {
                SetSliderValue((sender as ExternalPortViewModel).Power);
            }
        }

        public void SetSliderValue(double value) {
            Value = value.ToString("0.00");
            Slider.Value = value;
        }

        public void SetSliderValue(Vector3 powerVector) {
            float value = powerVector.Length();
            SetSliderValue(value);
        }
    }
}
