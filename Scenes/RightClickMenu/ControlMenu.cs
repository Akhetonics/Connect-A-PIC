using CAP_Core.ExternalPorts;
using CAP_Core.Grid;
using CAP_Core.LightCalculation;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.Scenes.RightClickMenu.Sections;
using ConnectAPIC.Scripts.ViewModel;
using ConnectAPIC.Scripts.ViewModel.CommandFactory;
using ConnectAPIC.Scripts.ViewModel.Commands;
using ConnectAPIC.Scripts.ViewModel.Commands.ExternalPorts;
using Godot;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.Scenes.RightClickMenu
{
    public enum PortSwitchButtonIndex
    {
        RedInput,
        GreenInput,
        BlueInput,
        Output
    }

    public partial class ControlMenu : Control {
        [Export] public ButtonGroup ButtonGroup { get; set; }
        [Export] public Curve animationCurve { get; set; }

        public static float LEFT_CENTER_LINE = -120;
        public static float RIGHT_CENTER_LINE = 1807;

        public static float TRAVEL_TIME = 0.3f; //Time needed for menu to travel from one port to another

        public ControlMenuViewModel ViewModel { get; set; }

        public bool LeftSideMode { get; set; } = true;

        public Control InputMenu { get; set; }
        private SliderSection sliderSection;
        public Control OutputMenu { get; set; }
        private InfoSection powerInfo;
        private InfoSection phaseInfo;

        private Godot.Collections.Array<BaseButton> portModeButtons;
        private ExternalPortViewModel portViewModel;
        private float currentXCoordinate = LEFT_CENTER_LINE;
        private bool mouseOutsideClickArea = true;
        private bool spawnInstantly = true;
        private float targetOffsetY = -1;
        private double time_tracked = 0;

        public override void _Input(InputEvent @event) {
            if (@event is InputEventMouseButton mouseButton
                    && mouseButton.Pressed
                    && mouseButton.ButtonIndex == MouseButton.Left
                    && mouseOutsideClickArea) {
                Visible = false;
            }
        }
        public override void _Ready() {
            InputMenu = GetNode<Control>("%InputMenu");
            sliderSection = InputMenu.GetChild<SliderSection>(0);

            OutputMenu = GetNode<Control>("%OutputMenu");
            powerInfo = OutputMenu.GetChild<InfoSection>(0);
            powerInfo.Title = "Power⚡";
            powerInfo.Value = "0.00";
            phaseInfo = OutputMenu.GetChild<InfoSection>(1);
            phaseInfo.Title = "Phase Shift Φ";
            phaseInfo.Value = "NaN";

            if (ButtonGroup != null) {
                portModeButtons = ButtonGroup.GetButtons();
                foreach (BaseButton button in portModeButtons) {
                    button.Pressed += () => HandlePortTypeSwitchingRadioButtonPressed(button);
                }
            }

            this.Visible = false;
        }
        public override void _Process(double delta) {
            MoveToTargetOffsetIfNotThere(delta);
        }

        public void Initialize(ControlMenuViewModel viewModel){
            ViewModel = viewModel;
        }

        public void OnPortClicked(object sender, EventArgs e)
        {
            if (sender is not ExternalPortViewModel portViewModel) return;
            ConnectToPort(portViewModel);
        }

        public void ConnectToPort(ExternalPortViewModel portViewModel) {
            if (this.portViewModel == portViewModel) {
                this.Visible = true;
                return;
            }

            if (this.portViewModel != null) {
                if (this.portViewModel.IsLeftPort != portViewModel.IsLeftPort)
                    spawnInstantly = true; //to move from left/right without animation

                DisconnectFromPort();
            }

            this.portViewModel = portViewModel;
            SetSide(this.portViewModel.IsLeftPort);


            SetPortTypeSwitchingRadioButton(portViewModel);
            SetSectionsVisibility(portViewModel.IsInput);
            InitializeSectionValues(portViewModel);
            targetOffsetY = (GameManager.TilePixelSize) * portViewModel.PortModel.TilePositionY;

            if (spawnInstantly) {
                Position = new Vector2(currentXCoordinate, targetOffsetY);
            }

            spawnInstantly = false;
            time_tracked = 0;
            this.Visible = true;
        }
        public void DisconnectFromPort() {
            this.Visible = false;
            portViewModel.PropertyChanged -= Port_PropertyChanged;
            sliderSection.PropertyChanged -= SliderSection_PropertyChanged;
            portViewModel = null;
        }

        public void SetSide(bool leftSideMode = true) {
            LeftSideMode = leftSideMode;

            // set left and right side exclusive element visibility
            var leftElements = GetTree().GetNodesInGroup("LeftSideGroup");
            var rightElements = GetTree().GetNodesInGroup("RightSideGroup");

            foreach (var element in leftElements) (element as Node2D).Visible = LeftSideMode;
            foreach (var element in rightElements) (element as Node2D).Visible = !LeftSideMode;

            // set offset sign
            if (leftSideMode)
            {
                currentXCoordinate = LEFT_CENTER_LINE;
            }
            else
            {
                currentXCoordinate = RIGHT_CENTER_LINE;
            }

            
        }

        private void MoveToTargetOffsetIfNotThere(double delta) {
            if (time_tracked < TRAVEL_TIME) {
                float step = animationCurve.Sample((float)time_tracked / TRAVEL_TIME);
                float yOffset = (float)Mathf.Lerp(Position.Y, targetOffsetY, step);
                Position = new Vector2(currentXCoordinate, yOffset);
                time_tracked += delta;
            }
        }

        private void InitializeSectionValues(ExternalPortViewModel port) {
            port.PropertyChanged += Port_PropertyChanged;
            sliderSection.PropertyChanged += SliderSection_PropertyChanged;

            if (port.PortModel is ExternalInput)
                sliderSection.SetSliderValue(port.Power);
            else 
                SetInfoSectionValues(port);
        }
        public void SetSectionsVisibility(bool isInput) {
            InputMenu.Visible = isInput;
            OutputMenu.Visible = !isInput;
        }
        private void SetInfoSectionValues(ExternalPortViewModel port) {
            StringBuilder sb = new StringBuilder();

            sb.Append("⚡(");
            sb.Append(port.Power.X.ToString("0.00") + ", ");
            sb.Append(port.Power.Y.ToString("0.00") + ", ");
            sb.Append(port.Power.Z.ToString("0.00") + ")");

            powerInfo.Value = sb.ToString();
            powerInfo.Title = "";

            sb.Clear();
            sb.Append("(");
            sb.Append(port.Phase.X.ToString("0.00") + "°, ");
            sb.Append(port.Phase.Y.ToString("0.00") + "°, ");
            sb.Append(port.Phase.Z.ToString("0.00") + "°)");

            phaseInfo.Value = sb.ToString();
        }

        private async void HandlePortTypeSwitchingRadioButtonPressed(BaseButton button)
        {
            var index = (PortSwitchButtonIndex)portModeButtons.IndexOf(button);
            var wasInputBefore = portViewModel.IsInput;
            await SetPortToInputOutputAsync(index);
            if (wasInputBefore && portViewModel.IsInput)
            {
                await SetInputPortColor(index);
            }
        }

        private async Task SetPortToInputOutputAsync(PortSwitchButtonIndex indexPressed)
        {
            bool? isOutput = null;
            LaserType laserType = ConvertButtonIndexToLaserType(indexPressed);
            if (indexPressed == PortSwitchButtonIndex.Output)
            {
                isOutput = true;
            }
            else if (!portViewModel.IsInput)
            {
                isOutput = false;
            }

            if (isOutput.HasValue)
            {
                var setPortTypeArgs = new SetPortTypeArgs(portViewModel, isOutput.Value, laserType);
                var setPortTypeCommand = ViewModel.CommandFactory.CreateCommand(CommandType.InputOutputChange);
                await setPortTypeCommand.ExecuteAsync(setPortTypeArgs);
            }
        }

        private static LaserType ConvertButtonIndexToLaserType(PortSwitchButtonIndex indexPressed)
        {
            LaserType laserType = LaserType.Red;
            if (indexPressed == PortSwitchButtonIndex.GreenInput)
                laserType = LaserType.Green;
            else if (indexPressed == PortSwitchButtonIndex.BlueInput)
                laserType = LaserType.Blue;
            return laserType;
        }

        private async Task SetInputPortColor(PortSwitchButtonIndex indexPressed)
        {
            LaserType laserType = ConvertButtonIndexToLaserType(indexPressed);
            var InputColorChangeCommand = ViewModel.CommandFactory.CreateCommand(CommandType.InputColorChange);
            await InputColorChangeCommand.ExecuteAsync(new SetInputColorArgs(portViewModel.GetPortIndex(), laserType));
        }

        private void SetPortTypeSwitchingRadioButton(ExternalPortViewModel port) {
            int index = (int)PortSwitchButtonIndex.Output; 
            if (port.PortModel is ExternalInput) {
                if (port.Color == LaserType.Red)
                    index = (int)PortSwitchButtonIndex.RedInput;
                else if (port.Color == LaserType.Green)
                    index = (int)PortSwitchButtonIndex.GreenInput;
                else
                    index = (int)PortSwitchButtonIndex.BlueInput;
            }
            portModeButtons[index].ButtonPressed = true;
        }

        private void Port_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(ExternalPortViewModel.PortModel))
            {
                SetSectionsVisibility(portViewModel.IsInput);
                portModeButtons[(int)PortSwitchButtonIndex.Output].ButtonPressed = true;
            } else if (e.PropertyName == nameof(ExternalPortViewModel.Power)
                    || e.PropertyName == nameof(ExternalPortViewModel.Phase)) {
                if (portViewModel.IsInput) {
                    sliderSection.SetSliderValue(portViewModel.Power);
                } else {
                    SetInfoSectionValues(portViewModel);
                }
            } else if (e.PropertyName == nameof(ExternalPortViewModel.Color)) {
                SetPortTypeSwitchingRadioButton(portViewModel);
            }
        }
        private void SliderSection_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            var SetInputPowerCmd = ViewModel.CommandFactory.CreateCommand(CommandType.InputPowerAdjust);
            SetInputPowerCmd.ExecuteAsync(new SetInputPowerArgs(portViewModel.PortModel, sliderSection.Slider.Value));
        }

        private void OnMouseEntered() {
            mouseOutsideClickArea = false;
        }
        private void OnMouseExited() {
            mouseOutsideClickArea = true;
        }
    }
}

