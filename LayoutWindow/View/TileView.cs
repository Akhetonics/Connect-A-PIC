using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace Tiles
{

    public partial class TileView : TextureRect
    {
        public delegate void ComponentEventHandler(TileView tile, ComponentBase component);
        public event ComponentEventHandler OnMoveComponentRequested;
        public event ComponentEventHandler OnCreateNewComponentRequested;
        public delegate void TileEventHandler(TileView tile);
        public event TileEventHandler OnDeletionRequested;
        public event TileEventHandler OnRotationRequested;
        public IComponentView ComponentView { get; set; }
        public int GridX { get; private set; }
        public int GridY { get; private set; }
        public static int TilePixelSize { get; } = 64;
       
        public new float RotationDegrees { get => base.RotationDegrees; protected set => base.RotationDegrees = value; }
        public new float Rotation { get => base.Rotation; protected set => base.Rotation = value; }

        public override void _Ready()
        {
            PivotOffset = Size / 2;
        }
        /// <summary>
        /// Register the Tile when it gets created
        /// </summary>
        public void SetPositionInGrid(int X, int Y)
        {
            GridX = X;
            GridY = Y;
        }

        public override void _GuiInput(InputEvent inputEvent)
        {
            base._GuiInput(inputEvent);
            if (inputEvent is InputEventMouseButton mouseEvent)
            {
                if (mouseEvent.Position.X < 0 || mouseEvent.Position.Y < 0 || mouseEvent.Position.X > Size.X || mouseEvent.Position.Y > Size.Y)
                {
                    return;
                }
                if (mouseEvent.ButtonIndex == MouseButton.Middle && mouseEvent.Pressed)
                {
                    OnDeletionRequested?.Invoke(this);
                }
                if (mouseEvent.ButtonIndex == MouseButton.Right && mouseEvent.Pressed)
                {
                    OnRotationRequested?.Invoke(this);
                }
            }
        }
        public override bool _CanDropData(Vector2 position, Variant data)
        {
            // extract all tiles from the component that is about to be dropped here at position and SetDragPreview them
            if (data.Obj is ComponentBase component)
            {
                ShowMultiTileDragPreview(position, component);
            }

            return true;
        }
        protected void ShowMultiTileDragPreview(Vector2 position, ComponentBase component)
        {
            var previewGrid = new GridContainer();
            previewGrid.PivotOffset = previewGrid.Size / 2f;
            var oldRotation = component.Rotation90;
            component.Rotation90 = DiscreteRotation.R0;
            previewGrid.Columns = component.WidthInTiles;
            for (int y = 0; y < component.HeightInTiles; y++)
            {
                for (int x = 0; x < component.WidthInTiles; x++)
                {
                    var componentPart = component.GetPartAt(x, y);
                    var previewtile = new TextureRect();
                    previewtile._Ready();
                    previewtile.Texture = componentPart?.Texture.Duplicate() as Texture2D;
                    previewtile.Visible = true;
                    previewGrid.AddChild(previewtile);
                }
            }
            var children = previewGrid.GetChildren();
            previewGrid.RotationDegrees = (float)oldRotation * 90f;
            component.Rotation90 = oldRotation;
            this.SetDragPreview(previewGrid);
        }
        public override void _DropData(Vector2 position, Variant data)
        {
            if (data.Obj is ComponentBase component)
            {
                if (component.IsPlacedInGrid == false)
                {
                    OnCreateNewComponentRequested?.Invoke(this, component);
                }
                else
                {
                    OnMoveComponentRequested?.Invoke(this, component);
                }
            }
        }

        public override Variant _GetDragData(Vector2 position)
        {
            return this.ComponentView;
        }
        public void ResetToDefault(Texture2D baseTexture)
        {
            Texture = baseTexture;
            PivotOffset = Size / 2;
            Visible = true;
            _discreteRotation = DiscreteRotation.R0;
            RotationDegrees = (int)Rotation90 * 90;
            ComponentView = null;
        }
        public TileView Duplicate()
        {
            var copy = base.Duplicate() as TileView;
            copy.Rotation90 = Rotation90;
            copy.RotationDegrees = RotationDegrees;
            return copy;
        }


    }
}