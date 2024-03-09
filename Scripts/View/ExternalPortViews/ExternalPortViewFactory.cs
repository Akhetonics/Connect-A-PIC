using CAP_Core.ExternalPorts;
using CAP_Core.Grid;
using CAP_Core.LightCalculation;
using ConnectAPIC.Scenes.ExternalPorts;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using Godot;

namespace ConnectAPic.LayoutWindow
{
    public partial class ExternalPortViewFactory
    {
        public PortsContainer PortsContainer{ get; set; }
        public GridManager Grid {  get; set; }
        public LightCalculationService LightCalculator { get; set; }
        public ExternalPortViewFactory(PortsContainer portsContainer, GridManager grid, LightCalculationService lightCalculator)
        {
            PortsContainer = portsContainer;
            LightCalculator = lightCalculator;
            Grid = grid;
        }

        public ExternalPortView InitializeExternalPortView(ExternalPort externalPort)
        {
            ExternalPortView portView = PortsContainer.ExternalPortViewTemplate.Instantiate<ExternalPortView>();
            ExternalPortViewModel portViewModel = new ExternalPortViewModel(Grid, externalPort.TilePositionY, LightCalculator);

            portView.Initialize(portViewModel);

            if (externalPort is ExternalInput input)
            {
                portViewModel.IsInput = true;
    
                if (input.LaserType == LaserType.Red)
                {
                    portViewModel.Power = new Vector3(1, 0, 0);
                }
                else if (input.LaserType == LaserType.Green)
                {
                    portViewModel.Power = new Vector3(0, 1, 0);
                }
                else
                {
                    portViewModel.Power = new Vector3(0, 0, 1);
                }
            }
            else
            {
                portViewModel.IsInput = false;
            }
    
            portView.Visible = true;
            portView.Position = new Vector2(0, (GameManager.TilePixelSize) * externalPort.TilePositionY);
            PortsContainer.AddChild(portView);
            return portView;
        }
    
        public List<ExternalPortView> InitializeExternalPortViewList(ObservableCollection<ExternalPort> ExternalPorts)
            {
                List<ExternalPortView> portViews = new();
                foreach (var port in ExternalPorts)
                {
                    portViews.Add(
                        InitializeExternalPortView(port)
                        );
                }

                return portViews;
            }
    }
}
