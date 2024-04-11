using CAP_Contracts.Logger;
using CAP_Core.CodeExporter;
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
        ICommand CreateCommand(CommandType type);
        void ClearHistory();
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
        MoveSlider,
        ExportNazca,
        SaveGrid
    }

    public class CommandFactory : ICommandFactory
    {
        private const int MaxHistoryItems = 1000;

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
        private LinkedList<ICommand> History { get; } = new();
        private Stack<ICommand> RedoStack { get; } = new();
        public LightCalculationService LightCalculationService { get; }
        public GridViewModel GridViewModel { get; private set; }

        public ICommand CreateCommand  (CommandType type)
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
                    newCommand = new InputColorChangeCommand(GridManager , GridViewModel);
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
                case CommandType.ExportNazca:
                    newCommand = new ExportNazcaCommand(new NazcaExporter(),GridManager, new FileDataAccessor());
                    break;
                case CommandType.SaveGrid:
                    newCommand = new SaveGridCommand(GridManager, new FileDataAccessor());
                    break;
                    // more cases for new command types
                default:
                    throw new ArgumentException("CommandType unknown", nameof(type));
            }
            newCommand.Executed += (object sender, EventArgs e) => {
                if (History.Count > 0 && History.Last.Value.CanMergeWith(newCommand))
                {
                    History.Last.Value.MergeWith(newCommand);
                } else
                {
                    History.AddLast(newCommand);
                }
                if (History.Count > MaxHistoryItems)
                {
                    ShrinkHistoryBy(1);
                }
            };
            RedoStack.Clear();
            return newCommand;
        }

        private void ShrinkHistoryBy(double removeItemCount)
        {
            for (int i = 0; i < removeItemCount; i++)
            {
                History.RemoveFirst();
            }
        }


        public bool Undo()
        {
            if (History.Count == 0) return false;
            var commandToUndo = History.Last.Value;
            RedoStack.Push(commandToUndo);
            commandToUndo.Undo();
            History.RemoveLast();
            return true;
        }
        public bool Redo()
        {
            if (RedoStack.Count == 0) return false;
            var commandtoRedo = RedoStack.Pop();
            History.AddLast(commandtoRedo);
            commandtoRedo.Redo();
            return true;
        }
        // you might want to erase the history after loading a new PIC
        public void ClearHistory()
        {
            History.Clear();
            RedoStack.Clear();
        }
    }
}
