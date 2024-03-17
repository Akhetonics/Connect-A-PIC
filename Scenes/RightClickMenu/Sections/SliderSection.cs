using Godot;
using System;
using System.ComponentModel;
using System.Diagnostics;

public partial class SliderSection : ISection
{
    private HSlider slider;
    private Timer timer;
    public float MinValue
    {
        get => MinValue;
        set
        {
            MinValue = value;
            slider.MinValue = value;
        }
    }
    public float MaxValue
    {
        get => MaxValue;
        set
        {
            MaxValue = value;
            slider.MaxValue = value;
        }
    }

    /// <summary>
    /// How often we notify slider value change while dragging
    /// </summary>
    public float SliderUpdateInterval { get; set; } = 0.1f;

    bool sliding;
    double prevValue;

	public override void _Ready()
	{
        slider = GetNode<HSlider>("%Slider");
        timer = GetNode<Timer>("%Timer");

        timer.Timeout += () =>
        {
            if (prevValue != slider.Value)
            {
                OnPropertyChanged(this, new PropertyChangedEventArgs(slider.Value.ToString()));
                prevValue = slider.Value;
            }

            if (sliding) timer.Start(SliderUpdateInterval);
        };

        slider.DragStarted += () =>
        {
            prevValue = slider.Value;
            sliding = true;
            timer.Start(SliderUpdateInterval);
        };
        slider.DragEnded += (bool valueChanged) =>
        {
            sliding = false;
            timer.Stop();
            OnPropertyChanged(this, new PropertyChangedEventArgs(slider.Value.ToString()));
        };
    }
}
