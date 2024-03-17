using Godot;
using System;
using System.ComponentModel;
using System.Diagnostics;

public partial class SliderSection : ISection
{
    public HSlider Slider { get; private set; }
    private Timer timer;
    public float MinValue
    {
        get => MinValue;
        set
        {
            MinValue = value;
            Slider.MinValue = value;
        }
    }
    public float MaxValue
    {
        get => MaxValue;
        set
        {
            MaxValue = value;
            Slider.MaxValue = value;
        }
    }

    /// <summary>
    /// How often we notify slider value change while dragging
    /// </summary>
    public float SliderUpdateInterval { get; set; } = 0.1f;

    bool sliding;
    double prevValue;

    public SliderSection Initialize(String title, Vector3 power, float minValue = 0, float maxValue = 1, float sliderUpdateInterval = 0.1f)
    {
        Title = title;
        Slider.Value = power.X + power.Y + power.Z; //power values should add up to max 1
        Value = Slider.Value.ToString();
        return this;
    }

    public SliderSection Initialize(String title, float minValue = 0, float maxValue = 1, float sliderUpdateInterval = 0.1f)
    {
        Title = title;
        Slider.Value = MinValue;
        return this;
    }

    public override void _Ready()
	{
        Slider = GetNode<HSlider>("%Slider");
        timer = GetNode<Timer>("%Timer");

        timer.Timeout += () =>
        {
            if (prevValue != Slider.Value)
            {
                OnPropertyChanged(this, new PropertyChangedEventArgs(Slider.Value.ToString()));
                prevValue = Slider.Value;
            }

            if (sliding) timer.Start(SliderUpdateInterval);
        };

        Slider.DragStarted += () =>
        {
            prevValue = Slider.Value;
            sliding = true;
            timer.Start(SliderUpdateInterval);
        };
        Slider.DragEnded += (bool valueChanged) =>
        {
            sliding = false;
            timer.Stop();
            OnPropertyChanged(this, new PropertyChangedEventArgs(Slider.Value.ToString()));
        };
    }

}
