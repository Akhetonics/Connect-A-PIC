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


public partial class ExternalPortViewModel : Node
{
    public GridView GridView { get; }

    //TODO: this violates MVVM so need to find another way (need to discuss)
    public GridViewModel GridViewModel { get; }
    public GridManager Grid { get; }
    public List<ExternalPortView> Views { get; }


    public event EventHandler<string> PowerChanged;
    public event EventHandler<bool> LightChanged;

    public int TilePositionY { get; }
    public double PowerRed { get; set; }
    public double PowerGreen { get; set; }
    public double PowerBlue { get; set; }


    public ExternalPortViewModel(GridManager grid, GridView gridView, GridViewModel gridViewModel)
    {
        GridViewModel = gridViewModel;
        GridView = gridView;
        Grid = grid;

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

        GridViewModel.LightCalculator.LightCalculationChanged += (object sender, LightCalculationChangeEventArgs e) =>
        {
            var touchingComponent = grid.GetComponentAt(0, TilePositionY);

            if (touchingComponent == null)
            {
                ResetPowers();
                return;
            };

            //TODO: we use tilePositionY here which seems to be unique for each tile
            //TODO: have to figure out how to do it properly
            var offsetY = TilePositionY - touchingComponent.GridYMainTile;
            var touchingPin = touchingComponent.PinIdLeftOut(0, offsetY);

            if (touchingPin == null)
            {
                ResetPowers();
                return;
            };

            var fieldOut = e.LightFieldVector[(Guid)touchingPin].Magnitude;

            if (e.LaserInUse.Color == LightColor.Red)
            {
                PowerRed = fieldOut * fieldOut;
            }
            else if (e.LaserInUse.Color == LightColor.Green)
            {
                PowerGreen = fieldOut * fieldOut;
            }
            else
            {
                PowerBlue = fieldOut * fieldOut;
            }

            PowerChanged?.Invoke(this, AllColorsPower());
        };

        Grid = grid;
        TilePositionY = 0; //TODO: find out what is this?

        PowerChanged?.Invoke(this, AllColorsPower());
    }

    private void InitializeExternalPortViews(ObservableCollection<ExternalPort> ExternalPorts)
    {
        ExternalPortView portView;
        foreach (var port in ExternalPorts)
        {
            if (port is ExternalInput input)
            {
                if (input.LaserType == LaserType.Red)
                {
                    portView = new ExternalPortView(this).SetAsInput(1,0,0);
                }
                else if (input.LaserType == LaserType.Green)
                {
                    portView = new ExternalPortView(this).SetAsInput(0, 1, 0);
                }
                else
                {
                    portView = new ExternalPortView(this).SetAsInput(0, 0, 1);
                }
            }
            else
            {
                portView = new ExternalPortView(this/*, port.TilePositionY*/).SetAsOutput();

                //view = (PowerMeterView)ExternalOutputTemplate.Instantiate();
                //view.GlobalPosition = new Vector2(GridView.DragDropProxy.GlobalPosition.X, 0); // y will be overridden below
                //var powerMeterView = (PowerMeterView)view;


                //TODO: figure out how to have one viewModel for each port, each one needs separate tilePositionY
                //var powerMeterViewModel = new PowerMeterViewModel(Grid, port.TilePositionY, GridViewModel.LightCalculator);
                //powerMeterView.Initialize(powerMeterViewModel);
            }

            Views.Add(portView);

            portView.Visible = true;
            GridViewModel.GridView.DragDropProxy.AddChild(portView);
            portView.Position = new Vector2(portView.Position.X - GridView.GlobalPosition.X, (GameManager.TilePixelSize) * port.TilePositionY);
        }
    }

    public string AllColorsPower()
    {
        string allUsedPowers = "";

        if (PowerRed > 0.005)
        {
            allUsedPowers += $"[color=#FF6666]R: {PowerRed:F2}[/color]\n";
        }
        if (PowerGreen > 0.005)
        {
            allUsedPowers += $"[color=#66FF66]G: {PowerGreen:F2}[/color]\n";
        }
        if (PowerBlue > 0.005)
        {
            allUsedPowers += $"[color=#6666FF]B: {PowerBlue:F2}[/color]";
        }

        // Removes the trailing newline character if any colors were added
        return allUsedPowers.TrimEnd('\n');
    }

    private void ResetPowers()
    {
        PowerRed = 0;
        PowerGreen = 0;
        PowerBlue = 0;
        PowerChanged?.Invoke(this, AllColorsPower());
    }

}
