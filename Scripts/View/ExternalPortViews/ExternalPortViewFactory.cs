using CAP_Core.ExternalPorts;
using CAP_Core.Grid;
using CAP_Core.LightCalculation;
using ConnectAPIC.Scenes.ExternalPorts;
using Godot;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ConnectAPic.LayoutWindow
{
    public class ExternalPortViewFactory
    {
        public PackedScene ExternalPortTemplate { get; set; }
        public GridManager Grid {  get; set; }
        public LightCalculationService LightCalculator { get; set; }
        public ExternalPortViewFactory(PackedScene externalPortTemplate, GridManager grid, LightCalculationService lightCalculator)
        {
            ExternalPortTemplate = externalPortTemplate;
            LightCalculator = lightCalculator;
            Grid = grid;
        }
    
        public ExternalPortView InitializeExternalPortView(ExternalPort externalPort)
        {
            ExternalPortView portView = ExternalPortTemplate.Instantiate<ExternalPortView>();
            ExternalPortViewModel portViewModel = new ExternalPortViewModel(Grid, externalPort.TilePositionY, LightCalculator);
            portView.Initialize(portViewModel);
            
            if (externalPort is ExternalInput input)
            {
                portViewModel.IsInput = true;
    
                if (input.LaserType == LaserType.Red)
                {
                    portViewModel.SetPower(red: 1);
                }
                else if (input.LaserType == LaserType.Green)
                {
                    portViewModel.SetPower(green: 1);
                }
                else
                {
                    portViewModel.SetPower(blue: 1);
                }
            }
            else
            {
                portViewModel.IsInput = false;
            }
    
            portView.Visible = true;
            portView.Position = new Vector2(0, (GameManager.TilePixelSize) * externalPort.TilePositionY);
    
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
