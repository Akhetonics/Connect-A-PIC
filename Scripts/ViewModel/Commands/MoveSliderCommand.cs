using CAP_Core.Grid;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel;
using System.Linq;
using System.Threading.Tasks;

namespace ConnectAPIC.Scripts.ViewModel.Commands
{
    public class MoveSliderCommand : CommandBase<MoveSliderCommandArgs>
    {
        public GridManager Grid { get; }
        public Slider Slider { get; private set; }
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
            Slider = sliderComponent.GetSlider(moveParams.sliderNumber);
            OldSliderPosition = Slider.Value;
            Slider.Value = moveParams.newValue;
            return Task.CompletedTask;
        }
        public override void Undo()
        {
            var sliderComponent = Grid.ComponentMover.GetComponentAt(ExecutionParams.gridX , ExecutionParams.gridY);
            Slider = sliderComponent.GetSlider(ExecutionParams.sliderNumber);
            Slider.Value = OldSliderPosition;
        }
        public override void Redo()
        {
            base.Redo();
        }
        public override bool CanMergeWith(ICommand other)
        {
            if(other is  MoveSliderCommand moveSliderCmd)
            {
                if (   moveSliderCmd.ExecutionParams.gridX == this.ExecutionParams.gridX
                    && moveSliderCmd.ExecutionParams.gridY == this.ExecutionParams.gridY
                    && moveSliderCmd.ExecutionParams.sliderNumber == this.ExecutionParams.sliderNumber)
                {
                    return true;
                }
            }
            return false;
        }
        public override void MergeWith(ICommand other)
        {
            this.ExecutionParams = ((MoveSliderCommand)other).ExecutionParams;
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
