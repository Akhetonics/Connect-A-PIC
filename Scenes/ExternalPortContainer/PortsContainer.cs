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

    //private void ReconstructMenu(ExternalPortView view, bool is_input)
    //{
    //    if (is_input) DestructInputMenu(view);
    //    else DestructOutputMenu(view);
    //    Vector2 oldPosition = view.menu.Position;
    //    view.menu = null;
    //    ConstructMenu(view, oldPosition);
    //}
    //private void ConstructMenu(ExternalPortView view)
    //{
    //    ConstructMenu(view, view.Position);
    //}

    //private void ConstructMenu(ExternalPortView view, Vector2 position)
    //{
    //    if (view == null) return;

    //    //if menu already constructed
    //    if (view.menu != null)
    //    {
    //        view.menu.Visible = true;
    //        //TODO: adjust this
    //        view.menu.Position = position;
    //        return;
    //    }

    //    if (view.ViewModel.IsInput)
    //    {
    //        view.menu = RightClickMenu.Instantiate<ControlMenu>();
    //        //menu can only be constructed after its ready
    //        view.menu.Ready += () => ConstructInputMenu(view);
    //    }
    //    else
    //    {
    //        view.menu = RightClickMenu.Instantiate<ControlMenu>();
    //        view.menu.Ready += () => ConstructOutputMenu(view);
    //    }

    //    view.menu.Visible = true;
    //    this.AddChild(view.menu);
    //    view.menu.Position = position;
    //}

    //private static void ConstructInputMenu(ExternalPortView portView)
    //{
    //    ControlMenu menu = portView.menu;
    //    ExternalPortViewModel portViewModel = portView.ViewModel;

    //    //Port mode toggle (Input/Output Switch)
    //    ToggleSection ioToggle = menu.AddSection<ToggleSection>();
    //    ioToggle.Initialize(new List<String>{ "Input", "Output" }, "Toggle port mode", "Input");
    //    ioToggle.PropertyChangeHandler = (object sender, PropertyChangedEventArgs e) =>
    //        inputOutputChangeCommand.ExecuteAsync(portView.ViewModel).Wait();
    //    ioToggle.PropertyChanged += ioToggle.PropertyChangeHandler;

    //    //On/Off toggle
    //    OnOffSection onOff = menu.AddSection<OnOffSection>();
    //    onOff.Initialize("Switch input light", portViewModel.IsLightOn);
    //    onOff.PropertyChangeHandler = (object sender, PropertyChangedEventArgs e) =>
    //        switchOnLightCommand.ExecuteAsync(!(sender as OnOffSection).IsOn).Wait();
    //    onOff.PropertyChanged += onOff.PropertyChangeHandler;
    //    portView.ViewModel.PropertyChanged += onOff.ToggleSubscription;

    //    //Color switching toggle
    //    ToggleSection colorToggle = menu.AddSection<ToggleSection>();
    //    colorToggle.Initialize(new List<String> { "Red", "Green", "Blue" }, "Toggle port color", portViewModel.Power);
    //    colorToggle.PropertyChangeHandler = (object sender, PropertyChangedEventArgs e) =>
    //        inputColorChangeCommand.ExecuteAsync(new InputColorChangeArgs(portViewModel.PortModel as ExternalInput, colorToggle.GetNextToggleValue())).Wait();
    //    colorToggle.PropertyChanged += colorToggle.PropertyChangeHandler;
    //    portView.ViewModel.PropertyChanged += colorToggle.ToggleValueSubscription;

    //    //Slider section
    //    SliderSection slider = menu.AddSection<SliderSection>();
    //    slider.Initialize("Light power", portViewModel.Power);
    //    slider.PropertyChangeHandler = (sender, e) =>
    //        inputPowerAdjustCommand.ExecuteAsync(new InputPowerAdjustArgs(portViewModel.PortModel, slider.Slider.Value)).Wait();
    //    slider.PropertyChanged += slider.PropertyChangeHandler;
    //    portView.ViewModel.PropertyChanged += slider.ValueChangeSubscription;
    //}
    //private static void DestructInputMenu(ExternalPortView portView)
    //{
    //    ControlMenu menu = portView.menu;

    //    //Port mode toggle (Input/Output Switch)
    //    ToggleSection ioToggle = menu.RemoveSection<ToggleSection>();
    //    ioToggle.PropertyChanged -= ioToggle.PropertyChangeHandler;
    //    ioToggle.QueueFree();

    //    //On/Off toggle
    //    OnOffSection onOff = menu.RemoveSection<OnOffSection>();
    //    onOff.PropertyChanged -= onOff.PropertyChangeHandler;
    //    portView.ViewModel.PropertyChanged -= onOff.ToggleSubscription;
    //    onOff.QueueFree();

    //    //Color switching toggle
    //    ToggleSection colorToggle = menu.RemoveSection<ToggleSection>();
    //    colorToggle.PropertyChanged -= colorToggle.PropertyChangeHandler;
    //    colorToggle.QueueFree();

    //    //Slider section
    //    SliderSection slider = menu.RemoveSection<SliderSection>();
    //    slider.PropertyChanged -= slider.PropertyChangeHandler;
    //    portView.ViewModel.PropertyChanged -= slider.ValueChangeSubscription;
    //    slider.QueueFree();

    //    menu.QueueFree();
    //}
    //private static void ConstructOutputMenu(ExternalPortView portView)
    //{
    //    ControlMenu menu = portView.menu;
    //    ExternalPortViewModel portViewModel = portView.ViewModel;

    //    //Port mode toggle (Input/Output Switch)
    //    ToggleSection ioToggle = menu.AddSection<ToggleSection>();
    //    ioToggle.Initialize(new List<String> { "Input", "Output" }, "Toggle port mode", "Input");
    //    ioToggle.PropertyChangeHandler = (object sender, PropertyChangedEventArgs e) =>
    //        inputOutputChangeCommand.ExecuteAsync(portView.ViewModel.TilePositionY).Wait();
    //    ioToggle.PropertyChanged += ioToggle.PropertyChangeHandler;

    //    //Color infos
    //    InfoSection colorInfo = menu.AddSection<InfoSection>();
    //    colorInfo.Initialize("Colors (R,G,B)", portViewModel.Power.ToString());

    //    //TODO: figure out how to do this using non anonymus functions(could do it with defining functions here and storing them globally)
    //    /*portViewModel.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
    //    {
    //        {
    //        if (e.PropertyName == nameof(ExternalPortViewModel.IsLightOn)
    //         || e.PropertyName == nameof(ExternalPortViewModel.Power))
    //        {
    //            colorInfo.Value = portViewModel.AllColorsPower();
    //        }
    //    };*/

    //    //Phase shift infos
    //    InfoSection phaseInfo = menu.AddSection<InfoSection>();
    //    phaseInfo.Initialize("Phase shift (R,G,B)", "(N/A, N/A, N/A)");

    //    //TODO: figure out how to do this using non anonymus functions
    //    /*portViewModel.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
    //    {
    //        if (e.PropertyName == nameof(ExternalPortViewModel.IsLightOn)
    //         || e.PropertyName == nameof(ExternalPortViewModel.Power))
    //        {
    //            //TODO: set phase shift when available
    //            colorInfo.Value = "(N/A, N/A, N/A)";
    //        }
    //    };*/
    //}
    //private static void DestructOutputMenu(ExternalPortView portView)
    //{
    //    ControlMenu menu = portView.menu;
    //    ToggleSection ioToggle = menu.RemoveSection<ToggleSection>();
    //    ioToggle.PropertyChanged -= ioToggle.PropertyChangeHandler;
    //    ioToggle.QueueFree();

    //    InfoSection colorInfo = menu.RemoveSection<InfoSection>();
    //    colorInfo.QueueFree();

    //    InfoSection phaseInfo = menu.RemoveSection<InfoSection>();
    //    colorInfo.QueueFree();

    //    menu.QueueFree();
    //}

}
