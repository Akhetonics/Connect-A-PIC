using Godot;
using ConnectAPIC.Scenes.Component;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace ConnectAPIC.Scenes.Tiles
{
    public class Tile
    {
        public ComponentBase Component;
        public Image image;
        private Dictionary<RectangleSide, Pin> Pins;
        private Component.DiscreteRotation rotation;
        
        public Tile()
        {
            Pins = new Dictionary<RectangleSide, Pin>();
        }
        public  Tile(Pin right, Pin up, Pin left, Pin down) : this ()
        {
            Pins.Add(RectangleSide.Right, right);
            Pins.Add(RectangleSide.Up, up);
            Pins.Add(RectangleSide.Left, left);
            Pins.Add(RectangleSide.Down, down);
        }
        public void Rotate(Component.DiscreteRotation rotation)
        {
            this.rotation = rotation;
            if (rotation == DiscreteRotation.R90) image.Rotate90(ClockDirection.Counterclockwise);
            if (rotation == DiscreteRotation.R180) image.Rotate180();
            if (rotation == DiscreteRotation.R270) image.Rotate90(ClockDirection.Clockwise);
        }
        public Pin GetPinAt(RectangleSide side ) // takes rotation into account
        {
            return Pins.GetValueOrDefault(RectangleSideRotateable.Rotate(side, rotation));
        }
    }
}
