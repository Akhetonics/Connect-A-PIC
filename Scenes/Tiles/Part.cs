using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace Tiles
{

    public partial class Part : TileBase
    {

        [Export] public NodePath PinLeftPath;
        [Export] public NodePath PinUpPath;
        [Export] public NodePath PinRightPath;
        [Export] public NodePath PinDownPath;
        protected Dictionary<RectangleSide, Pin> Pins;
       
        public override void _Ready()
        {
            PivotOffset = Size / 2;
            Pins = new Dictionary<RectangleSide, Pin>
            {
                [RectangleSide.Left] = new Pin("", GetNode<TextureRect>(PinLeftPath)),
                [RectangleSide.Up] = new Pin("", GetNode<TextureRect>(PinUpPath)),
                [RectangleSide.Right] = new Pin("", GetNode<TextureRect>(PinRightPath)),
                [RectangleSide.Down] = new Pin("", GetNode<TextureRect>(PinDownPath)),
            };
        }



        public void InitializePin(RectangleSide side, string name, MatterType matterType)
        {
            side = side.RotateRectangleSide(Rotation90);
            Pins[side].MatterType = matterType;
            Pins[side].Name = name;
        }
        
        public void InitializePin(RectangleSide side, Pin pin)
        {
            this.InitializePin(side, pin.Name, pin.MatterType);
        }

        public Pin GetPinAt(RectangleSide side) // takes rotation into account
        {
            side = side.RotateRectangleSide(Rotation90);
            return Pins[side];
        }

        public Part Duplicate()
        {
            var copy = base.Duplicate() as Part;
            copy.Pins = new Dictionary<RectangleSide, Pin>();
            if (Pins != null)
            {
                foreach (var kvp in Pins)
                {
                    if (kvp.Value != null)
                    {
                        copy.Pins.Add(kvp.Key, kvp.Value.Duplicate() as Pin);
                    }

                }
            }
            return copy;
        }

    }
}