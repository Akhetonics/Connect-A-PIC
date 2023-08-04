using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace Model
{

    public class Part
    {

        protected List<Pin> Pins;
        public DiscreteRotation _discreteRotation;
        public DiscreteRotation Rotation90
        {
            get => _discreteRotation;
            set
            {
                _discreteRotation = value;
            }
        }
        public Part()
        {
            Pins = new List<Pin>
            {
                new Pin("", MatterType.None, RectangleSide.Left),
                new Pin("", MatterType.None, RectangleSide.Up),
                new Pin("", MatterType.None, RectangleSide.Right),
                new Pin("", MatterType.None, RectangleSide.Down),
            };
        }
        public void InitializePin(RectangleSide side, string name, MatterType matterType)
        {
            side = side.RotateRectangleSide(Rotation90);
            var pin = Pins.Where(p => p.Side == side).FirstOrDefault();
            pin.MatterType = matterType;
            pin.Name = name;
        }
        
        public void InitializePin(RectangleSide side, Pin pin)
        {
            this.InitializePin(side, pin.Name, pin.MatterType);
        }

        public Pin GetPinAt(RectangleSide side) // takes rotation into account
        {
            side = side.RotateRectangleSide(Rotation90);
            return Pins.Where(p => p.Side == side).FirstOrDefault();
        }
        

        public new Part Duplicate()
        {
            var copy = new Part();
            copy.Pins = new List<Pin>();
            if (this.Pins != null)
            {
                foreach (var pin in Pins)
                {
                    copy.Pins.Add(pin.Duplicate());   
                }
            }
            return copy;
        }

    }
}