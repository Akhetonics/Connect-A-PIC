using CAP_Core;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPic.LayoutWindow;
using ConnectAPIC.Scripts.ViewModel;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectAPIC.Scripts.Helpers;
using Chickensoft.AutoInject;
using SuperNodes.Types;
using CAP_Contracts.Logger;
using System.Xml.Linq;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using CAP_Core.Components;
using CAP_Core.Helpers;
using static Godot.Control;

namespace ConnectAPIC.Scripts.View.ToolBox
{
    public partial class ComponentBrush : ToolBase, IToolPreviewing
    {
        public ComponentViewFactory ComponentViewFactory;
        public float TileBorderSize { get; }
        public int ComponentTypeNr { get; }
        public TemplateTileView MousePreview { get; private set; }
        public ComponentView MousePreviewComponent { get; private set; }
        public DiscreteRotation StandardRotation { get; set; } = DiscreteRotation.R0;
        public bool LeftMouseButtonPressed { get; private set; }

        public ComponentBrush(ComponentViewFactory componentViewFactory, GridView gridView, float tileBorderSize, int componentTypeNr) : base(gridView)
        {
            if (componentViewFactory == null) Logger.PrintErr(nameof(componentViewFactory) + " did not get provided into constructor");
            if (gridView == null) Logger.PrintErr(nameof(gridView) + " did not get provided into constructor");
            ComponentViewFactory = componentViewFactory;
            TileBorderSize = tileBorderSize;
            ComponentTypeNr = componentTypeNr;
            MousePreviewComponent = ComponentViewFactory.CreateComponentView(ComponentTypeNr);
            MousePreview = CreatePreview(MousePreviewComponent);
            GridView.AddChild(MousePreview);
            MousePreview.Hide();
        }

        protected override void Activate()
        {
            // create the preview-tile that will be displayed at the mouse cursor
            MousePreview.Show();
        }

        public override void _Process(double delta)
        {
            if (IsActive)
            {
                // display the preview of the tile at the current mouse position - green if it could be placed and red if not.
                // if button is clicked, create a new element there
                Vector2 PreviewIconPosition = CalculatePreviewPosition();
                Vector2I MouseGridPos = GetMouseGridPosition();
                MousePreview.SetPosition(PreviewIconPosition);
                int compWidth = ComponentViewFactory.GetComponentDimensions(ComponentTypeNr).X;
                int compHeight = ComponentViewFactory.GetComponentDimensions(ComponentTypeNr).Y;
                if (GridViewModel.Grid.ComponentMover.IsColliding(MouseGridPos.X, MouseGridPos.Y, compWidth, compHeight))
                {
                    MousePreview.Modulate = new Color(1, 0, 0);
                }
                else
                {
                    MousePreview.Modulate = new Color(0, 1, 0);
                }
            }
        }

        private Vector2 CalculatePreviewPosition()
        {
            var tileSize = (GameManager.TilePixelSize);
            var offset = GridView.DragDropProxy.Position;
            Vector2 position = GetMouseGridPosition() * tileSize + offset;
            return position;
        }

        protected override void FreeTool()
        {
            // free the mouse-cursor-preview reticle
            MousePreview.Hide();
            IsActive = false;
        }

        // creates the smaller icon-preview for the toolbox
        public TemplateTileView CreateIcon()
        {
            var toolTilePixelSize = GameManager.TilePixelSize - TileBorderSize;
            var componentInstance = ComponentViewFactory.CreateComponentView(ComponentTypeNr);
            componentInstance.CustomMinimumSize = new Vector2(toolTilePixelSize, toolTilePixelSize);
            var componentSizeCorrection = componentInstance.GetBiggestSize() / toolTilePixelSize;
            var biggestScaleFactor = Math.Max(componentSizeCorrection.X, componentSizeCorrection.Y);
            if (biggestScaleFactor <= 0)
            {
                Logger.PrintErr("biggestScaleFactor is too small, the toolbox cannot scale this component properly of Component NR: " + ComponentTypeNr);
            }
            componentInstance.Scale /= biggestScaleFactor;
            TemplateTileView previewIcon = CreatePreview(componentInstance);
            previewIcon.CustomMinimumSize = new Vector2(toolTilePixelSize, toolTilePixelSize);
            previewIcon.MouseFilter = MouseFilterEnum.Stop;
            return previewIcon;
        }

        // create the real-size preview for the main field
        public TemplateTileView CreatePreview(ComponentView componentInstance)
        {
            TemplateTileView preview = new();
            preview.AddChild(componentInstance);
            preview.MouseFilter = Control.MouseFilterEnum.Ignore;
            preview.SetMeta(ToolIDMetaName, ID.ToString());
            return preview;
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (IsActive == false) return;
            if (@event is InputEventMouseButton mouseButtonEvent)
            {
                Vector2I gridPosition = GetMouseGridPosition();
                if (mouseButtonEvent.ButtonIndex == MouseButton.Left)
                {
                    LeftMouseButtonPressed = mouseButtonEvent.Pressed;
                    if (mouseButtonEvent.Pressed == true)
                    {
                        var createCommandParams = new CreateComponentArgs(ComponentTypeNr, gridPosition.X, gridPosition.Y, StandardRotation);
                        if (GridViewModel.CreateComponentCommand.CanExecute(createCommandParams))
                        {
                            GridViewModel.CreateComponentCommand.ExecuteAsync(createCommandParams).Wait();
                        }
                    }
                }
                else if (mouseButtonEvent.ButtonIndex == MouseButton.Right && mouseButtonEvent.Pressed)
                {
                    StandardRotation++;
                    MousePreviewComponent.RotationDegrees = StandardRotation.ToDegreesClockwise();
                }
            }

            // enabling mouse drag drawing
            else if (@event is InputEventMouseMotion )
            {
                Vector2I gridPosition = GetMouseGridPosition();
                if (LeftMouseButtonPressed)
                {   
                    var createCommandParams = new CreateComponentArgs(ComponentTypeNr, gridPosition.X, gridPosition.Y, StandardRotation);
                    if (GridViewModel.CreateComponentCommand.CanExecute(createCommandParams))
                    {
                        GridViewModel.CreateComponentCommand.ExecuteAsync(createCommandParams).Wait();
                    }
                }
            }
            HandleMiddleMouseDeleteDrawing(@event);
        }

    }
}
