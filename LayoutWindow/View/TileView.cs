using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using System.Linq;

namespace ConnectAPIC.LayoutWindow.View
{

    public partial class TileView : TextureRect
    {
        public delegate void ComponentEventHandler(TileView tile, ComponentBaseView component);
        public event ComponentEventHandler OnExistingTileDropped;
        public event ComponentEventHandler OnNewTileDropped;
        public delegate void TileEventHandler(TileView tile);
        public event TileEventHandler OnMiddleClicked;
        public event TileEventHandler OnRightClicked;
        [Export] public PinView PinRight { get; set; }
        [Export] public PinView PinDown { get; set; }
        [Export] public PinView PinLeft { get; set; }
        [Export] public PinView PinUp { get; set; }
        public ComponentBaseView ComponentView { get; set; }
        public int GridX { get; private set; }
        public int GridY { get; private set; }
        public static int TilePixelSize { get; } = 64;

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
            PinRight.SetPinRelativePosition(RectangleSide.Right);
            PinDown.SetPinRelativePosition(RectangleSide.Down);
            PinLeft.SetPinRelativePosition(RectangleSide.Left);
            PinUp.SetPinRelativePosition(RectangleSide.Up);
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
                    OnMiddleClicked?.Invoke(this);
                }
                if (mouseEvent.ButtonIndex == MouseButton.Right && mouseEvent.Pressed)
                {
                    OnRightClicked?.Invoke(this);
                }
            }
        }
        public override bool _CanDropData(Vector2 position, Variant data)
        {
            // extract all tiles from the component that is about to be dropped here at position and SetDragPreview them
            if (data.Obj is ComponentBaseView component)
            {
                ShowMultiTileDragPreview(position, component);
            }

            return true;
        }
        protected void ShowMultiTileDragPreview(Vector2 position, ComponentBaseView component )
        {
            var previewGrid = new GridContainer();
            previewGrid.PivotOffset = previewGrid.Size / 2f;
            var oldRotation = component.Rotation90;
            component.Rotation90 = 0;
            previewGrid.Columns = component.WidthInTiles;
            for (int y = 0; y < component.HeightInTiles; y++)
            {
                for (int x = 0; x < component.WidthInTiles; x++)
                {
                    var previewtile = this.Duplicate();
                    previewtile._Ready();
                    previewtile.Texture = component.GetTexture(x,y).Duplicate() as Texture2D;
                    previewtile.Visible = true;
                    previewGrid.AddChild(previewtile);
                }
            }
            previewGrid.RotationDegrees = (int)oldRotation*90;
            component.Rotation90 = oldRotation;
            this.SetDragPreview(previewGrid);
        }
        public override void _DropData(Vector2 atPosition, Variant data)
        {
            if (data.Obj is ComponentBaseView componentView)
            {
                if (!componentView.Visible)
                {
                    OnNewTileDropped?.Invoke(this, componentView);
                }
                else
                {
                    OnExistingTileDropped?.Invoke(this, componentView);
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
            RotationDegrees = 0;
            ComponentView = null;
            PinRight.SetMatterType(MatterType.None);
            PinDown.SetMatterType(MatterType.None);
            PinLeft.SetMatterType(MatterType.None);
            PinUp.SetMatterType(MatterType.None);
        }
        public TileView Duplicate()
        {
            var copy = base.Duplicate() as TileView;
            copy.RotationDegrees = RotationDegrees;
            var children = copy.GetChildren();
            copy.PinRight = children.First(f=>f.Name == "PinRight") as PinView;
            copy.PinDown= children.First(f => f.Name == "PinDown") as PinView;
            copy.PinLeft= children.First(f => f.Name == "PinLeft") as PinView;
            copy.PinUp = children.First(f => f.Name == "PinUp") as PinView;
            return copy;
        }


    }
}