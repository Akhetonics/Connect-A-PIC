using CAP_Contracts.Logger;
using CAP_Core.Components.Creation;
using CAP_Core.Grid;
using CAP_Core.Helpers;
using CAP_Core.LightCalculation;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using ConnectAPIC.Scripts.View.ToolBox;
using ConnectAPIC.Scripts.ViewModel.Commands;
using System;
using System.Collections.Generic;

namespace ConnectAPIC.Scripts.ViewModel.CommandFactory
{
    public interface ICommandFactory
    {
        ICommand CreateCommand(CommandType type, params object[] parameters);
    }
    public enum CommandType
    {
        BoxSelectComponent,
        CreateComponent,
        MoveComponent,
        DeleteComponent,
        InputColorChange,
        InputOutputChange,
        InputPowerAdjust,
        RotateComponent,
        SwitchOnLight,
        LoadGrid,
        MoveSlider
    }

    public class CommandFactory :ICommandFactory
    {

        public CommandFactory(
            GridManager gridManager,
            ComponentFactory componentFactory,
            SelectionManager selectionManager,
            ILogger logger,
            LightCalculationService lightCalculationService,
            GridViewModel gridViewModel) 
        {
            GridManager = gridManager;
            ComponentFactory = componentFactory;
            SelectionManager = selectionManager;
            Logger = logger;
            LightCalculationService = lightCalculationService;
            GridViewModel = gridViewModel;
        }

        public GridManager GridManager { get; }
        public ComponentFactory ComponentFactory { get; }
        public SelectionManager SelectionManager { get; }
        public ILogger Logger { get; }
        private Stack<ICommand> History { get; } = new();
        private Stack<ICommand> RedoStack { get; } = new();
        public LightCalculationService LightCalculationService { get; }
        public GridViewModel GridViewModel { get; private set; }

        public ICommand CreateCommand  (CommandType type, params object[] parameters)
        {
            ICommand newCommand;
            switch (type)
            {
                case CommandType.BoxSelectComponent:
                    newCommand = new BoxSelectComponentsCommand(GridManager, SelectionManager);
                    break;
                case CommandType.CreateComponent:
                    newCommand = new CreateComponentCommand(GridManager, ComponentFactory);
                    break;
                case CommandType.MoveComponent:
                    newCommand = new MoveComponentCommand(GridManager, SelectionManager);
                    break;
                case CommandType.DeleteComponent:
                    newCommand = new DeleteComponentCommand(GridManager);
                    break;
                case CommandType.InputColorChange:
                    newCommand = new InputColorChangeCommand(GridManager , LightCalculationService);
                    break;
                case CommandType.InputOutputChange:
                    newCommand = new InputOutputChangeCommand(GridManager, LightCalculationService);
                    break;
                case CommandType.InputPowerAdjust:
                    newCommand = new InputPowerAdjustCommand(GridManager, LightCalculationService);
                    break;
                case CommandType.RotateComponent:
                    newCommand = new RotateComponentCommand(GridManager);
                    break;
                case CommandType.SwitchOnLight:
                    newCommand = new SwitchOnLightCommand(GridManager.LightManager);
                    break;
                case CommandType.LoadGrid:
                    newCommand = new LoadGridCommand(GridManager, new FileDataAccessor(), ComponentFactory, this.GridViewModel);
                    break;
                case CommandType.MoveSlider:
                    newCommand = new MoveSliderCommand(GridManager);
                    break;
                    // more cases for new command types
                default:
                    throw new ArgumentException("CommandType unknown", nameof(type));
            }
            newCommand.Executed += (object sender, EventArgs e) => {
                if (History.TryPop(out var previousCommand) && newCommand.CanMergeWith(previousCommand))
                {
                    previousCommand.MergeWith(newCommand);
                } else
                {
                    History.Push(newCommand);
                }
            };
            RedoStack.Clear();
            return newCommand;
        }

        public bool Undo()
        {
            if (History.Count == 0) return false;
            var commandToUndo = History.Pop();
            RedoStack.Push(commandToUndo);
            commandToUndo.Undo();
            return true;
        }
        public bool Redo()
        {
            if (RedoStack.Count == 0) return false;
            var commandtoRedo = RedoStack.Pop();
            History.Push(commandtoRedo);
            commandtoRedo.Redo();
            return true;
        }
    }
}
