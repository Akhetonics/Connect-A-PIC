using ConnectAPIC.Scenes.Component;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.Scenes.Tiles
{
    public abstract partial class TileDraggable : TextureRect
    {
        public delegate void ComponentEventHandler(TileDraggable tile, ComponentBase component);
        public event ComponentEventHandler OnMoveComponentRequested;
        public event ComponentEventHandler OnCreateNewComponentRequested;
        public ComponentBase Component { get; set; }
        public int GridX { get; private set; }
        public int GridY { get; private set; }
        /// <summary>
        /// Register the Tile when it gets created
        /// </summary>
        public void SetPositionInGrid(int X, int Y)
        {
            GridX = X;
            GridY = Y;
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
            var newComponent = component.Duplicate();
            var oldRotation = component.Rotation90;
            component.Rotation90 = DiscreteRotation.R0;
            previewGrid.Columns = component.WidthInTiles;
            for (int y = 0; y < component.HeightInTiles; y++)
            {
                for (int x = 0; x < component.WidthInTiles; x++)
                {
                    var subTile = component.GetSubTileAt(x, y);
                    var previewtile = subTile.Duplicate();
                    previewtile._Ready();
                    //previewtile.Rotation90= subTile.Rotation90;// all this rotation mess has to be done because it seems that previewTile cannot be rotated within the grid directly (??)
                    previewGrid.AddChild(previewtile);
                }
            }
            previewGrid.RotationDegrees = (float)oldRotation *90f;
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
            return this.Component;
        }
    }
}
