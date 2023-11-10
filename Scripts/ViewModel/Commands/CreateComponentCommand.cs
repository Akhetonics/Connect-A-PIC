using CAP_Core;
using CAP_Core.Component.ComponentHelpers;
using ConnectAPIC.LayoutWindow.View;
using System;
using System.Windows.Input;

namespace ConnectAPIC.LayoutWindow.ViewModel.Commands
{

    public class CreateComponentCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Grid GridModel;
        public CreateComponentCommand(Grid mainGrid )
        {
            this.GridModel = mainGrid;
        }
        
        public bool CanExecute(object parameter)
        {
            if( parameter is CreateComponentArgs args)
            {
                var dimensions = ComponentViewFactory.Instance.GetComponentDimensions(args.ComponentTypeNumber);
                if (GridModel != null && !GridModel.IsColliding(args.Gridx, args.Gridy, dimensions.X, dimensions.Y))
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
            Component component = ComponentFactory.Instance.CreateComponent(compParams.ComponentTypeNumber);
            GridModel.PlaceComponent(compParams.Gridx, compParams.Gridy, component);
        }
    }
    public class CreateComponentArgs
    {
        public readonly int ComponentTypeNumber;
        public readonly int Gridx;
        public readonly int Gridy;
        public readonly DiscreteRotation Rotation;

        public CreateComponentArgs(int componentTypeNumber, int gridx, int gridy, DiscreteRotation rotation)
        {
            this.ComponentTypeNumber = componentTypeNumber;
            this.Gridx = gridx;
            this.Gridy = gridy;
            this.Rotation = rotation;
        }
    }
}
