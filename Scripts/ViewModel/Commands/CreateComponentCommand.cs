using CAP_Core;
using CAP_Core.Components;
using CAP_Core.Components.Creation;
using System;
using System.Windows.Input;

namespace ConnectAPIC.LayoutWindow.ViewModel.Commands
{

    public class CreateComponentCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Grid GridModel;
        private readonly ComponentFactory ComponentFactory;

        public CreateComponentCommand(Grid mainGrid , ComponentFactory componentFactory)
        {
            this.GridModel = mainGrid;
            this.ComponentFactory = componentFactory;
        }
        
        public bool CanExecute(object parameter)
        {
            if( parameter is CreateComponentArgs args)
            {
                var dimensions = ComponentFactory.GetDimensions(args.ComponentTypeNumber);
                if (GridModel != null && !GridModel.IsColliding(args.GridX, args.GridY, dimensions.X, dimensions.Y))
                {
                    return true;
                }
            }
            return false;
        }

        public void Execute(object parameter)
        {
            if ( !CanExecute(parameter) ) return;
            var compParams = (CreateComponentArgs)parameter;
            Component component = ComponentFactory.CreateComponent(compParams.ComponentTypeNumber);
            component.Rotation90CounterClock = compParams.Rotation;
            GridModel.PlaceComponent(compParams.GridX, compParams.GridY, component);

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
