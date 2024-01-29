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
        public GridManager grid { get; }
        public MoveSliderCommand(GridManager mainGrid)
        {
            grid = mainGrid;
        }

        public bool CanExecute(object parameter)
        {
            return (parameter is MoveSliderCommandArgs moveParams
                && grid.GetComponentAt(moveParams.gridX, moveParams.gridY)?.SliderMap
                .TryGetValue(moveParams.sliderNumber,out Slider _) != null);
        }

        public Task ExecuteAsync(object parameter)
        {
            if (!CanExecute(parameter)) return default;
            var moveParams = (MoveSliderCommandArgs)parameter;
            var sliderComponent = grid.GetComponentAt(moveParams.gridX, moveParams.gridY);
            // update the slider reference of the component
            if (sliderComponent.SliderMap.ContainsKey(moveParams.sliderNumber))
            {
                sliderComponent.SliderMap[moveParams.sliderNumber].Value = moveParams.newValue;
            } else
            {
                throw new ArgumentException("the specified SliderNumber does not exist in the Model.Grid");
            }
            // also update the Slidervalue in the SMatrix.. this should be cleaned up sooner or later
            foreach(var sMatrix in sliderComponent.WaveLengthToSMatrixMap.Values)
            {
                var sliderID = sliderComponent.SliderMap[moveParams.sliderNumber].ID;
                if (sMatrix.SliderReference.ContainsKey(sliderID))
                {
                    sMatrix.SliderReference[sliderID] = moveParams.newValue;
                } else
                {
                    sMatrix.SliderReference.Add(sliderID, moveParams.newValue);
                }
            }
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
