using CAP_Core.Helpers;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using ConnectAPIC.Scripts.Helpers;
using ConnectAPIC.Scripts.ViewModel.CommandFactory;
using ConnectAPIC.Scripts.ViewModel.Commands;
using Godot;
using System;
using System.Linq;

namespace ConnectAPIC.Scripts.View.ToolBox
{
    public partial class SelectionTool : ToolBase, IToolPreviewing
    {
        public bool LeftMouseButtonPressed { get; private set; }
        public bool IsSelectionBoxActive { get; private set; }
        public Vector2 SelectionStartMousePos { get; private set; }
        public Vector2 SelectionEndMousePos { get; private set; }
        public Rect2 SelectionRect { get; private set; }
        public Vector2 RightClickStartXY { get; private set; }

        private const float DragThreshold = 5.0f;

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
            var parameterSelect = new BoxSelectComponentsArgs(new() { (GridStart : new IntVector(0, 0), GridEnd : new IntVector(0, 0)) }, AppendBehaviors.CreateNew);
            var parameterRemove = new BoxSelectComponentsArgs(new() { (GridStart : new IntVector(0, 0), GridEnd : new IntVector(0, 0)) }, AppendBehaviors.Remove);
            SelectItems(parameterSelect);
            SelectItems(parameterRemove);
        }

        private void SelectItems(BoxSelectComponentsArgs parameter)
        {
            var command = GridViewModel.CommandFactory.CreateCommand(CommandType.BoxSelectComponent);
            command.ExecuteAsync(parameter).Wait();
        }
        private void DeleteItems()
        {
            var parameter = new DeleteComponentArgs(GridViewModel.SelectionGroupManager.SelectedComponents.ToHashSet(), Guid.NewGuid());
            var deleteCommand = GridViewModel.CommandFactory.CreateCommand(CommandType.DeleteComponent);
            deleteCommand.ExecuteAsync(parameter).Wait();
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
            if (IsActive)
            {
            }
        }

        public override void _Input(InputEvent @event)
        {
            if (IsActive == false) return;
            // display the blue selection box
            // has to be input because the toolbox handles the MouseMotion somehow so that it will never be in unhandled input..
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
                    // ignore drag when startPoint is on top of a component
                    Vector2I gridPosition = GetMouseGridPosition();

                    if (mouseButtonEvent.Pressed)
                    {
                        SelectionStartMousePos = GridView.GetLocalMousePosition();
                        bool isInGrid = GridViewModel.Grid.TileManager.IsCoordinatesInGrid(gridPosition.X, gridPosition.Y);
                        bool isColliding = GridViewModel.Grid.ComponentMover.IsColliding(gridPosition.X, gridPosition.Y, 1, 1);
                        if (isInGrid && isColliding && SelectionTool.IsEditSelectionKeyPressed() == false)
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
                        var parameter = new BoxSelectComponentsArgs(new() { (GridStart: gridStart, GridEnd: gridEnd) }, AppendBehavior);
                        SelectItems(parameter);
                    }
                }
                if (mouseButtonEvent.ButtonIndex == MouseButton.Right)
                {
                    if (mouseButtonEvent.Pressed)
                    {
                        RightClickStartXY = mouseButtonEvent.GlobalPosition;
                    }
                    else
                    {
                        float dragDistance = RightClickStartXY.DistanceTo(mouseButtonEvent.GlobalPosition);
                        if (dragDistance <= DragThreshold)
                        {
                            TryRotateComponent(GridView.LocalToMap(GridView.GetLocalMousePosition()));
                        }
                    }
                }
            }

            // Keyboard input
            if (@event is InputEventKey eventKey && eventKey.Pressed)
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

        private void TryRotateComponent(Vector2I componentPos)
        {
            var args = new RotateComponentArgs(componentPos.X, componentPos.Y);
            var rotateCmd = GridViewModel.CommandFactory.CreateCommand(CommandType.RotateComponent);
            rotateCmd?.ExecuteAsync(args).Wait();
        }
        public static bool IsEditSelectionKeyPressed()
        {
            return Input.IsKeyPressed(Key.Shift) || Input.IsKeyPressed(Key.Ctrl) || Input.IsKeyPressed(Key.Alt);
        }

        public override void _Draw()
        {
            if (IsSelectionBoxActive && Input.IsMouseButtonPressed(MouseButton.Left))
            {
                DrawRect(SelectionRect, Colors.AliceBlue - new Color(0, 0, 0, 0.8f), true);
                DrawRect(SelectionRect, Colors.AliceBlue, false);
            }
        }
    }
}
