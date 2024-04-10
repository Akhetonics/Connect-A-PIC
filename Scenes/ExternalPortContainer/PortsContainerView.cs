using Godot;
using System.Collections.Generic;
using SuperNodes.Types;
using Chickensoft.AutoInject;
using CAP_Core.Grid;
using CAP_Core.LightCalculation;
using ConnectAPIC.Scripts.ViewModel;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.Scenes.RightClickMenu;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.Scripts.ViewModel.CommandFactory;


[SuperNode(typeof(Dependent))]
public partial class PortsContainerView : Node2D
{
    public override partial void _Notification(int what);
    [Dependency] public GridViewModel GridViewModel => DependOn<GridViewModel>();
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
        controlMenu.Initialize(GridViewModel.ControlMenuViewModel);
        this.AddChild(controlMenu);

        // connect ports to control menu
        foreach (ExternalPortViewModel viewModel in ports)
        {
            viewModel.Clicked += controlMenu.OnPortClicked;
        }
    }
}
