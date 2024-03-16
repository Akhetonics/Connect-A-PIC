using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

public partial class ToggleSection : ISection
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        return toggleValues[(toggleIndex + 1) % toggleValues.Count];
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        // We send that we intedn to toggle to next value
        OnPropertyChanged(this, new PropertyChangedEventArgs(GetNextToggleValue()));
	}
}



