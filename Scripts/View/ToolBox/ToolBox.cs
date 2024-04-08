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

        public SelectionTool MySelectionTool { get; private set; }

        [Export] public GridContainer gridContainer;
        public override void _Ready()
        {
            this.CheckForNull(x => x.gridContainer, Logger);
        }
        public void OnResolved()
        {
            if (ComponentViewFactory == null) Logger.PrintErr("ComponentViewFactory cannot be null");
            
            ToolViewModel.PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) => {
                switch (e.PropertyName)
                {
                    case nameof(ToolViewModel.CurrentTool):
                        PaintCurrentToolGreen();
                        break;
                }
            };

            SetAvailableTools();
            Logger?.Log(LogLevel.Debug, "Initialized ToolBox");
        }

        private void PaintCurrentToolGreen()
        {
            // paint the preview tile green of the tool that has been set to the current tool
            // paint all the others back to normal
            var children = gridContainer.GetChildren().Cast<TextureRect>();
            foreach (var item in children)
            {
                Guid toolID;
                try
                {
                    toolID = ComponentBrush.GetToolIDFromPreview(item);
                } catch (FormatException)
                {
                    Logger.PrintErr($"Guid was not properly stored in Meta: {item.GetMeta(ToolBase.ToolIDMetaName)}");
                    continue;
                }
                var tool = ToolViewModel.Tools.SingleOrDefault(t => t.ID == toolID);
                if (tool == null)
                {
                    Logger.PrintErr($"Tool not found in ToolViewModel - Guid is not registered: ID: {toolID} Item: {item}");
                }
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
            try
            {
                List<IToolPreviewing> tools = new();
                CreateSelectionTool(tools);
                CreateAllComponentBrushes(tools);
                MakeToolIconsClickable(tools);
                ToolViewModel.SetCurrentTool(MySelectionTool);
            } catch (Exception ex)
            {
                Logger.PrintErr(ex.ToString());
            }
            
            // create power Meter tool for creating power meter Windows
        }

        private void CreateSelectionTool(List<IToolPreviewing> tools)
        {
            MySelectionTool = new SelectionTool(GridView);
            tools.Add(MySelectionTool);
        }

        private void CreateAllComponentBrushes(List<IToolPreviewing> tools)
        {
            var allComponentTypesNRs = ComponentViewFactory.GetAllComponentIDs();
            foreach (int componentTypeNr in allComponentTypesNRs)
            {
                var borderSize = gridContainer.GetThemeConstant("h_separation");
                IToolPreviewing brush = new ComponentBrush(ComponentViewFactory, GridView, borderSize, componentTypeNr);
                tools.Add(brush);
            }
        }

        private void MakeToolIconsClickable(List<IToolPreviewing> tools)
        {
            foreach (IToolPreviewing tool in tools)
            {
                ToolViewModel.Tools.Add(tool);
                var rect = tool.CreateIcon();
                rect.Clicked += (sender, e) =>
                {
                    try
                    {
                        ToolViewModel.SetCurrentTool(tool);
                    } catch (Exception ex)
                    {
                        Logger.PrintErr(""+ex);
                    }
                };
                gridContainer.AddChild(rect);
            }
        }

        public override void _Input(InputEvent @event)
        {
            if(@event is InputEventKey eventKey)
            {
                // fall back to selection tool if esc is pressed
                if(eventKey.Pressed && eventKey.Keycode == Key.Escape && ToolViewModel.CurrentTool != MySelectionTool)
                {
                    ToolViewModel.SetCurrentTool(MySelectionTool);
                    GetViewport().SetInputAsHandled();
                }
            }
        }
    }
}
