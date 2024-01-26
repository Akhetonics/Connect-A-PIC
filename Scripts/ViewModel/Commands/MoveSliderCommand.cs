using CAP_Core.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.Scripts.ViewModel.Commands
{
    public class MoveSliderCommand : ICommand
    {
        public GridManager grid { get; }
        public MoveSliderCommand(GridManager mainGrid)
        {
            grid = mainGrid;
        }

        public bool CanExecute(object parameter)
        {
            return (parameter is MoveSliderCommandArgs moveParams
                && grid.GetComponentAt(moveParams.gridX, moveParams.gridY)?.Sliders
                .TryGetValue(moveParams.sliderNumber,out double value) != null);
        }

        public Task ExecuteAsync(object parameter)
        {
            if (!CanExecute(parameter)) return default;
            var moveParams = (MoveSliderCommandArgs)parameter;
            grid.GetComponentAt(moveParams.gridX, moveParams.gridY).Sliders[moveParams.sliderNumber] = moveParams.newValue;
            return Task.CompletedTask;
        }
    }

    public class MoveSliderCommandArgs
    {
        public readonly int gridX;
        public readonly int gridY;
        public readonly int sliderNumber;
        public readonly double newValue;

        public MoveSliderCommandArgs(int gridX, int gridY)
        {
            this.gridX = gridX;
            this.gridY = gridY;
        }
    }
}
