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


[SuperNode(typeof(Dependent))]
public partial class PortsContainer : Node2D
{
    public override partial void _Notification(int what);
    [Dependency] public GridManager GridManager => DependOn<GridManager>();
    [Dependency] public ExternalPortViewFactory PortViewFactory => DependOn<ExternalPortViewFactory>();

    public List<ExternalPortView> Views { get; set; } = new();
    public PackedScene ExternalPortViewTemplate { get; set; }


    //TODO: should observe Grid external ports when external ports change change Views accordingly?


    public void OnResolved()
    {
        //Initiate Views according to Grids ExternalPorts?
        GridManager.ExternalPorts.CollectionChanged += (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) =>
        {
            ExternalPortView tmpView;
            foreach (ExternalPort port in e.OldItems)
            {
                tmpView = Views.Find((view) => port.TilePositionY == view.ViewModel.TilePositionY);
                Views.Remove(tmpView);
                tmpView.ViewModel.QueueFree();
                tmpView.QueueFree();
            }
            foreach (ExternalPort port in e.NewItems)
            {
                tmpView = PortViewFactory.InitializeExternalPortView(port);
                Views.Add(tmpView);
            }
        };

        Views = PortViewFactory.InitializeExternalPortViewList(GridManager.ExternalPorts);
        Debug.Print("initialized? really?");
    }



    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //TODO: obtain gridView and place yourself next to GridView?
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
