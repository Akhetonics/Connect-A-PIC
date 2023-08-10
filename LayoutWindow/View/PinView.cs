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
        [Export] protected Texture2D ElectricityPinTexture { get; set; }
        [Export] protected Texture2D LightPinTexture { get; set; }
        public static readonly int PinPixelSize = 6;

        public PinView()
        {
            SetMatterType(MatterType.None);
        }
        public override void _Ready()
        {
            base._Ready();
            Visible = false;
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
                        Texture = ElectricityPinTexture;
                        break;
                    case MatterType.Light:
                        Texture = LightPinTexture;
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
        public PinView Duplicate()
        {
            var copy = base.Duplicate() as PinView;
            copy.ElectricityPinTexture = ElectricityPinTexture;
            copy.LightPinTexture = LightPinTexture;
            copy._Ready();
            return copy;
        }
    }
}
