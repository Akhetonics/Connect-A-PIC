using CAP_Core.ExternalPorts;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.Scenes.ExternalPorts;
using ConnectAPIC.Scripts.ViewModel;
using Godot;
using System;
using System.Collections.Generic;

public partial class SimpleControlMenu : Control
{
    [Export] public ButtonGroup ButtonGroup { get; set; }
    
    public Control InputMenu { get; set; }
    public Control OutputMenu { get; set; }

    private ExternalPortViewModel port;
    private Godot.Collections.Array<BaseButton> portModeButtons;
	public override void _Ready()
	{
        InputMenu = GetNode<Control>("%InputMenu");
        OutputMenu = GetNode<Control>("%OutputMenu");

        if (ButtonGroup != null)
            portModeButtons = ButtonGroup.GetButtons();

        //this.Visible = false;
    }

    public void ConnectToPort(ExternalPortViewModel port)
    {
        if (this.port != null) DisconnectFromPort();
        this.port = port;

        SetPortModeRadioButton(port);

        SetSections(port.IsInput);

        ConnectToSections(port);

        //TODO: Set appropriate offset on X position
        Position = new Vector2(0, (GameManager.TilePixelSize) * port.TilePositionY);
        this.Visible = true;
    }

    public void DisconnectFromPort()
    {
        this.Visible = false;
        //determine if this.port is input or output

        //if input
        //  disconnect from slider
        //if output
        //  disconnect from output

        port = null;
    }


    private void ConnectToSections(ExternalPortViewModel port)
    {
        if (port.IsInput)
        {
            //TODO: connect to slider sections
        }
        else
        {
             //TODO: connect to info sections
        }
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
        if (isInput)
        {
            OutputMenu.Visible = false;
            InputMenu.Visible = true;
        }
        else
        {
            OutputMenu.Visible = true;
            InputMenu.Visible = false;
        }
    }
}
