using CAP_Core.Components;
using CAP_Core.Components.Creation;
using CAP_Core.Grid;
using ConnectAPIC.Scripts.View.ToolBox;
using ConnectAPIC.Scripts.ViewModel.Commands;
using System;
using System.Threading.Tasks;

namespace ConnectAPIC.LayoutWindow.ViewModel.Commands
{

    public class CreateComponentCommand : CommandBase<CreateComponentArgs>
    {
        private GridManager GridModel;
        private readonly ComponentFactory ComponentFactory;

        public CreateComponentCommand(GridManager mainGrid , ComponentFactory componentFactory)
        {
            this.GridModel = mainGrid;
            this.ComponentFactory = componentFactory;
        }
        
        public override bool CanExecute(object parameter)
        {
            if( parameter is CreateComponentArgs args)
            {
                var dimensions = ComponentFactory.GetDimensions(args.ComponentTypeNumber);
                if (GridModel != null && !GridModel.ComponentMover.IsColliding(args.GridX, args.GridY, dimensions.X, dimensions.Y))
                {
                    return true;
                }
            }
            return false;
        }

        internal override Task ExecuteAsyncCmd(CreateComponentArgs parameter)
        {
            Component component = ComponentFactory.CreateComponent(parameter.ComponentTypeNumber);
            component.Rotation90CounterClock = parameter.Rotation;
            GridModel.ComponentMover.PlaceComponent(parameter.GridX, parameter.GridY, component);
            return Task.CompletedTask;
        }

        public override void Undo()
        {
            if (ExecutionParams != null) return;
            GridModel.ComponentMover.UnregisterComponentAt(ExecutionParams.GridX , ExecutionParams.GridY);
        }

    }
    public class CreateComponentArgs
    {
        public readonly int ComponentTypeNumber;
        public readonly int GridX;
        public readonly int GridY;
        public readonly DiscreteRotation Rotation;

        public CreateComponentArgs(int componentTypeNumber, int gridX, int gridY, DiscreteRotation rotation)
        {
            this.ComponentTypeNumber = componentTypeNumber;
            this.GridX = gridX;
            this.GridY = gridY;
            this.Rotation = rotation;
        }
    }
}
