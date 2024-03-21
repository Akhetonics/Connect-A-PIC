using CAP_Core.Components;
using CAP_Core.Grid;
using CAP_Core.Helpers;
using Castle.Components.DictionaryAdapter.Xml;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using ConnectAPIC.Scripts.Helpers;
using ConnectAPIC.Scripts.ViewModel.Commands;
using Godot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        {
        }
        public override void _Process(double delta)
        {
            base._Process(delta);
            if (IsActive)
            {
                // draw the rectangle and implement selection groups
            }
        }

        public class AddToSelectionGroupCommmand : ICommand
        {
            public AddToSelectionGroupCommmand(GridManager grid)
            {
                Grid = grid;
            }

            public GridManager Grid { get; }

            public bool CanExecute(object parameter)
            {
                return parameter is AddToSelectionGroupParams;
            }

            public Task ExecuteAsync(object parameter)
            {
                if (CanExecute(parameter) == false) return Task.CompletedTask;
                throw new NotImplementedException();
            }
        }
        public class AddToSelectionGroupParams
        {
            List<IntVector> SelectedComponentsPositions = new ();
            public AddToSelectionGroupParams(List<IntVector> selectedComponentsPositions)
            {
                SelectedComponentsPositions = selectedComponentsPositions;
            }
        }
        
        public class BoxSelectComponentsCommand : ICommand
        {
            public BoxSelectComponentsCommand(GridManager grid)
            {
                Grid = grid;
            }

            public GridManager Grid { get; }

            public bool CanExecute(object parameter)
            {
                return parameter is BoxSelectComponentsParams;
            }

            public Task ExecuteAsync(object parameter)
            {
                if (CanExecute(parameter) == false) return Task.CompletedTask;
                // select all components in box
                if(parameter is BoxSelectComponentsParams box)
                {
                    // define start and end to count from low to high in case the box is upside down
                    var startX = Math.Max(box.GridStart.X, box.GridEnd.X);
                    var stopX = Math.Min(box.GridStart.X, box.GridEnd.X);
                    var startY = Math.Max(box.GridStart.Y, box.GridEnd.Y);
                    var stopY = Math.Min(box.GridStart.Y, box.GridEnd.Y);

                    // collect all new Components inside the box
                    var newSelection = new HashSet<Component>();
                    for (int x = startX; x <= stopX; x++)
                    {
                        for(int y = startY; y <= stopY; y++)
                        {
                            var newComponent = Grid.GetComponentAt(x, y);
                            if (newComponent != null)
                            {
                                newSelection.Add(newComponent);
                            }
                        }
                    }

                    // AppendBehavior.CreateNew -> fill list with new items and remove old items
                    var currentSelection = new HashSet<Component>(Grid.SelectedComponents);
                    foreach (var component in currentSelection)
                    {
                        if (!newSelection.Contains(component))
                        {
                            Grid.SelectedComponents.Remove(component);
                        }
                    }
                    // add remaining new selected components
                    foreach(var newComponent in newSelection)
                    {
                        if (!currentSelection.Contains(newComponent))
                        {
                            Grid.SelectedComponents.Add(newComponent);
                        }
                    }

                }
                return Task.CompletedTask;
            }
        }
        public enum AppendBehaviors
        {
            CreateNew,
            Append,
            Remove
        }
        public class BoxSelectComponentsParams
        {
            public BoxSelectComponentsParams(IntVector gridStart , IntVector gridEnd , AppendBehaviors appendBehaviour)
            {
                GridStart = gridStart;
                GridEnd = gridEnd;
                AppendBehaviour = appendBehaviour;
            }

            public IntVector GridStart { get; }
            public IntVector GridEnd { get; }
            public AppendBehaviors AppendBehaviour { get; }
        }

        public class SelectionGroupManager
        {
            public SelectionGroupManager(GridViewModel viewModel)
            {
                ViewModel = viewModel;
                AddToSelectionGroupCommand = new AddToSelectionGroupCommmand(viewModel.Grid);
                BoxSelectComponentsCommand = new BoxSelectComponentsCommand(viewModel.Grid);
            }

            public GridViewModel ViewModel { get; }
            public AddToSelectionGroupCommmand AddToSelectionGroupCommand { get; private set; }
            public BoxSelectComponentsCommand BoxSelectComponentsCommand { get; private set; }
        }
        public override void _Input(InputEvent @event)
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
                        bool isInGrid = GridViewModel.Grid.IsInGrid(gridPosition.X, gridPosition.Y);
                        bool isColliding = GridViewModel.Grid.IsColliding(gridPosition.X, gridPosition.Y, 1, 1);
                        if (isInGrid == true && isColliding && SelectionTool.IsEditSelectionKeyPressed() == false)
                        {
                            return;
                        }
                        IsSelectionBoxActive = true;
                        SelectionStartMousePos = GridView.DragDropProxy.GetLocalMousePosition();
                    }
                    else // Left Mouse Button was released
                    {
                        IsSelectionBoxActive = false;
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
                        var parameter = new BoxSelectComponentsParams(gridStart, gridEnd, AppendBehavior);
                        if (GridViewModel.SelectionGroupManager.BoxSelectComponentsCommand.CanExecute(parameter))
                        {
                            GridViewModel.SelectionGroupManager.BoxSelectComponentsCommand.ExecuteAsync(parameter).Wait();
                        }
                    }
                }
            }
            // display the blue selection box
            else if (@event is InputEventMouseMotion)
            {
                if (IsSelectionBoxActive)
                {
                    SelectionEndMousePos = GridView.DragDropProxy.GetLocalMousePosition();
                    SelectionRect = new Rect2(SelectionStartMousePos, SelectionEndMousePos - SelectionStartMousePos);
                    QueueRedraw();
                }
            }

            // Keyboard input
            if (@event is InputEventKey eventKey)
            {
                switch(eventKey.Keycode)
                {
                    case Key.Delete:
                        // delete all elements in SelectionGroup with a special groupDelete command -> or by modifying the command so it accepts groups.
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
