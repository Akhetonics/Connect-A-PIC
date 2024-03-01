using CAP_Core.ExternalPorts;
using CAP_Core.Grid;
using Chickensoft.GodotTestDriver.Drivers;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.Scripts.View.PowerMeter;
using ConnectAPIC.Scripts.ViewModel;
using ConnectAPIC.Scenes.ExternalPorts;
using Godot;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using CAP_Core.LightCalculation;
using CAP_Core.Components;
using CAP_Core.Components.ComponentHelpers;
using System.Diagnostics;
using System.Linq;
using CAP_Core.Components.Creation;
using CAP_Core;
using System.ComponentModel;
using System.Runtime.CompilerServices;


public partial class ExternalPortViewModel : Node, INotifyPropertyChanged
{
    public GridManager Grid { get; }
    public LightCalculationService LightCalculator { get; }
    public ObservableCollection<ExternalPort> ExternalPorts { get; set; }
    public event EventHandler<bool> LightChanged;
    private bool _lightIsOn;

    public bool LightIsOn
    {
        get { return _lightIsOn; }
        set { _lightIsOn = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public ExternalPortViewModel(PackedScene externalPortTemplate, GridManager grid, GridView gridView, LightCalculationService lightCalculator)
    {
        LightCalculator = lightCalculator;
        Grid = grid;

        Grid.ExternalPorts.CollectionChanged += (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) =>
        {
            // delete all external ports one by one 
            foreach(ExternalPort port in e.OldItems)
            {
                ExternalPorts.Remove(port);
            }
            // add the new ports
            foreach(ExternalPort port in e.NewItems)
            {
                ExternalPorts.Add(port);
            }
        };

        Grid.OnLightSwitched += (object sender, bool e) =>
        {
            LightChanged?.Invoke(this, e);
        };
    }



    private void PortsSwitched(object sender, int e)
    {
        int portIndex = Grid.ExternalPorts.IndexOf(Grid.ExternalPorts.FirstOrDefault(exPort => exPort.TilePositionY == e));

        if (portIndex == -1)
        {
            //TODO: print some error
            return;
        }

        if (Grid.ExternalPorts[portIndex] is ExternalInput oldExternalInput)
        {
            //TODO: need to refactor this after implementing right click menu
            if (oldExternalInput.LaserType == LaserType.Red)
            {
                Grid.ExternalPorts[portIndex] = new ExternalInput(oldExternalInput.PinName, LaserType.Green, oldExternalInput.TilePositionY, oldExternalInput.InFlowPower);
            }
            else if (oldExternalInput.LaserType == LaserType.Green)
            {
                Grid.ExternalPorts[portIndex] = new ExternalInput(oldExternalInput.PinName, LaserType.Blue, oldExternalInput.TilePositionY, oldExternalInput.InFlowPower);
            }
            else
            {
                Grid.ExternalPorts[portIndex] = new ExternalOutput(oldExternalInput.PinName, oldExternalInput.TilePositionY);
            }
        }
        else
        {
            ExternalOutput oldOutput = (ExternalOutput)Grid.ExternalPorts[portIndex];
            Grid.ExternalPorts[portIndex] = new ExternalInput(oldOutput.PinName, LaserType.Red, oldOutput.TilePositionY, 1);
        }

        //TODO: remove this after debugging
        foreach (var port in Grid.ExternalPorts)
        {
            if (port is ExternalInput)
            {
                Debug.Print("Input: " + port.PinName + " " + port.TilePositionY);
            }
            else
            {
                Debug.Print("Output: " + port.PinName + " " + port.TilePositionY);
            }
        }

    }
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
