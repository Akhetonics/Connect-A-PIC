using CAP_Core.ExternalPorts;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.Scenes.ExternalPorts;
using ConnectAPIC.Scripts.ViewModel;
using ConnectAPIC.Scripts.ViewModel.Commands;
using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;

public partial class SimpleControlMenu : Control
{
    [Export] public ButtonGroup ButtonGroup { get; set; }
    //TODO: get curve for smooth animation
    //[Export] public C Curve { get; set; }

    public static float X_OFFSET = -120;

    public PortsContainer PortsContainer { get; set; }

    public Control InputMenu { get; set; }
    private SliderSection sliderSection;
    public Control OutputMenu { get; set; }
    private InfoSection powerInfo;
    private InfoSection phaseInfo;

    private Godot.Collections.Array<BaseButton> portModeButtons;
    private ExternalPortViewModel port;
    private bool mouseOutsideClickArea = false;
    private float prevOffsetY = -1;
    private float targetOffsetY = -1;
    private bool inPosition = false;

    public override void _Input(InputEvent @event) {
        if (@event is InputEventMouseButton mouseButton
                && mouseButton.Pressed
                && mouseButton.ButtonIndex == MouseButton.Left
                && mouseOutsideClickArea) {
            Visible = false;
        }
    }

    public override void _Ready()
	{
        InputMenu = GetNode<Control>("%InputMenu");
        sliderSection = InputMenu.GetChild<SliderSection>(0);

        OutputMenu = GetNode<Control>("%OutputMenu");
        powerInfo = OutputMenu.GetChild<InfoSection>(0);
        powerInfo.Title = "Power⚡";
        powerInfo.Value = "0.00";
        phaseInfo = OutputMenu.GetChild<InfoSection>(1);
        phaseInfo.Title = "Phase Shift Φ";
        phaseInfo.Value = "NaN";

        if (ButtonGroup != null)
        {
            portModeButtons = ButtonGroup.GetButtons();
            foreach (BaseButton button in portModeButtons)
            {
                button.Pressed += () => Button_Pressed(button);
            }
        }

        //TODO: remove comment when done testing
        this.Visible = false;
    }

    public override void _Process(double delta) {
        if (Mathf.Abs(Position.Y - targetOffsetY) > 0.05){
            float yOffset= (float)Mathf.Lerp(Position.Y, targetOffsetY, delta);
            Position = new Vector2(X_OFFSET, yOffset);
        }
    }

    private void Button_Pressed(BaseButton button)
    {
        int index = portModeButtons.IndexOf(button);

        if (index == 3)
        {
            PortsContainer.inputOutputChangeCommand.ExecuteAsync(port).Wait();
            return;
        }

        if (!port.IsInput)
            PortsContainer.inputOutputChangeCommand.ExecuteAsync(port).Wait();

        LaserType laserType = LaserType.Red;

        if (index == 1)
            laserType = LaserType.Green;
        else if (index == 2)
            laserType = LaserType.Blue;

        PortsContainer.inputColorChangeCommand.ExecuteAsync(new InputColorChangeArgs(port.PortModel as ExternalInput, laserType)).Wait();
    }

    public void SetPortContainer(PortsContainer portsContainer)
    {
        PortsContainer = portsContainer;
    }

    public void ConnectToPort(ExternalPortViewModel port)
    {
        if (this.port != null) DisconnectFromPort();
        this.port = port;

        SetPortModeRadioButton(port);

        SetSections(port.IsInput);

        ConnectToSections(port);

        //TODO: Set appropriate offset on X position
        targetOffsetY = (GameManager.TilePixelSize) * port.PortModel.TilePositionY;

        if (prevOffsetY < 0) {
            prevOffsetY = targetOffsetY;
            Position = new Vector2(X_OFFSET, prevOffsetY);
        }

        this.Visible = true;
    }
    public void DisconnectFromPort()
    {
        this.Visible = false;

        port.PropertyChanged -= Port_PropertyChanged;
        sliderSection.PropertyChanged -= SliderSection_PropertyChanged;
        prevOffsetY = Position.Y;
        port = null;
    }

    private void Port_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ExternalPortViewModel.IsInput))
        {
            SetSections(port.IsInput);
            portModeButtons[3].ButtonPressed = true;
        }
        else if (e.PropertyName == nameof(ExternalPortViewModel.Power))
        {
            if (port.IsInput)
            {
                sliderSection.SetSliderValue(port.Power);
            }
            else
            {
                powerInfo.Value = port.Power.Length().ToString("0.00");
            }
        }//TODO: could also add phase change code
        else if (e.PropertyName == nameof(ExternalPortViewModel.Color))
        {
            SetPortModeRadioButton(port);
        }
    }
    private void SliderSection_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        PortsContainer.inputPowerAdjustCommand.ExecuteAsync(new InputPowerAdjustArgs(port.PortModel, sliderSection.Slider.Value)).Wait();
    }

    private void SetPortModeRadioButton(ExternalPortViewModel port)
    {
        int index = 3; //3 is by default output button index
        if (port.IsInput)
        {
            if (port.Color == LaserType.Red)
                index = 0;
            else if (port.Color == LaserType.Green)
                index = 1;
            else
                index = 2;
        }
        portModeButtons[index].ButtonPressed = true;
    }
    public void SetSections(bool isInput)
    {
        InputMenu.Visible = isInput;
        OutputMenu.Visible = !isInput;
    }
    private void ConnectToSections(ExternalPortViewModel port)
    {
        port.PropertyChanged += Port_PropertyChanged;
        sliderSection.PropertyChanged += SliderSection_PropertyChanged;

        if (port.IsInput)
            sliderSection.SetSliderValue(port.Power);
    }
    private void OnMouseEntered() {
        mouseOutsideClickArea = false;
    }
    private void OnMouseExited() {
        mouseOutsideClickArea = true;
    }
}





