using CAP_Core.Components;
using CAP_Core.Components.Creation;
using CAP_Core.Grid;
using ConnectAPIC.Scripts.ViewModel.Commands;
using System;
using System.Threading.Tasks;

namespace ConnectAPIC.LayoutWindow.ViewModel.Commands
{

    public class CreateComponentCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private GridManager GridModel;
        private readonly ComponentFactory ComponentFactory;

        public CreateComponentCommand(GridManager mainGrid , ComponentFactory componentFactory)
        {
            this.GridModel = mainGrid;
            this.ComponentFactory = componentFactory;
        }
        
        public bool CanExecute(object parameter)
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

        public Task ExecuteAsync(object parameter)
        {
            if ( !CanExecute(parameter) ) return default;
            var compParams = (CreateComponentArgs)parameter;
            Component component = ComponentFactory.CreateComponent(compParams.ComponentTypeNumber);
            component.Rotation90CounterClock = compParams.Rotation;
            GridModel.ComponentMover.PlaceComponent(compParams.GridX, compParams.GridY, component);
            return Task.CompletedTask;

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
