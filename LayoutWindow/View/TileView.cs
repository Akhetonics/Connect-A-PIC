using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using System.Linq;

namespace ConnectAPIC.LayoutWindow.View
{

    public partial class TileView : TextureRect
    {
        private PinView _PinRight;
        private PinView _PinDown;
        private PinView _PinLeft;
        private PinView _PinUp;
        [Export] public PinView PinRight { get => _PinRight; set { _PinRight = value; _PinRight.SetPinRelativePosition(RectangleSide.Right); } }
        [Export] public PinView PinDown { get => _PinDown; set { _PinDown = value; _PinDown.SetPinRelativePosition(RectangleSide.Down); } }
        [Export] public PinView PinLeft { get => _PinLeft; set { _PinLeft = value; _PinLeft.SetPinRelativePosition(RectangleSide.Left); } }
        [Export] public PinView PinUp { get => _PinUp; set { _PinUp = value; _PinUp.SetPinRelativePosition(RectangleSide.Up); } }

        public TileViewModel TileViewModel { get; set; }
        public static int TilePixelSize { get; } = 64;

        public override void _Ready()
        {
            PivotOffset = Size / 2;
        }
        
        public void Initialize(TileViewModel viewModel)
        {
            this.TileViewModel = viewModel;
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
                    // call the Remove Component command on the ComponentViewModel
                    TileViewModel.DeleteComponentCommand.Execute(null);
                }
                if (mouseEvent.ButtonIndex == MouseButton.Right && mouseEvent.Pressed)
                {
                    TileViewModel.RotateComponentCommand.Execute(null);
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