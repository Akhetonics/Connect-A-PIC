using Godot;
using System;
using System.ComponentModel;
using System.Reflection.Metadata.Ecma335;

public partial class OnOffSection : ISection
{
    public bool IsOn
    {
        get => IsOn;
        set
        {
            IsOn = value;
            onTexture.Visible = value;
            Value = value ? "On" : "Off";
        }
    }

    private TextureRect onTexture;

    public override void _Ready()
	{
        onTexture = GetNode<TextureRect>("%OnIcon");
	}

    private void OnToggleButtonPressed()
    {
        OnPropertyChanged(this, new PropertyChangedEventArgs(IsOn ? "Off" : "On"));
    }
}





