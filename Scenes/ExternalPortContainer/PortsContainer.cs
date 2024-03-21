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


[SuperNode(typeof(Dependent))]
public partial class PortsContainer : Node2D
{
    public override partial void _Notification(int what);
    [Dependency] public GridManager GridManager => DependOn<GridManager>();
    [Dependency] public LightManager LightManager => DependOn<LightManager>();
    [Dependency] public LightCalculationService LightCalculator => DependOn<LightCalculationService>();

    [Export] public PackedScene ExternalPortViewTemplate { get; set; }
    [Export] public PackedScene RightClickMenu {  get; set; }

    public ExternalPortViewFactory PortViewFactory { get; set; }
    public List<ExternalPortView> Views { get; set; } = new();

    //Commands
    //TODO: this is temporary solution, needs to be removed after implementing command handling
    private SwitchOnLightCommand switchOnLightCommand;


    public void OnResolved()
    {
        PortViewFactory = new ExternalPortViewFactory(this, GridManager, LightCalculator, LightManager);

        GridManager.ExternalPortManager.ExternalPorts.CollectionChanged += (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) =>
        {
            ExternalPortView tmpView;
            foreach (ExternalPort port in e.OldItems)
            {
                tmpView = Views.Find((view) => port.TilePositionY == view.ViewModel.TilePositionY);
                Views.Remove(tmpView);

                if (tmpView.menu != null)
                    if (tmpView.ViewModel.IsInput)
                        DestructInputMenu(tmpView.menu, tmpView);
                    else
                        DestructOutputMenu(tmpView.menu);

                tmpView.RightClicked -= View_RightClicked;
                tmpView.QueueFree();
            }
            foreach (ExternalPort port in e.NewItems)
            {
                tmpView = PortViewFactory?.InitializeExternalPortView(port);
                tmpView.RightClicked += View_RightClicked;
                Views.Add(tmpView);
            }
        };

        Views = PortViewFactory?.InitializeExternalPortViewList(GridManager.ExternalPortManager.ExternalPorts);

        foreach (ExternalPortView view in Views) {
            view.RightClicked += View_RightClicked;
        }

        //TODO: again temporary solution before command handling
        if (Views.Count > 0) {
            switchOnLightCommand = new SwitchOnLightCommand(Views[0].ViewModel.Grid);
        }
    }

    private void View_RightClicked(object sender, EventArgs e)
    {
        ExternalPortView view = sender as ExternalPortView;
        if (view == null) return;

        //if menu already constructed
        if (view.menu != null)
        {
            view.menu.Visible = true;
            //TODO: adjust this
            view.menu.Position = view.Position;
            return;
        }

        if (view.ViewModel.IsInput){
            view.menu = RightClickMenu.Instantiate<ControlMenu>();
            //menu can only be constructed after its ready
            view.menu.Ready += () => ConstructInputMenu(view, view.menu);
        }
        else
        {
            view.menu = RightClickMenu.Instantiate<ControlMenu>();
            view.menu.Ready += () => ConstructOutputMenu(view, view.menu);
        }
        
        view.menu.Visible = true;
        this.AddChild(view.menu);
    }

    private void Menu_Ready()
    {
        throw new NotImplementedException();
    }

    private void ConstructInputMenu(ExternalPortView portView, ControlMenu menu){
        ExternalPortViewModel portViewModel = portView.ViewModel;

        //Port mode toggle (Input/Output Switch)
        ToggleSection ioToggle = menu.AddSection<ToggleSection>()
            .Initialize(new List<String>{ "Input", "Output" }, "Toggle port mode", "Input");
        ioToggle.PropertyChanged += InputOutputToggleHandler;

        //On/Off toggle
        OnOffSection onOff = menu.AddSection<OnOffSection>()
            .Initialize("Switch input light", portViewModel.IsLightOn);
        onOff.PropertyChanged += InputOnOffToggleHandler;
        portView.ViewModel.PropertyChanged += onOff.ToggleSubscription;

        //Color switching toggle
        ToggleSection colorToggle = menu.AddSection<ToggleSection>()
            .Initialize(new List<String> { "Red", "Green", "Blue" }, "Toggle port color", portViewModel.Power);
        colorToggle.PropertyChanged += InputColorToggleHandler;

        //Slider section
        SliderSection slider = menu.AddSection<SliderSection>()
            .Initialize("Light power", portViewModel.Power);
        slider.PropertyChanged += InputPowerSliderHandler;

        //Place menu next to portView
        //TODO: test this out properly
        menu.Position = portView.Position + new Vector2(50, 0);
    }
    private void DestructInputMenu(ControlMenu menu, ExternalPortView portView){
        //Port mode toggle (Input/Output Switch)
        ToggleSection ioToggle = menu.RemoveSection<ToggleSection>();
        ioToggle.PropertyChanged -= InputOutputToggleHandler;
        ioToggle.QueueFree();

        //On/Off toggle
        OnOffSection onOff = menu.RemoveSection<OnOffSection>();
        onOff.PropertyChanged -= InputOnOffToggleHandler;
        onOff.QueueFree();

        //Color switching toggle
        ToggleSection colorToggle = menu.RemoveSection<ToggleSection>();
        colorToggle.PropertyChanged -= InputColorToggleHandler;
        colorToggle.QueueFree();

        //Slider section
        SliderSection slider = menu.RemoveSection<SliderSection>();
        slider.PropertyChanged -= InputPowerSliderHandler;
        slider.QueueFree();

        menu.QueueFree();
    }

