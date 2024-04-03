using CAP_Core.Grid;
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

        public ControlMenuViewModel(GridManager grid){
            InputPowerAdjustCommand = new InputPowerAdjustCommand(grid);
            InputColorChangeCommand = new InputColorChangeCommand(grid);
            InputOutputChangeCommand = new InputOutputChangeCommand(grid);
        }
    }
}
