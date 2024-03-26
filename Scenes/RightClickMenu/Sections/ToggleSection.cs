using ConnectAPIC.Scripts.ViewModel;
using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

public partial class ToggleSection : ISection
{

    private List<String> toggleValues;

    private int _toggleIndex;
    private int toggleIndex
    {
        get => _toggleIndex;
        set
        {
            _toggleIndex = value%toggleValues.Count;
            Value = toggleValues[_toggleIndex];
        }
    }

    public void Initialize(List<String> toggleValues, String title, Vector3 value)
    {
        String strValue = "";
        if (value.X > 0.005) strValue = "Red";
        else if (value.Y > 0.005) strValue = "Green";
        else if (value.Z > 0.005) strValue = "Blue";

        Initialize(toggleValues, title, strValue);
    }


    public void Initialize(List<String> toggleValues, String title, String value = "")
    {
        Title = title;
        Value = value;

        this.toggleValues = toggleValues;

        if (toggleValues != null) {
            if (Value.Equals(""))
                Value = toggleValues[0]; //toggle index is 0 by default
            else
                toggleIndex = toggleValues.IndexOf(Value);
        }
    }

    //TODO: can add own arguments for sections to make them more flexible
    public void ToggleValueSubscription(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ExternalPortViewModel.Color))
        {
            CycleToNextValue();
        }
    }

    public void CycleSubscriber(object parameter)
    {
        if (parameter is not bool) return;

        if ((bool)parameter) CycleToNextValue();
    }

    public void CycleToNextValue()
    {
        if (toggleValues != null)
            toggleIndex++;
    }

    public String GetNextToggleValue()
    {
        if (toggleValues == null) return null;
        return toggleValues[(toggleIndex + 1) % toggleValues.Count];
    }

    private void OnToggleButtonPressed()
	{
        // We send that we intedn to toggle to next value
        OnPropertyChanged(this, new PropertyChangedEventArgs("Cycle"));
    }

}



