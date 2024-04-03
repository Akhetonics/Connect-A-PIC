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
        public PortsContainer PortsContainer{ get; set; }
        public GridManager Grid {  get; set; }
        public LightCalculationService LightCalculator { get; set; }

        public ExternalPortViewFactory(PortsContainer portsContainer, GridManager grid, LightCalculationService lightCalculator)
        {
            PortsContainer = portsContainer;
            LightCalculator = lightCalculator;
            Grid = grid;
        }

        /// <summary>
        /// Initializes port view and view model corresponding to given external port model
        /// </summary>
        /// <param name="externalPort">External port model for which view and view model is created</param>
        /// <returns>View model for given external port</returns>
        public ExternalPortViewModel InitializeExternalPortView(ExternalPort externalPort)
        {
            ExternalPortView portView = PortsContainer.ExternalPortViewTemplate.Instantiate<ExternalPortView>();
            ExternalPortViewModel portViewModel = new ExternalPortViewModel(Grid, externalPort, LightCalculator);

            portView.Initialize(portViewModel);

            if (externalPort is ExternalInput input)
            {
                portViewModel.IsInput = true;
    
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
            else
            {
                portViewModel.IsInput = false;
            }
    
            portView.Visible = true;
            portView.Position = new Vector2(0, (GameManager.TilePixelSize) * externalPort.TilePositionY);
            PortsContainer.AddChild(portView);
            return portViewModel;
        }

        /// <summary>
        /// Initializes list of port views and view models corresponding to the input list (ExternalPorts) of port models
        /// </summary>
        /// <param name="ExternalPorts">List of external port models</param>
        /// <returns>List of external port view models each connected to port models from the input list</returns>
        public List<ExternalPortViewModel> InitializeExternalPortViewList(ObservableCollection<ExternalPort> ExternalPorts)
            {
                List<ExternalPortViewModel> portViewModels = new();
                foreach (var port in ExternalPorts)
                {
                    portViewModels.Add(
                        InitializeExternalPortView(port)
                        );
                }

                return portViewModels;
            }
    }
}
