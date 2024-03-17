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
    }

    private void View_RightClicked(object sender, EventArgs e)
    {
        ExternalPortView view = sender as ExternalPortView;
        if (view == null) return;

        //TODO: find out if view is input or output
        //TODO: build right click menu accordingly (if haven't done it already)
        //TODO: open right click menu on that ports position (with some offset)
    }

    private void ConstructInputMenu(ExternalPortView portView){
        ExternalPortViewModel portViewModel = portView.ViewModel;
        RightClickMenu menu = RightClickMenu.Instantiate<RightClickMenu>();

        //Port mode toggle (Input/Output Switch)
        ToggleSection ioToggle = menu.AddSection<ToggleSection>()
            .Initialize(new List<String>{ "Input", "Output" }, "Toggle port mode", "Input");
        ioToggle.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
        {
            //TODO: invoke a command which will change port from input to output

            //TODO: remove this, command should do it probably (or maybe listen to port viewModel and do it from there?)
            ioToggle.CycleToNextValue();
        };

        //On/Off toggle
        OnOffSection onOff = menu.AddSection<OnOffSection>()
            .Initialize("Switch input light", portViewModel.IsLightOn);
        onOff.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
        {
            //TODO: command which will switch on/off ports

            //TODO: remove this after implementing command
            onOff.IsOn = !onOff.IsOn;
        };

        //Color switching toggle
        ToggleSection colorToggle = menu.AddSection<ToggleSection>()
            .Initialize(new List<String> { "Red", "Green", "Blue" }, "Toggle port color", portViewModel.Power);
        colorToggle.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
        {
            //TODO: invoke a command which will change port color

            //TODO: remove this later
            colorToggle.CycleToNextValue();
        };

        //Slider section
        SliderSection slider = menu.AddSection<SliderSection>()
            .Initialize("Light power", portViewModel.Power);
        slider.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
        {
            //TODO: property change here means Value of slider updated so update stengh of light
            //TODO: also update value of slider

            //TODO: remove this after commands
            slider.Value = slider.Slider.Value.ToString();
        };

        //Place menu next to portView
        //TODO: test this out properly
        menu.Position = portView.Position + new Vector2(50, 0);
    }

}
