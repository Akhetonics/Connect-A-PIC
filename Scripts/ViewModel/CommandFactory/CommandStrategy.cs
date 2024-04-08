using CAP_Contracts.Logger;
using CAP_Core.CodeExporter;
using CAP_Core.Components.Creation;
using CAP_Core.Grid;
using CAP_Core.Helpers;
using CAP_Core.LightCalculation;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using ConnectAPIC.Scripts.View.ComponentFactory;
using ConnectAPIC.Scripts.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConnectAPIC.Scripts.View.ToolBox.SelectionTool;

namespace ConnectAPIC.Scripts.ViewModel.CommandFactory
{
    public interface ICommandStrategy
    {
        ICommand CreateCommand(CommandType type, params object[] parameters);
    }
    public enum CommandType
    {
        CreateComponent,
        MoveComponent,
        DeleteComponent,
        RotateComponent,
        SwitchOnLight,
        ExportToNazca,
        SaveGrid,
        LoadGrid,
        MoveSlider
        // add more commandtypes here
    }

    public class CommandStrategy :ICommandStrategy
    {

        public CommandStrategy(
            GridManager gridManager,
            ComponentFactory componentFactory,
            SelectionGroupManager selectionGroupManager,
            ILogger logger,
            LightCalculationService lightCalculationService,
            GridViewModel gridViewModel) 
        {
            GridManager = gridManager;
            ComponentFactory = componentFactory;
            SelectionGroupManager = selectionGroupManager;
            Logger = logger;
            LightCalculationService = lightCalculationService;
            GridViewModel = gridViewModel;
        }

        public GridManager GridManager { get; }
        public ComponentFactory ComponentFactory { get; }
        public SelectionGroupManager SelectionGroupManager { get; }
        public ILogger Logger { get; }
        public LightCalculationService LightCalculationService { get; }
        public GridViewModel GridViewModel { get; private set; }

        public ICommand CreateCommand  (CommandType type, params object[] parameters)
        {
            switch (type)
            {
                case CommandType.CreateComponent:
                    return new CreateComponentCommand(GridManager, ComponentFactory);
                case CommandType.MoveComponent:
                    return new MoveComponentCommand(GridManager, SelectionGroupManager.SelectionManager);
                case CommandType.DeleteComponent:
                    return new DeleteComponentCommand(GridManager);
                case CommandType.RotateComponent:
                    return new RotateComponentCommand(GridManager);
                case CommandType.SwitchOnLight:
                    return new SwitchOnLightCommand(GridManager.LightManager);
                case CommandType.ExportToNazca:
                    return new ExportNazcaCommand(new NazcaExporter(), GridManager, new DataAccessorGodot());
                case CommandType.SaveGrid:
                    return new SaveGridCommand(GridManager, new FileDataAccessor());
                case CommandType.LoadGrid:
                    return new LoadGridCommand(GridManager, new FileDataAccessor(), ComponentFactory, this.GridViewModel);
                case CommandType.MoveSlider:
                    return new MoveSliderCommand(GridManager);
                    // more cases for new command types
                default:
                    throw new ArgumentException("CommandType unknown", nameof(type));
            }
        }
    }
}
