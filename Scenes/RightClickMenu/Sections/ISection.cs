using Godot;
using System;
using System.ComponentModel;
using System.Diagnostics;

public abstract partial class ISection : Node, INotifyPropertyChanged
{
    //To notify if some property of section changed (like if button is pressed to toggle)
    public event PropertyChangedEventHandler PropertyChanged;
    private String _title;
    private String _value;
    private bool ready = false;
    public String Title {
        get => _title;
        set {
            _title = value;
            if (ready)
                titleLabel.Text = value;
        }
    }
    public String Value {
        get => _value;
        set {
            _value = value;
            if (ready)
                valueLabel.Text = value;
        }
    }

    private Label titleLabel;
    private Label valueLabel;

	public override void _Ready()
	{
        titleLabel = GetNode<Label>("%Title");
        valueLabel = GetNode<Label>("%Value");
        if (_title != null) titleLabel.Text = _title;
        if (_value != null) valueLabel.Text = _value;
        ready = true;
	}

    protected void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
        PropertyChanged?.Invoke(sender, e);
    }


}
