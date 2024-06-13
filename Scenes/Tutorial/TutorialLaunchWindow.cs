using Godot;
using System;

public partial class TutorialLaunchWindow : Control
{
    [Export] public Button ToggleButton { get; set; }
    [Export] public TutorialSystem TutorialSystem { get; set; }

    //Godot signal for easier handling from gui
    [Signal] public delegate void WindowClosedEventHandler();

    public override void _Ready()
    {
        Visible = false;
        if (ToggleButton != null)
        {
            ToggleButton.ButtonPressed = false;
            ToggleButton._Toggled(false);
            ToggleButton.Toggled += (bool toggledOn) => OnToggleButtonPressed(toggledOn);
        }
    }

    private void OnTutorialButtonPressed()
    {
        CloseThisWindow();
        TutorialSystem.StartTutorial();
    }
    
    private void OnCloseButtonPressed()
    {
        CloseThisWindow();
    }

    private void OnToggleButtonPressed(bool toggledOn)
    {
        Visible = toggledOn;
        if (!Visible) EmitSignal(SignalName.WindowClosed);
    }

    private void CloseThisWindow()
    {
        Visible = false;
        ToggleButton.ButtonPressed = false;
        EmitSignal(SignalName.WindowClosed);
    }






}




