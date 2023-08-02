using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles;

namespace ConnectAPIC.LayoutWindow.View
{
    public partial class PinView : TextureRect
    {
        public static int PinPixelSize = 6;

        public override void _Ready()
        {
            base._Ready();
        }
        public void SetPinRelativePosition(RectangleSide side)
        {
            if (side == RectangleSide.Right)
            {
                this.Position = new Vector2(TileView.TilePixelSize - PinPixelSize, TileView.TilePixelSize / 2 - PinPixelSize / 2);
            }
            if (side == RectangleSide.Down)
            {
                Position = new Vector2(TileView.TilePixelSize / 2 - PinPixelSize / 2, TileView.TilePixelSize - PinPixelSize);
            }
            if (side == RectangleSide.Left)
            {
                Position = new Vector2(0, TileView.TilePixelSize / 2 - PinPixelSize / 2);
            }
            if (side == RectangleSide.Up)
            {
                Position = new Vector2(TileView.TilePixelSize / 2 - PinPixelSize / 2, 0);
            }
        }
        public void SetMatterType(MatterType newMatterType)
        {
            if (Texture != null)
            {
                Visible = true;
                switch (newMatterType)
                {
                    case MatterType.Electricity:
                        Texture = GD.Load<Texture2D>("res://Scenes/Tiles/PinElectric.png");
                        break;
                    case MatterType.Light:
                        Texture = GD.Load<Texture2D>("res://Scenes/Tiles/PinLight.png");
                        break;
                    case MatterType.None:
                        Visible = false;
                        break;
                    default:
                        Visible = false;
                        break;
                }
            }
        }
    }
}
