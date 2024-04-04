using CAP_Core.Grid;
using CAP_Core.LightCalculation;
using ConnectAPIC.Scripts.ViewModel.Commands;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.Scenes.RightClickMenu {
    public class ControlMenuViewModel {
        public InputPowerAdjustCommand InputPowerAdjustCommand;
        public InputColorChangeCommand InputColorChangeCommand;
        public InputOutputChangeCommand InputOutputChangeCommand;

        public ControlMenuViewModel(GridManager grid, LightCalculationService lightCalculator){
            InputPowerAdjustCommand = new InputPowerAdjustCommand(grid, lightCalculator);
            InputColorChangeCommand = new InputColorChangeCommand(grid, lightCalculator);
            InputOutputChangeCommand = new InputOutputChangeCommand(grid, lightCalculator);
        }
    }
}
