using CAP_Core.Helpers;
using CAP_Core.Tiles;

namespace CAP_Core.Components
{
    public class Part : ICloneable
    {
        public readonly List<Pin> Pins;
        public DiscreteRotation Rotation90 { get; set; }
        public Part()
        {
            Rotation90 = DiscreteRotation.R0;
            Pins = new List<Pin>
            {
                new Pin("west", 0, MatterType.None, RectSide.Left),
                new Pin("north",1, MatterType.None, RectSide.Up),
                new Pin("east", 2, MatterType.None, RectSide.Right),
                new Pin("south", 3, MatterType.None, RectSide.Down),
            };
        }
        public Part(List<Pin> pins)
        {
            Pins = pins;
        }


        public void InitializePin(RectSide side, string name, MatterType matterType)
        {
            var pin = GetPinAt(side);
            pin.MatterType = matterType;
            if (name != null)
            {
                pin.Name = name;
            }
        }

        /// <summary>
        /// takes rotation into account so that "right" is the absolute "right" Pin on the screen
        /// </summary>
        /// <param name="side"></param>
        /// <param name="absoluteOrientation"></param>
        /// <returns></returns>
        public Pin? GetPinAt(RectSide side, bool absoluteOrientation = true)
        {
            if (absoluteOrientation)
                side = side.RotateSideClockwise(Rotation90);

            return Pins.Find(p => p.Side == side);
        }
        public override string ToString()
        {
            // Filter pins that have MatterType of None and then join them as string
            return string.Join(", ", Pins.Where(pin => pin.MatterType != MatterType.None));
        }

        public object Clone()
        {
            var clonedPins = Pins.Select(pin => (Pin)pin.Clone()).ToList();
            var clonedPart = new Part(clonedPins);
            return clonedPart;
        }
    }
}
