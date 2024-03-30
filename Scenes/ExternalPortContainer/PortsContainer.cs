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
using ConnectAPIC.Scenes.RightClickMenu;
using System.ComponentModel;
using ConnectAPIC.Scripts.ViewModel;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using ConnectAPIC.Scripts.ViewModel.Commands;




[SuperNode(typeof(Dependent))]
public partial class PortsContainer : Node2D
{
    public override partial void _Notification(int what);
    [Dependency] public GridManager Grid => DependOn<GridManager>();
    [Dependency] public LightCalculationService LightCalculator => DependOn<LightCalculationService>();

    [Export] public PackedScene ExternalPortViewTemplate { get; set; }
    [Export] public PackedScene RightClickMenuTemplate {  get; set; }

    public SimpleControlMenu ControlMenu { get; set; }
    public ExternalPortViewFactory PortViewFactory { get; set; }
    public List<ExternalPortViewModel> PortViewModels { get; set; } = new();

    //Commands
    //TODO: this is temporary solution, needs to be removed after implementing command handling
    public static InputPowerAdjustCommand inputPowerAdjustCommand;
    public static InputColorChangeCommand inputColorChangeCommand;
    public static InputOutputChangeCommand inputOutputChangeCommand;

    public void OnResolved()
    {
        InitializeCommands();
        InitializePortsAndPortsFactory();
        InitializeControlMenuAndConnectToPorts();
    }

    private void InitializeControlMenuAndConnectToPorts()
    {
        foreach (ExternalPortViewModel viewModel in PortViewModels)
        {
            viewModel.Clicked += ViewModel_Clicked;
        }
        ControlMenu = RightClickMenuTemplate.Instantiate<SimpleControlMenu>();
        ControlMenu.SetPortContainer(this);
        this.AddChild(ControlMenu);
    }
    private void InitializePortsAndPortsFactory()
    {
        PortViewFactory = new ExternalPortViewFactory(this, Grid, LightCalculator);
        PortViewModels = PortViewFactory?.InitializeExternalPortViewList(Grid.ExternalPorts);
    }
    private void InitializeCommands()
    {
        inputPowerAdjustCommand = new InputPowerAdjustCommand(Grid);
        inputColorChangeCommand = new InputColorChangeCommand(Grid);
        inputOutputChangeCommand = new InputOutputChangeCommand(Grid);
    }

    private void ViewModel_Clicked(object sender, EventArgs e)
    {
        ExternalPortViewModel viewModel = sender as ExternalPortViewModel;
        if (viewModel == null) return;
        ControlMenu.ConnectToPort(viewModel);
    }
}
