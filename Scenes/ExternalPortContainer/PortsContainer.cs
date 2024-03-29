using ConnectAPIC.Scenes.ExternalPorts;
using Godot;
using System;
using System.Collections.Generic;
using ConnectAPic.LayoutWindow;
using SuperNodes.Types;
using Chickensoft.AutoInject;
using System.Collections.ObjectModel;
using CAP_Core.Grid;
using CAP_Core.ExternalPorts;
using System.Diagnostics;
using CAP_Core.LightCalculation;


[SuperNode(typeof(Dependent))]
public partial class PortsContainer : Node2D
{
    public override partial void _Notification(int what);
    [Dependency] public GridManager GridManager => DependOn<GridManager>();
    [Dependency] public LightCalculationService LightCalculator => DependOn<LightCalculationService>();

    [Export] public PackedScene ExternalPortViewTemplate { get; set; }
    [Export] public PackedScene RightClickMenu {  get; set; }

    public ExternalPortViewFactory PortViewFactory { get; set; }
    public List<ExternalPortView> Views { get; set; } = new();


    public void OnResolved()
    {
        PortViewFactory = new ExternalPortViewFactory(this, GridManager, LightCalculator);

        GridManager.ExternalPorts.CollectionChanged += (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) =>
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

        Views = PortViewFactory?.InitializeExternalPortViewList(GridManager.ExternalPorts);

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
}
