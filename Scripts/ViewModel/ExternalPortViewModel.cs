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


public partial class ExternalPortViewModel : Node
{
    public PackedScene ExternalPortTemplate { get; set; }
    public GridView GridView { get; }
    public GridManager Grid { get; }
    public LightCalculationService LightCalculator { get; }
    public Dictionary<int, ExternalPortView> Views { get; }
    
    public event EventHandler<bool> LightChanged;


    public ExternalPortViewModel(PackedScene externalPortTemplate, GridManager grid, GridView gridView, LightCalculationService lightCalculator)
    {
        ExternalPortTemplate = externalPortTemplate;
        LightCalculator = lightCalculator;
        GridView = gridView;
        Grid = grid;

        Views = new Dictionary<int, ExternalPortView>();

        Grid.ExternalPorts.CollectionChanged += (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) =>
        {
            foreach (ExternalPortView val in Views.Values) {
                val.Visible = false;
                val.QueueFree();
            }
            Views.Clear(); //we free up leftover empty references

            InitializeExternalPortViews(Grid.ExternalPorts);
        };

        Grid.OnLightSwitched += (object sender, bool e) =>
        {
            LightChanged?.Invoke(this, e);
        };

        InitializeExternalPortViews(Grid.ExternalPorts);
    }

    private void InitializeExternalPortViews(ObservableCollection<ExternalPort> ExternalPorts)
    {
        ExternalPortView portView;
        foreach (var port in ExternalPorts)
        {
            portView = ExternalPortTemplate.Instantiate<ExternalPortView>();
            GridView.DragDropProxy.AddChild(portView);

            if (port is ExternalInput input)
            {
                if (input.LaserType == LaserType.Red)
                {
                    portView.SetAsInput(this, 1,0,0);
                }
                else if (input.LaserType == LaserType.Green)
                {
                    portView.SetAsInput(this, 0, 1, 0);
                }
                else
                {
                    portView.SetAsInput(this, 0, 0, 1);
                }
            }
            else
            {
                portView.SetAsOutput(new PowerMeterViewModel(Grid, port.TilePositionY, LightCalculator));
            }

            portView.Visible = true;
            portView.Position = new Vector2(0, (GameManager.TilePixelSize) * port.TilePositionY);
            portView.SetPortPositionY(port.TilePositionY);
            Views[port.TilePositionY] = portView;

            portView.Switched += PortsSwitched;
        }
    }

    private void PortsSwitched(object sender, int e)
    {
        int portIndex = Grid.ExternalPorts.IndexOf(Grid.ExternalPorts.FirstOrDefault(exPort => exPort.TilePositionY == e));

        if (portIndex == -1)
        {
            //TODO: print some error
            return;
        }

        if (Grid.ExternalPorts[portIndex] is ExternalInput oldExternalInput){
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

        //TODO: this is a temporary fix remove it after discussion
        //LightCalculator.temporaryFixRemoveThis(Grid);


        //TODO: remove this after debugging
        foreach (var port in Grid.ExternalPorts){
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
}
