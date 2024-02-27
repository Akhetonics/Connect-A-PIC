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


public partial class ExternalPortViewModel : Node
{
    public ExternalPortView ExternalPortInstance { get; set; }
    public PackedScene ExternalPortTemplate { get; set; }

    public GridView GridView { get; }

    //TODO: this violates MVVM so need to find another way (need to discuss)
    public GridViewModel GridViewModel { get; }
    public GridManager Grid { get; }
    public List<ExternalPortView> Views { get; }

    public event EventHandler<bool> LightChanged;


    public ExternalPortViewModel(PackedScene externalPortTemplate, GridManager grid, GridView gridView, GridViewModel gridViewModel)
    {
        ExternalPortTemplate = externalPortTemplate;
        ExternalPortInstance = externalPortTemplate.Instantiate<ExternalPortView>();
        ExternalPortInstance.Visible = false;
        GridViewModel = gridViewModel;
        GridView = gridView;
        Grid = grid;

        Views = new List<ExternalPortView>();

        Grid.ExternalPorts.CollectionChanged += (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) =>
        {
            Views.ForEach(view => { view.QueueFree(); });
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
                portView.SetAsOutput(new PowerMeterViewModel(Grid, port.TilePositionY, GridViewModel.LightCalculator));
            }

            portView.Visible = true;
            portView.Position = new Vector2(0, (GameManager.TilePixelSize) * port.TilePositionY);
            Views.Add(portView);
        }
    }

}
