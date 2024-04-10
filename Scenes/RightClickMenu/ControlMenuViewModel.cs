using CAP_Core.Grid;
using CAP_Core.LightCalculation;
using ConnectAPIC.Scripts.ViewModel.CommandFactory;
using ConnectAPIC.Scripts.ViewModel.Commands;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.Scenes.RightClickMenu {
    public class ControlMenuViewModel {
        public ControlMenuViewModel(GridManager grid, LightCalculationService lightCalculator, ICommandFactory commandFactory){
            Grid = grid;
            LightCalculator = lightCalculator;
            CommandFactory = commandFactory;
        }

        public GridManager Grid { get; }
        public LightCalculationService LightCalculator { get; }
        public ICommandFactory CommandFactory { get; }
    }
}
