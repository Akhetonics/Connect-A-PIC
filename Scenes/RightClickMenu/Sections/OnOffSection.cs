using Godot;
using System;
using System.Collections.Generic;
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

    public OnOffSection Initialize(String title, bool value)
    {
        Title = title;
        IsOn = value;
        return this;
    }

    public override void _Ready()
	{
        onTexture = GetNode<TextureRect>("%OnIcon");
	}

    private void OnToggleButtonPressed()
    {
        OnPropertyChanged(this, new PropertyChangedEventArgs(IsOn ? "Off" : "On"));
    }
}