    private void ConstructOutputMenu(ExternalPortView portView, ControlMenu menu) {
        ExternalPortViewModel portViewModel = portView.ViewModel;

        //Port mode toggle (Input/Output Switch)
        ToggleSection ioToggle = menu.AddSection<ToggleSection>()
            .Initialize(new List<String> { "Input", "Output" }, "Toggle port mode", "Input");
        ioToggle.PropertyChanged += InputOutputToggleHandler;
        //Color infos
        InfoSection colorInfo = menu.AddSection<InfoSection>()
            .Initialize("Colors (R,G,B)", portViewModel.Power.ToString());

        //TODO: figure out how to do this using non anonymus functions(could do it with defining functions here and storing them globally)
        /*portViewModel.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
        {
            {
            if (e.PropertyName == nameof(ExternalPortViewModel.IsLightOn)
             || e.PropertyName == nameof(ExternalPortViewModel.Power))
            {
                colorInfo.Value = portViewModel.AllColorsPower();
            }
        };*/

        //Phase shift infos
        InfoSection phaseInfo = menu.AddSection<InfoSection>()
            .Initialize("Phase shift (R,G,B)", "(N/A, N/A, N/A)");

        //TODO: figure out how to do this using non anonymus functions
        /*portViewModel.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
        {
            if (e.PropertyName == nameof(ExternalPortViewModel.IsLightOn)
             || e.PropertyName == nameof(ExternalPortViewModel.Power))
            {
                //TODO: set phase shift when available
                colorInfo.Value = "(N/A, N/A, N/A)";
            }
        };*/

        //Place menu next to portView
        //TODO: test this out properly
        menu.Position = portView.Position + new Vector2(50, 0);
    }
    private void DestructOutputMenu(ControlMenu menu) {
        ToggleSection ioToggle = menu.RemoveSection<ToggleSection>();
        ioToggle.PropertyChanged -= InputOutputToggleHandler;
        ioToggle.QueueFree();

        InfoSection colorInfo = menu.RemoveSection<InfoSection>();
        colorInfo.QueueFree();

        InfoSection phaseInfo = menu.RemoveSection<InfoSection>();
        colorInfo.QueueFree();

        menu.QueueFree();
    }


    private void InputOutputToggleHandler(object sender, PropertyChangedEventArgs e)
    {
            //TODO: invoke a command which will change port from input to output

            //TODO: remove this, command should do it probably (or maybe listen to port viewModel and do it from there?)
            ((ToggleSection)sender).CycleToNextValue();
    }
    private void InputOnOffToggleHandler(object sender, PropertyChangedEventArgs e)
    {
        switchOnLightCommand.ExecuteAsync(!(sender as OnOffSection).IsOn).Wait();
    }
    private void InputColorToggleHandler(object sender, PropertyChangedEventArgs e){
        //TODO: invoke a command which will change port color

        //TODO: remove this later
        ((ToggleSection)sender).CycleToNextValue();
    }
    private void InputPowerSliderHandler(object sender, PropertyChangedEventArgs e){
        //TODO: property change here means Value of slider updated so update stengh of light
        //TODO: also update value of slider

        //TODO: remove this after commands
        SliderSection slider = (SliderSection)sender;
        slider.SetSliderValue(slider.Slider.Value);
    }
}
