using Godot;
using System;
using System.ComponentModel;

public abstract partial class ISection : Node, INotifyPropertyChanged
{
	// Called when the node enters the scene tree for the first time.
    //To notify if some property of section changed (like if button is pressed to toggle)
    public event PropertyChangedEventHandler PropertyChanged;

    public String Title {
        get => Title;
        set {
            Title = value;
            titleLabel.Text = value;
        }
    }
    public String Value {
        get => Value;
        set {
            Value = value;
            titleLabel.Text = value;
        }
    }

    private Label titleLabel;
    private Label valueLabel;

	public override void _Ready()
	{
        titleLabel = GetNode<Label>("%Title");
        valueLabel = GetNode<Label>("%Value");
	}

    protected void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
        PropertyChanged?.Invoke(sender, e);
	}


}
