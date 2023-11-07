using CAP_Core;
using CAP_Core.Component.ComponentHelpers;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.Scripts.ViewModel.ComponentFactory;
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
                ComponentBaseView comp = ComponentViewFactory.Instance.CreateComponentView(args.ComponentViewType);
                int width = comp.WidthInTiles;
                int height = comp.HeightInTiles;
                comp.QueueFree();
                if (GridModel != null && GridModel.IsColliding(args.Gridx, args.Gridy, width, height) ==false){
                    return true;
                }
            }
            return false;
        }

        public void Execute(object parameter)
        {
            if (CanExecute(parameter) == false) return;
            var compParams = (CreateComponentArgs)parameter;
            var newComponentType = ComponentViewModelTypeConverter.ToModel(compParams.ComponentViewType);
            ComponentBase component = ComponentFactory.Instance.CreateComponent(newComponentType);
            GridModel.PlaceComponent(compParams.Gridx, compParams.Gridy, component);
        }
    }
    public class CreateComponentArgs
    {
        public readonly Type ComponentViewType;
        public readonly int Gridx;
        public readonly int Gridy;
        public readonly DiscreteRotation Rotation;

        public CreateComponentArgs(Type componentViewType, int gridx, int gridy, DiscreteRotation rotation)
        {
            this.ComponentViewType = componentViewType;
            this.Gridx = gridx;
            this.Gridy = gridy;
            this.Rotation = rotation;
        }
    }
}
