using CAP_Core.Components;
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
        public GridManager Grid { get; }
        public MoveSliderCommand(GridManager mainGrid)
        {
            Grid = mainGrid;
        }

        public bool CanExecute(object parameter)
        {
            return (parameter is MoveSliderCommandArgs moveParams
                && Grid.ComponentMover.GetComponentAt(moveParams.gridX, moveParams.gridY)?.GetAllSliders().SingleOrDefault(s=>s.Number== moveParams.sliderNumber)!= null);
        }

        public Task ExecuteAsync(object parameter)
        {
            if (!CanExecute(parameter)) 
                return Task.CompletedTask;
            var moveParams = (MoveSliderCommandArgs)parameter;
            var sliderComponent = Grid.ComponentMover.GetComponentAt(moveParams.gridX, moveParams.gridY);
            sliderComponent.GetSlider(moveParams.sliderNumber).Value = moveParams.newValue;
            // also update the SliderValue in the SMatrix.. this should be cleaned up sooner or later
           
            return Task.CompletedTask;
        }
    }

    public class MoveSliderCommandArgs
    {
        public readonly int gridX;
        public readonly int gridY;
        public readonly int sliderNumber;
        public readonly double newValue;

        public MoveSliderCommandArgs(int gridX, int gridY, int sliderNumber, double newValue)
        {
            this.gridX = gridX;
            this.gridY = gridY;
            this.sliderNumber = sliderNumber;
            this.newValue = newValue;
        }
    }
}
