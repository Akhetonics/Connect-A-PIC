using CAP_Core.Components;
using CAP_Core.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.Scripts.ViewModel.Commands
{
    public class MoveSliderCommand : CommandBase<MoveSliderCommandArgs>
    {
        public GridManager Grid { get; }
        public Slider OldSlider { get; private set; }
        public double OldSliderPosition { get; private set; }

        public MoveSliderCommand(GridManager mainGrid)
        {
            Grid = mainGrid;
        }

        public override bool CanExecute(object parameter)
        {
            return (parameter is MoveSliderCommandArgs moveParams
                && Grid.ComponentMover.GetComponentAt(moveParams.gridX, moveParams.gridY)?.GetAllSliders().SingleOrDefault(s=>s.Number== moveParams.sliderNumber)!= null);
        }

        internal override Task ExecuteAsyncCmd(MoveSliderCommandArgs moveParams)
        {
            var sliderComponent = Grid.ComponentMover.GetComponentAt(moveParams.gridX, moveParams.gridY);
            OldSlider = sliderComponent.GetSlider(moveParams.sliderNumber);
            OldSliderPosition = OldSlider.Value;
            OldSlider.Value = moveParams.newValue;
            return Task.CompletedTask;
        }
        public override void Undo()
        {
            OldSlider.Value = OldSliderPosition;
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
