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

    public ToggleSection Initialize(List<String> toggleValues, String title, Vector3 value)
    {
        String strValue = "";
        if (value.X > 0.005) strValue = "Red";
        else if (value.Y > 0.005) strValue = "Green";
        else if (value.Z > 0.005) strValue = "Blue";

        return Initialize(toggleValues, title, strValue);
    }


    public ToggleSection Initialize(List<String> toggleValues, String title, String value = "")
    {
        Title = title;
        Value = value;

        if (toggleValues == null) return this;
        this.toggleValues = toggleValues;

        if (Value.Equals("")) Value = toggleValues[0];

        return this;
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



