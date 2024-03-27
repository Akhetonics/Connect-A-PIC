using Godot;
using System;

public partial class InputModeButton : TextureButton
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        Toggled += ControlOpacity;
    }

    private void ControlOpacity(bool toggledOn)
    {
        if (toggledOn)
        {
            Modulate = Modulate.Darkened(0);
        }
        else
        {
            Modulate = Modulate.Darkened(0.2f);
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
