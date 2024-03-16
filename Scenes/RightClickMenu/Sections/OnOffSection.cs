using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.Metadata.Ecma335;

public partial class OnOffSection : ISection
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        onTexture = GetNode<TextureRect>("%OnIcon");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        OnPropertyChanged(this, new PropertyChangedEventArgs(IsOn ? "Off" : "On"));
	}
}





