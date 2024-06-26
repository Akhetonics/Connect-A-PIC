using CAP_Core.ExternalPorts;
using CAP_Core.Grid;
using CAP_Core.LightCalculation;
using ConnectAPIC.Scenes.ExternalPorts;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using Godot;
using ConnectAPIC.Scripts.ViewModel;

namespace ConnectAPic.LayoutWindow
{
    public class ExternalPortViewFactory
    {
        public PortsContainerView PortsContainerView { get; set; }
        public GridManager Grid {  get; set; }
        public LightCalculationService LightCalculator { get; set; }
        public ExternalPortViewFactory(PortsContainerView portsContainer, GridManager grid, LightCalculationService lightCalculator)
        {
            PortsContainerView = portsContainer;
            LightCalculator = lightCalculator;
            Grid = grid;
        }


        /// <summary>
        /// Initializes port view and view model corresponding to given external port model
        /// </summary>
        /// <param name="externalPort">External port model for which view and view model is created</param>
        /// <returns>View model for given external port</returns>
        private ExternalPortViewModel InstantiateExternalPortView(ExternalPort externalPort)
        {
            ExternalPortView portView = PortsContainerView.ExternalPortViewTemplate.Instantiate<ExternalPortView>();
            ExternalPortViewModel portViewModel = new(Grid, externalPort, LightCalculator, externalPort.IsLeftPort);

            portView.Initialize(portViewModel);

            if (externalPort is ExternalInput input)
            {
                if (input.LaserType == LaserType.Red)
                {
                    portViewModel.Power = new (1, 0, 0);
                }
                else if (input.LaserType == LaserType.Green)
                {
                    portViewModel.Power = new (0, 1, 0);
                }
                else
                {
                    portViewModel.Power = new (0, 0, 1);
                }
            }
    
            portView.Visible = true;
            var posX = externalPort.IsLeftPort ? 0 : (GameManager.TilePixelSize) * Grid.TileManager.Width;
            portView.Position = new Vector2(posX, (GameManager.TilePixelSize) * externalPort.TilePositionY);
            PortsContainerView.AddChild(portView);
            return portViewModel;
        }

        /// <summary>
        /// Initializes list of port views and view models corresponding to the input list (ExternalPorts) of port models
        /// </summary>
        /// <param name="ExternalPorts">List of external port models</param>
        /// <returns>List of external port view models each connected to port models from the input list</returns>
        private List<ExternalPortViewModel> InstantiateExternalPortViews(ObservableCollection<ExternalPort> ExternalPorts)
            {
                List<ExternalPortViewModel> portViewModels = new();
                foreach (var port in ExternalPorts)
                {
                    portViewModels.Add( InstantiateExternalPortView(port) );
                }

                return portViewModels;
            }

        public List<ExternalPortViewModel> InstantiateExternalPortViews() {
            return InstantiateExternalPortViews(Grid.ExternalPortManager.ExternalPorts);
        }
    }
}
