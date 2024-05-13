using CAP_Core.Components;
using CAP_Core.Components.Creation;
using CAP_Core.Grid;
using ConnectAPIC.Scripts.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConnectAPIC.LayoutWindow.ViewModel.Commands
{

    public class CreateComponentCommand : CommandBase<CreateComponentArgs>
    {
        private GridManager GridModel;
        private readonly IComponentFactory ComponentFactory;

        public CreateComponentCommand(GridManager mainGrid , IComponentFactory componentFactory)
        {
            this.GridModel = mainGrid;
            this.ComponentFactory = componentFactory;
        }
        
        public override bool CanExecute(object parameter)
        {
            if( parameter is CreateComponentArgs args)
            {
                foreach(var componentDef in args.ComponentDefinitions)
                {
                    var dimensions = ComponentFactory.GetDimensions(componentDef.ComponentTypeNumber);
                    if (GridModel == null ||
                    GridModel.ComponentMover.IsColliding(componentDef.GridX, componentDef.GridY, dimensions.X, dimensions.Y))
                    {
                        return false;
                    }
                }
            } else
            {
                return false;
            }
            return true;
        }

        internal override Task ExecuteAsyncCmd(CreateComponentArgs parameter)
        {
            foreach( var componentDef in parameter.ComponentDefinitions)
            {
                Component component = ComponentFactory.CreateComponent(componentDef.ComponentTypeNumber);
                component.Rotation90CounterClock = componentDef.Rotation;
                GridModel.ComponentMover.PlaceComponent(componentDef.GridX, componentDef.GridY, component);
            }
            return Task.CompletedTask;
        }

        public override void Undo()
        {
            if (ExecutionParams == null || ExecutionParams.ComponentDefinitions == null) return;
            foreach (var componentDef in ExecutionParams.ComponentDefinitions)
            {
                GridModel.ComponentMover.UnregisterComponentAt(componentDef.GridX, componentDef.GridY);
            }
        }

        public override bool CanMergeWith(ICommand other)
        {
            if (other is CreateComponentCommand createComponentCmd)
            {
                if (createComponentCmd.ExecutionParams.StrokeID == this.ExecutionParams.StrokeID )
                {
                    return true;
                }
            }
            return false;
        }
        public override void MergeWith(ICommand other)
        {
            ExecutionParams.ComponentDefinitions.AddRange(((CreateComponentCommand)other).ExecutionParams.ComponentDefinitions);
        }
    }

    public class CreateComponentArgs
    {
        public CreateComponentArgs(int componentTypeNumber, int gridX, int gridY, DiscreteRotation rotation, Guid StrokeID)
        {
            this.ComponentDefinitions = new()
            {
                new(componentTypeNumber,gridX,gridY,rotation)
            };
            this.StrokeID = StrokeID;
        }

        public List<ComponentDefinition> ComponentDefinitions { get; }
        public Guid StrokeID { get; }
    }
    public class ComponentDefinition
    {
        public ComponentDefinition(int componentTypeNumber, int gridX, int gridY, DiscreteRotation rotation)
        {
            this.ComponentTypeNumber = componentTypeNumber;
            this.GridX = gridX;
            this.GridY = gridY;
            this.Rotation = rotation;
        }

        public int ComponentTypeNumber { get; }
        public int GridX { get; }
        public int GridY { get; }
        public DiscreteRotation Rotation { get; }
    }
}
