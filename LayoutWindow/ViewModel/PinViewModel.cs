using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles;

namespace ConnectAPIC.LayoutWindow.ViewModel
{
    public class PinViewModel
    {
        private readonly Pin pin;
        private readonly PinView pinView;

        public PinViewModel(Pin pin, PinView pinView)
        {
            this.pin = pin;
            this.pinView = pinView;
        }
        public void SetPinRelativePosition(RectangleSide side)
        {
            if (side == RectangleSide.Right)
            {
                Position = new Vector2(TileView.TilePixelSize - PinPixelSize, TileView.TilePixelSize / 2 - PinPixelSize / 2);
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
            _matterType = newMatterType;
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
