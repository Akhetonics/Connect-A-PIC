using CAP_Core.Components.ComponentHelpers;
using CAP_Core.ExternalPorts;
using CAP_Core.Grid;
using CAP_Core.LightCalculation;
using Godot;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;


namespace ConnectAPIC.Scripts.ViewModel
{
    public class ExternalPortViewModel : INotifyPropertyChanged
    {
        public event EventHandler RightClicked;
        public void InvokeRightClicked() => RightClicked.Invoke(this, EventArgs.Empty);

        public GridManager Grid { get; }
        public LightCalculationService LightCalculator { get; }

        private ExternalPort _portModel;
        public ExternalPort PortModel
        {
            get => _portModel;
            set
            {
                if (_portModel != null)
                    _portModel.PropertyChanged -= Model_PropertyChanged;
                _portModel = value;
                _portModel.PropertyChanged += Model_PropertyChanged;

                IsInput = (value is ExternalInput);

                ExternalInput tmp = (value as ExternalInput);
                if (tmp != null)
                {
                    ResetInputPowerAndColorUsingLazerType(tmp.LaserType, tmp.InFlowPower.Real);
                }

                OnPropertyChanged();
            }
        }

        public int TilePositionY
        {
            get;
            private set;
        } = -1;

        private bool _isInput;
        public bool IsInput
        {
            get => _isInput;
            set
            {
                _isInput = value;
                OnPropertyChanged();
            }
        }

        private bool _isLightOn;
        public bool IsLightOn
        {
            get { return _isLightOn; }
            set
            {
                _isLightOn = value;
                OnPropertyChanged();
            }
        }

        private LaserType _color;
        public LaserType Color {
            get => _color;
            set
            {
                _color = value;
                OnPropertyChanged();
            }
        }

        private Vector3 _power;
        public Vector3 Power
        {
            get => _power;
            set
            {
                _power = value;
                OnPropertyChanged();
            } 
        }

       

        public event PropertyChangedEventHandler PropertyChanged;

        public ExternalPortViewModel(GridManager grid, ExternalPort externalPort, LightCalculationService lightCalculator)
        {
            LightCalculator = lightCalculator;
            PortModel = externalPort;
            Power = Vector3.Zero;
            Grid = grid;

            Grid.OnLightSwitched += (object sender, bool e) =>
            {
                IsLightOn = e;
                if (!IsLightOn && !IsInput)
                {
                    //for some reason when light turns off LightCalculationChanged signal isn't emitted
                    ResetPowers();
                }
            };

            lightCalculator.LightCalculationChanged += ResetPowerMeterDisplay;
        }




        public string AllColorsPower()
        {
            string allUsedPowers = "";

            if (Power.X > 0.005)
            {
                allUsedPowers += $"[color=#FF4444]R: {Power.X:F2}[/color]\n";
            }
            if (Power.Y > 0.005)
            {
                allUsedPowers += $"[color=#44FF44]G: {Power.Y:F2}[/color]\n";
            }
            if (Power.Z > 0.005)
            {
                allUsedPowers += $"[color=#4444FF]B: {Power.Z:F2}[/color]";
            }

            // Removes the trailing newline character if any colors were added
            return allUsedPowers.TrimEnd('\n');
        }
        private void ResetPowers()
        {
            Power = Vector3.Zero;
        }

        private void ResetPowerMeterDisplay(object sender, LightCalculationChangeEventArgs e)
        {
            if (IsInput) return;
            var touchingComponent = Grid.ComponentMover.GetComponentAt(0, PortModel.TilePositionY);
            if (touchingComponent == null)
            {
                ResetPowers();
                return;
            };
            var offsetY = PortModel.TilePositionY - touchingComponent.GridYMainTile;
            var touchingPin = touchingComponent.PinIdLeftOut(0, offsetY);
            if (touchingPin == null)
            {
                ResetPowers();
                return;
            };
            var fieldOut = e.LightFieldVector[(Guid)touchingPin].Magnitude;

            UpdateInputPowerAndColorUsingLazerType(e.LaserInUse, (float)(fieldOut * fieldOut));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //only external inputs gives away meaningfull property change signals
            ExternalInput inputPort = PortModel as ExternalInput;
            if (inputPort == null) return;

            if (e.PropertyName == nameof(ExternalInput.LaserType)
             || e.PropertyName == nameof(ExternalInput.InFlowPower))
            {
                var inputPower = inputPort.InFlowPower.Real;
                ResetInputPowerAndColorUsingLazerType(inputPort.LaserType, inputPower);
            }
        }

        private void UpdateInputPowerAndColorUsingLazerType(LaserType laserType, double inputPower)
        {
            SetPowerAndColorUsingLazerType(laserType, inputPower, Power);
        }

        private void ResetInputPowerAndColorUsingLazerType(LaserType laserType, double inputPower)
        {
            SetPowerAndColorUsingLazerType(laserType, inputPower, Vector3.Zero);
        }

        private void SetPowerAndColorUsingLazerType(LaserType laserType, double inputPower, Vector3 startValue)
        {
            var power = startValue;
            if (laserType == LaserType.Red)
            {
                power.X = (float)inputPower;
            }
            else if (laserType == LaserType.Green)
            {
                power.Y = (float)inputPower;
            }
            else
            {
                power.Z = (float)inputPower;
            }
            Color = laserType;
            Power = power;
        }
    }
}
