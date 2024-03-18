using CAP_Contracts.Logger;
using Chickensoft.AutoInject;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.Scripts.Helpers;
using ConnectAPIC.Scripts.View.ToolBox;
using ConnectAPIC.Scripts.ViewModel;
using Godot;
using SuperNodes.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConnectAPIC.Scenes.ToolBox
{
    [SuperNode(typeof(Dependent))]
    public partial class ToolBox : Node
    {
        public override partial void _Notification(int what);
        [Dependency] public ComponentViewFactory ComponentViewFactory => DependOn<ComponentViewFactory>();
        [Dependency] public ILogger Logger => DependOn<ILogger>();
        [Dependency] public ToolViewModel ToolViewModel => DependOn<ToolViewModel>();
        [Dependency] public GridView GridView => DependOn<GridView>();
        [Export] public GridContainer gridContainer;
        public override void _Ready()
        {
            this.CheckForNull(x => x.gridContainer);
        }
        public void OnResolved()
        {
            if (ComponentViewFactory == null) Logger.PrintErr("ComponentViewFactory cannot be null");

            SetAvailableTools();
            Logger?.Log(LogLevel.Debug, "Initialized ToolBox");
            ToolViewModel.PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) => {
                switch (e.PropertyName)
                {
                    case nameof(ToolViewModel.CurrentTool):
                        PaintCurrentToolGreen();
                        break;
                }
            };
        }

        private void PaintCurrentToolGreen()
        {
            // paint the previewtile green of the tool that has been set to the current tool
            // paint all the others back to normal
            var children = gridContainer.GetChildren().Cast<TextureRect>();
            foreach (var item in children)
            {
                var toolID = ComponentBrush.GetToolIDFromPreview(item);
                var tool = ToolViewModel.Tools.Single(t => t.GetID() == toolID);
                // modulate the current tool green and all the others white again
                if (tool == ToolViewModel.CurrentTool)
                {
                    item.Modulate = new Color(0, 1, 0);
                }
                else
                {
                    item.Modulate = new Color(1, 1, 1);
                }
            }
        }

        public void SetAvailableTools()
        {
            List<IToolPreviewable> tools = new();
            CreateAllComponentBrushes(tools);
            CreateSelectionTool(tools);
            MakeToolIconsClickable(tools);
            // create power Meter tool for creating power meter Windows
        }

        private static void CreateSelectionTool(List<IToolPreviewable> tools)
        {
            var selectionTool = new SelectionTool();
            tools.Add(selectionTool);
        }

        private void CreateAllComponentBrushes(List<IToolPreviewable> tools)
        {
            var allComponentTypesNRs = ComponentViewFactory.GetAllComponentIDs();
            foreach (int componentTypeNr in allComponentTypesNRs)
            {
                var borderSize = gridContainer.GetThemeConstant("h_separation");
                IToolPreviewable brush = new ComponentBrush(ComponentViewFactory, GridView, borderSize, componentTypeNr);
                tools.Add(brush);
            }
        }

        private void MakeToolIconsClickable(List<IToolPreviewable> tools)
        {
            foreach (IToolPreviewable tool in tools)
            {
                ToolViewModel.Tools.Add(tool);
                var rect = tool.CreateIcon();
                rect.Clicked += (sender, e) =>
                {
                    ToolViewModel.SetCurrentTool(tool);
                };
                gridContainer.AddChild(rect);
            }
        }

    }
}
