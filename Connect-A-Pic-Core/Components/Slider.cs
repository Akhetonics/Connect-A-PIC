using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class Slider : ICloneable, INotifyPropertyChanged
{
    public Guid ID { get; set; }
    public int Number { get; set; }
    private double _value;
    public double Value
    {
        get => _value;
        set
        {
            if (_value != value)
            {
                _value = value;
                OnPropertyChanged();
            }
        }
    }

    public double MaxValue { get; set; }
    public double MinValue { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public Slider(Guid id, int number, double value, double maxValue, double minValue)
    {
        ID = id;
        Number = number;
        Value = value;
        MaxValue = maxValue;
        MinValue = minValue;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public object Clone()
    {
        return new Slider(ID, Number, Value, MaxValue, MinValue);
    }
}
