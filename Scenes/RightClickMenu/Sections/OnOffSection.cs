using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Management;
using System.Reflection.Metadata.Ecma335;

public partial class OnOffSection : ISection
{
    private bool _isOn;
    private bool ready = false;
    public bool IsOn
    {
        get => _isOn;
        set
        {
            _isOn = value;
            Value = value ? "On" : "Off";
            if (ready)
                onTexture.Visible = value;
        }
    }

    private TextureRect onTexture;

    public OnOffSection Initialize(String title, bool value)
    {
        Title = title;
        IsOn = value;
        return this;
    }

    public override void _Ready()
	{
        onTexture = GetNode<TextureRect>("%OnIcon");
        ready = true;
        base._Ready();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        OnPropertyChanged(this, new PropertyChangedEventArgs(IsOn ? "Off" : "On"));
	}
}





