using CAP_Core.ExternalPorts;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.Scripts.Helpers;
using ConnectAPIC.Scripts.ViewModel;
using Godot;
using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static ConnectAPic.LayoutWindow.GameManager;


namespace ConnectAPIC.Scenes.ExternalPorts
{
    public partial class ExternalPortView: Node
    {
        public ExternalPortViewModel ViewModel { get; set; }
        public PowerMeterViewModel PowerMeterViewModel {  get; set; }
        public ExternalPortViewFactory portFactory { get; set; }

        public event EventHandler<int> Switched;

        private List<ExternalPortScene> portScenes;

        bool lightsOn = true;

        public ExternalPortView(PackedScene externalPortTemplate)
        {
            portFactory = new ExternalPortViewFactory(externalPortTemplate);

            ViewModel.ExternalPorts.CollectionChanged += (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => {
                /* TODO: there can be more complex way to do it so that we changeo only one scene
                 * but it's not economical for 10-20 prots so this needs to be discussed
                foreach ( var removedItem in e.OldItems)
                {
                
                }
                foreach( var newItem in e.NewItems)
                {

                }*/

                foreach (var scene in portScenes)
                {
                    scene.QueueFree();
                }
                portScenes.Clear();

                portScenes = portFactory.InitializeExternalPortViews(ViewModel.ExternalPorts);
            };

            portScenes = portFactory.InitializeExternalPortViews(ViewModel.ExternalPorts);
        }

        public override void _Ready()
        {
        }

        public override void _Process(double delta)
        {
            if (lightsOn && isInput)
            {
            }
        }


        [Export] public RichTextLabel InfoLabel { get; set; }
        public PowerMeterViewModel ViewModel { get; set; }
        public PowerMeterView() // should be empty for duplicate to work
        {
        }
        public void Initialize(PowerMeterViewModel viewModel)
        {
            ViewModel = viewModel;
            viewModel.PowerChanged += (object sender, string e) => DisplayPower(e);
        }

        public void DisplayPower(string labelText)
        {
            InfoLabel.Text = labelText;
        }
    }

}
}












