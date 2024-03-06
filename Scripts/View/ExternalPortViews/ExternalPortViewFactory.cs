using CAP_Core.ExternalPorts;
using Godot;
using ConnectAPIC.Scripts.ViewModel;
using ConnectAPIC.Scenes.ExternalPorts;
using System.Collections.ObjectModel;
using System.Collections.Generic;

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
            public List<ExternalPortScene> InitializeExternalPortViews(ObservableCollection<ExternalPort> ExternalPorts)
            {
                List<ExternalPortScene> portViews = new List<ExternalPortScene>();
                ExternalPortScene portView;
                foreach (var port in ExternalPorts)
                {
                    portView = ExternalPortTemplate.Instantiate<ExternalPortScene>();

                    if (port is ExternalInput input)
                    {
                        if (input.LaserType == LaserType.Red)
                        {
                            portView.SetAsInput(1, 0, 0);
                        }
                        else if (input.LaserType == LaserType.Green)
                        {
                            portView.SetAsInput(0, 1, 0);
                        }
                        else
                        {
                            portView.SetAsInput(0, 0, 1);
                        }
                    }
                    else
                    {
                        portView.SetAsOutput();
                    }

                    portView.SetPortPositionY(port.TilePositionY);

                    portView.Visible = true;
                    portView.Position = new Vector2(0, (GameManager.TilePixelSize) * port.TilePositionY);
                    portViews.Add(portView);
                }

                return portViews;
            }
        }
    }
}
