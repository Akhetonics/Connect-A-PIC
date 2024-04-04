using ConnectAPIC.Scenes.ExternalPorts;
using Godot;
using System;
using System.Collections.Generic;
using SuperNodes.Types;
using Chickensoft.AutoInject;
using System.Collections.ObjectModel;
using CAP_Core.Grid;
using CAP_Core.ExternalPorts;
using System.Diagnostics;
using CAP_Core.LightCalculation;
using System.ComponentModel;
using ConnectAPIC.Scripts.ViewModel;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using ConnectAPIC.Scripts.ViewModel.Commands;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.Scenes.RightClickMenu;


[SuperNode(typeof(Dependent))]
public partial class PortsContainerView : Node2D
{
    public override partial void _Notification(int what);
    [Dependency] public GridManager Grid => DependOn<GridManager>();
    [Dependency] public LightCalculationService LightCalculator => DependOn<LightCalculationService>();

    [Export] public PackedScene ExternalPortViewTemplate { get; set; }
    [Export] public PackedScene RightClickMenuTemplate {  get; set; }

    public void OnResolved() {
        InitializePortsAndControlMenuThenConnectThem();
    }

    private void InitializePortsAndControlMenuThenConnectThem()
    {
        // initialize ports (port views will be children of this, and they will be connected to view model properly)
        var factory = new ExternalPortViewFactory(this, Grid, LightCalculator);
        List<ExternalPortViewModel> ports = factory.InitializeExternalPortViewList();

        // create control menu, it will be child of ports container as well
        ControlMenu controlMenu = RightClickMenuTemplate.Instantiate<ControlMenu>();
        controlMenu.Initialize(Grid, LightCalculator);
        this.AddChild(controlMenu);

        // connect ports to control menu
        foreach (ExternalPortViewModel viewModel in ports)
        {
            viewModel.Clicked += controlMenu.OnPortClicked;
        }
    }
}
