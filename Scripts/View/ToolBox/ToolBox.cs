using CAP_Contracts.Logger;
using Chickensoft.AutoInject;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.Scripts.Helpers;
using ConnectAPIC.Scripts.ViewModel;
using Godot;
using SuperNodes.Types;
using System;

namespace ConnectAPIC.Scenes.ToolBox
{
    [SuperNode(typeof(Dependent))]
    public partial class ToolBox : Node
    {
        public override partial void _Notification(int what);
        [Dependency] public ComponentViewFactory ComponentViewFactory => DependOn<ComponentViewFactory>();
        [Dependency] public ILogger Logger => DependOn<ILogger>();
        [Dependency] public GridViewModel GridViewModel => DependOn<GridViewModel>();
        [Export] public GridContainer gridContainer;
        public override void _Ready()
        {
            this.CheckForNull(x => x.gridContainer);
        }
        public void OnResolved()
        {
            SetAvailableTools();
            Logger?.Log(CAP_Contracts.Logger.LogLevel.Debug, "Initialized ToolBox");
        }
        public void SetAvailableTools()
        {
            if (ComponentViewFactory == null)
            {
                Logger.PrintErr("ComponentViewFactory cannot be null");
                return;
            }
            ToolViewModel toolViewModel = GridViewModel.ToolViewModel;
            // create draw-brushes for all components
            foreach( var tool in toolViewModel.Tools)
            {

            }
            var allComponentTypesNRs = ComponentViewFactory.GetAllComponentIDs();
            foreach (int typeNumber in allComponentTypesNRs)
            {
                var borderSize = gridContainer.GetThemeConstant("h_separation");
                var toolTilePixelSize = GameManager.TilePixelSize - borderSize;
                var componentInstance = ComponentViewFactory.CreateComponentView(typeNumber);
                componentInstance.CustomMinimumSize = new Vector2(toolTilePixelSize, toolTilePixelSize);
                var componentSizeCorrection = componentInstance.GetBiggestSize() / toolTilePixelSize;
                var biggestScaleFactor = Math.Max(componentSizeCorrection.X, componentSizeCorrection.Y);
                if (biggestScaleFactor <= 0)
                {
                    Logger.PrintErr("biggestScaleFactor is too small, the toolbox cannot scale this component properly of Component NR: " + typeNumber);
                }
                componentInstance.Scale /= biggestScaleFactor;
                TemplateTileView rect = new();
                rect.CustomMinimumSize = new Vector2(toolTilePixelSize, toolTilePixelSize);
                rect.AddChild(componentInstance);
                gridContainer.AddChild(rect);
            }

            // create select tool for selection/move/rotate/groupDelete

            // create power Meter tool for creating power meter Windows
        }
    }
}
