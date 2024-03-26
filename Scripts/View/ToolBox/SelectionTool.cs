using CAP_Core;
using CAP_Core.Grid;
using CAP_Core.Helpers;
using Castle.Components.DictionaryAdapter.Xml;
using Castle.Core.Logging;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using ConnectAPIC.Scripts.Helpers;
using ConnectAPIC.Scripts.ViewModel.Commands;
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using static Godot.CameraFeed;

namespace ConnectAPIC.Scripts.View.ToolBox
{
    public partial class SelectionTool : ToolBase, IToolPreviewing
    {
        public bool LeftMouseButtonPressed { get; private set; }
        public bool IsSelectionBoxActive { get; private set; }
        public Vector2 SelectionStartMousePos { get; private set; }
        public Vector2 SelectionEndMousePos { get; private set; }
        public Rect2 SelectionRect { get; private set; }

        public TemplateTileView CreateIcon()
        {
            TemplateTileView preview = new();
            var toolTilePixelSize = GameManager.TilePixelSize - GameManager.TileBorderLeftDown;
            preview.Texture = ResourceLoader.Load<Texture2D>("res://GFX/SelectTool.png");
            preview.CustomMinimumSize = new Vector2(toolTilePixelSize, toolTilePixelSize);
            preview.SetMeta(ToolIDMetaName, ID.ToString());
            return preview;
        }
        public SelectionTool(GridView gridView) : base(gridView)
        { }

        protected override void FreeTool()
        {
            UnselectAll();
        }

        private void UnselectAll()
        {
            var parameterSelect = new BoxSelectComponentsArgs(new IntVector(0, 0), new IntVector(0, 0), AppendBehaviors.CreateNew);
            var parameterRemove = new BoxSelectComponentsArgs(new IntVector(0, 0), new IntVector(0, 0), AppendBehaviors.Remove);
            SelectItems(parameterSelect);
            SelectItems(parameterRemove);
        }

        private void SelectItems(BoxSelectComponentsArgs parameter)
        {
            if (GridViewModel.SelectionGroupManager.BoxSelectComponentsCommand.CanExecute(parameter))
            {
                GridViewModel.SelectionGroupManager.BoxSelectComponentsCommand.ExecuteAsync(parameter).Wait();
            }
        }
        private void DeleteItems()
        {
            var parameter = new DeleteComponentArgs(GridViewModel.SelectionGroupManager.SelectedComponents.ToList());
            if (GridViewModel.DeleteComponentCommand.CanExecute(parameter))
            {
                GridViewModel.DeleteComponentCommand.ExecuteAsync(parameter).Wait();
            }
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
            if (IsActive)
            {
                // draw the rectangle and implement selection groups
            }
        }

        public override void _Input(InputEvent @event)
        {
            if (IsActive == false) return;
            // display the blue selection box
            // has to be input because the toolbox handles the MouseMotion somehow so that it will never be in unhandledinput..
            else if (@event is InputEventMouseMotion)
            {
                if (IsSelectionBoxActive)
                {
                    SelectionEndMousePos = GridView.DragDropProxy.GetLocalMousePosition();
                    SelectionRect = new Rect2(SelectionStartMousePos, SelectionEndMousePos - SelectionStartMousePos);
                    QueueRedraw();
                }
            }
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (IsActive == false) return;
            HandleMiddleMouseDeleteDrawing(@event);
            if (@event is InputEventMouseButton mouseButtonEvent)
            {
                if (mouseButtonEvent.ButtonIndex == MouseButton.Left)
                {
                    // check that there is no component at start point -> otherwise abort..
                    LeftMouseButtonPressed = mouseButtonEvent.Pressed; // release or press the button
                    // ignore drag when startpoint is ontop of a component
                    Vector2I gridPosition = GetMouseGridPosition();
                    
                    if (mouseButtonEvent.Pressed)
                    {
                        SelectionStartMousePos = GridView.GetLocalMousePosition();
                        bool isInGrid = GridViewModel.Grid.IsInGrid(gridPosition.X, gridPosition.Y);
                        bool isColliding = GridViewModel.Grid.IsColliding(gridPosition.X, gridPosition.Y, 1, 1);
                        if (isInGrid == true && isColliding && SelectionTool.IsEditSelectionKeyPressed() == false)
                        {
                            return;
                        }
                        IsSelectionBoxActive = true;
                    }
                    else // Left Mouse Button was released
                    {
                        IsSelectionBoxActive = false;
                        SelectionEndMousePos = GridView.GetLocalMousePosition();
                        // add all items in box to selection group
                        var gridStart = GetGridPosition(SelectionStartMousePos).ToIntVector();
                        var gridEnd   = GetGridPosition(SelectionEndMousePos).ToIntVector();
                        QueueRedraw();
                        var AppendBehavior = AppendBehaviors.CreateNew;
                        if(Input.IsKeyPressed(Key.Shift) || Input.IsKeyPressed(Key.Ctrl))
                        {
                            AppendBehavior = AppendBehaviors.Append;
                        } else if (Input.IsKeyPressed(Key.Alt))
                        {
                            AppendBehavior = AppendBehaviors.Remove;
                        }
                        var parameter = new BoxSelectComponentsArgs(gridStart, gridEnd, AppendBehavior);
                        SelectItems(parameter);
                    }
                }
            }

            // Keyboard input
            if (@event is InputEventKey eventKey)
            {
                switch(eventKey.Keycode)
                {
                    case Key.Delete:
                        // delete all elements in SelectionGroup with a special groupDelete command -> or by modifying the command so it accepts groups.
                        DeleteItems();
                        break;
                    case Key.Escape:
                        IsSelectionBoxActive = false;
                        QueueRedraw();
                        break;
                }
            }
        }

        public static bool IsEditSelectionKeyPressed()
        {
            return Input.IsKeyPressed(Key.Shift) || Input.IsKeyPressed(Key.Ctrl) || Input.IsKeyPressed(Key.Alt);
        }

        public override void _Draw()
        {
            if (IsSelectionBoxActive)
            {
                DrawRect(SelectionRect, Colors.AliceBlue - new Color(0, 0, 0, 0.8f), true);
                DrawRect(SelectionRect, Colors.AliceBlue, false);
            }
        }
    }
}
