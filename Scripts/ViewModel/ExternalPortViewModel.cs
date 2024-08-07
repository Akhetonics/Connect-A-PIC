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
        public event EventHandler Clicked;
        public void InvokeClicked() => Clicked?.Invoke(this, EventArgs.Empty);

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

                _portModel = value;
                if (value is ExternalInput input)
                {
                    ResetInputPowerAndColorUsingLaserType(input.LaserType, input.InFlowPower.Real);
                }
                else
                {
                    Power = Vector3.Zero;
                }

                OnPropertyChanged();
            }
        }
        public bool IsLeftPort {  get; private set; }
        public bool IsInput => PortModel is ExternalInput;
        public int TilePositionY { get; private set; } = -1;

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

        private Vector3 _phase;
        public Vector3 Phase {
            get => _phase;
            set {
                _phase = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ExternalPortViewModel(GridManager grid, ExternalPort externalPort, LightCalculationService lightCalculator, bool isLeftPort = true)
        {
            LightCalculator = lightCalculator;
            PortModel = externalPort;
            Power = Vector3.Zero;
            Grid = grid;

            Grid.LightManager.OnLightSwitched += (object sender, bool e) =>
            {
                IsLightOn = e;
                if (!IsLightOn && PortModel is not ExternalInput)
                {
                    //for some reason when light turns off LightCalculationChanged signal isn't emitted
                    ResetPowers();
                }
            };

            lightCalculator.LightCalculationUpdated += ResetPowerMeterDisplay;
            IsLeftPort = isLeftPort;
        }

        public int GetPortIndex()
        {
            return Grid.ExternalPortManager.ExternalPorts.IndexOf(PortModel);
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

        private void ResetPowerMeterDisplay(object sender, LightCalculationUpdated e)
        {
            if (PortModel is ExternalInput) return;
            var x_coord = IsLeftPort ? 0 : Grid.TileManager.Width - 1;
            var touchingComponent = Grid.ComponentMover.GetComponentAt(x_coord, PortModel.TilePositionY);
            if (touchingComponent == null)
            {
                ResetPowers();
                return;
            };
            var offsetY = PortModel.TilePositionY - touchingComponent.GridYMainTile;
            Guid? touchingPin;

            if (IsLeftPort)
                touchingPin = touchingComponent.PinIdLeftOut(0, offsetY);
            else
                touchingPin = touchingComponent.PinIdRightOut(0, offsetY);

            if (touchingPin == null)
            {
                ResetPowers();
                return;
            };
            var fieldOut = e.LightFieldVector[(Guid)touchingPin].Magnitude;
            SetNewPhaseShift((Guid)touchingPin, e);
            UpdateInputPowerAndColorUsingLaserType(e.LaserInUse, (float)(fieldOut * fieldOut));
        }

        private void SetNewPhaseShift(Guid touchingPin, LightCalculationUpdated e)
        {
            float red = Phase.X;
            float green = Phase.Y;
            float blue = Phase.Z;
            float phaseVal = Mathf.RadToDeg((float)e.LightFieldVector[touchingPin].Phase + 2 * Mathf.Pi) % 360;

            if (e.LaserInUse == LaserType.Red) red = phaseVal;
            else if (e.LaserInUse == LaserType.Green) green = phaseVal;
            else blue = phaseVal;

            Phase = new Vector3(red, green, blue);
        }


        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            //only external inputs gives away meaningful property change signals
            ExternalInput inputPort = PortModel as ExternalInput;
            if (inputPort == null) return;

            if (e.PropertyName == nameof(ExternalInput.LaserType)
             || e.PropertyName == nameof(ExternalInput.InFlowPower)) {
                var inputPower = inputPort.InFlowPower.Real;
                ResetInputPowerAndColorUsingLaserType(inputPort.LaserType, inputPower);
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateInputPowerAndColorUsingLaserType(LaserType laserType, double inputPower)
        {
            SetPowerAndColorUsingLaserType(laserType, inputPower, Power);
        }

        private void ResetInputPowerAndColorUsingLaserType(LaserType laserType, double inputPower)
        {
            SetPowerAndColorUsingLaserType(laserType, inputPower, Vector3.Zero);
        }

        private void SetPowerAndColorUsingLaserType(LaserType laserType, double inputPower, Vector3 startValue)
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
