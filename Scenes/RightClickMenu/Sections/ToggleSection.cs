using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

public partial class ToggleSection : ISection
{

    private List<String> toggleValues;
    private int toggleIndex
    {
        get => toggleIndex;
        set
        {
            toggleIndex = value%toggleValues.Count;
            Value = toggleValues[toggleIndex];
        }
    }

    public void Initialize(List<String> toggleValues, String title, String value = "")
    {
        Title = title;
        Value = value;

        if (toggleValues == null) return;
        this.toggleValues = toggleValues;

        if (Value.Equals(""))
            Value = toggleValues[0];
    }

    public void CycleToNextValue(){
        toggleIndex++;
    }

    public String GetNextToggleValue()
    {
        return toggleValues[(toggleIndex + 1) % toggleValues.Count];
    }

    private void OnToggleButtonPressed()
	{
        // We send that we intedn to toggle to next value
        OnPropertyChanged(this, new PropertyChangedEventArgs(GetNextToggleValue()));
    }
}



