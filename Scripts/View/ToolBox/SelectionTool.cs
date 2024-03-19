using ConnectAPic.LayoutWindow;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.Scripts.View.ToolBox
{
    public partial class SelectionTool : ToolBase, IToolPreviewing
    {
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
                
            }
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (IsActive == false) return;
            if (@event is InputEventMouseButton mouseButtonEvent)
            {
                Vector2I gridPosition = GetMouseGridPosition();
              
                if (mouseButtonEvent.ButtonIndex == MouseButton.Right && mouseButtonEvent.Pressed)
                {
                    //StandardRotation++;
                    //MousePreviewComponent.RotationDegrees = StandardRotation.ToDegreesClockwise();
                }
                else if (mouseButtonEvent.ButtonIndex == MouseButton.Middle)
                {
                    //MiddleMouseButtonPressed = mouseButtonEvent.Pressed;
                    //if (mouseButtonEvent.Pressed)
                    //{
                    //    var delParams = new DeleteComponentArgs(gridPosition.X, gridPosition.Y);
                    //    if (GridViewModel.DeleteComponentCommand.CanExecute(delParams))
                    //    {
                    //        GridViewModel.DeleteComponentCommand.ExecuteAsync(delParams).Wait();
                    //    }
                    //}
                }
            }

            // enabling mouse drag drawing
            else if (@event is InputEventMouseMotion mouseMotionEvent)
            {
                Vector2I gridPosition = GetMouseGridPosition();
                //if (LeftMouseButtonPressed)
                //{
                //    var createCommandParams = new CreateComponentArgs(ComponentTypeNr, gridPosition.X, gridPosition.Y, StandardRotation);
                //    if (GridViewModel.CreateComponentCommand.CanExecute(createCommandParams))
                //    {
                //        GridViewModel.CreateComponentCommand.ExecuteAsync(createCommandParams).Wait();
                //    }
                //}
                //else if (MiddleMouseButtonPressed)
                //{
                //    // enabling mouse drag deleting of components
                //    var delParams = new DeleteComponentArgs(gridPosition.X, gridPosition.Y);
                //    if (GridViewModel.DeleteComponentCommand.CanExecute(delParams))
                //    {
                //        GridViewModel.DeleteComponentCommand.ExecuteAsync(delParams).Wait();
                //    }
                //}

            }


        }

    }
}
