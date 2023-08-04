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
        public DiscreteRotation Rotation90 { get; set; }
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
            var pin = Pins.Find(p => p.Side == side);
            pin.MatterType = matterType;
            pin.Name = name;
        }
        
        public void InitializePin(RectangleSide side, Pin pin)
        {
            this.InitializePin(side, pin.Name, pin.MatterType);
        }

        public Pin GetPinAt(RectangleSide side , bool CorrectRotation = true) // takes rotation into account
        {
            if (CorrectRotation)
                side = side.RotateRectangleSide(Rotation90);
            return Pins.Find(p => p.Side == side);
        }
        

        public Part Duplicate()
        {
            var copy = new Part
            {
                Pins = new List<Pin>()
            };
            foreach (var pin in Pins)
            {
                copy.Pins.Add(pin.Duplicate());   
            }
            return copy;
        }

    }
}