using CAP_Core.ExternalPorts;
using Godot;
using ConnectAPIC.Scripts.ViewModel;
using ConnectAPIC.Scenes.ExternalPorts;
using System.Collections.ObjectModel;

namespace ConnectAPic.LayoutWindow
{
    public partial class GameManager
    {
        public class ExternalPortViewFactory
        {
            public PackedScene ExternalPortTemplate { get; set; }
            public ExternalPortViewFactory(PackedScene externalPortTemplate)
            {
                ExternalPortTemplate = externalPortTemplate;
            }
            private void InitializeExternalPortViews(ObservableCollection<ExternalPort> ExternalPorts)
            {
                ExternalPortView portView;
                foreach (var port in ExternalPorts)
                {
                    portView = ExternalPortTemplate.Instantiate<ExternalPortView>();

                    if (port is ExternalInput input)
                    {
                        if (input.LaserType == LaserType.Red)
                        {
                            portView.SetAsInput(this, 1, 0, 0);
                        }
                        else if (input.LaserType == LaserType.Green)
                        {
                            portView.SetAsInput(this, 0, 1, 0);
                        }
                        else
                        {
                            portView.SetAsInput(this, 0, 0, 1);
                        }
                    }
                    else
                    {
                        portView.SetAsOutput(new PowerMeterViewModel(Grid, port.TilePositionY, LightCalculator));
                    }

                    portView.Visible = true;
                    portView.Position = new Vector2(0, (GameManager.TilePixelSize) * port.TilePositionY);
                    portView.SetPortPositionY(port.TilePositionY);
                    Views.Add(port.TilePositionY, portView);
                    portView.Switched += PortsSwitched;
                    ExternalPortViewInitialized?.Invoke(this, portView);
                }
            }
        }
    }
}
