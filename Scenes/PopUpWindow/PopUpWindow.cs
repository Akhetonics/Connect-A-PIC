using Godot;
using System;
using System.Diagnostics;

namespace ConnectAPIC.Scenes.PopUpWindow
{
    public partial class PopUpWindow : Control
    {
        [Export] public Button ToggleButton { get; set; }


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

        private void OnToggleButtonPressed(bool toggledOn)
        {
            Visible = toggledOn;
        }

        private void OnCloseButtonPressed()
        {
            Visible = false;
            ToggleButton.ButtonPressed = false;
        }

    }
}

