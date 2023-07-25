using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using System;
using System.Collections.Generic;
namespace Tiles
{

    public partial class Tile : TileDraggable
    {

        public delegate void TileEventHandler(Tile tile);
        public event TileEventHandler OnDeletionRequested;
        public event TileEventHandler OnRotationRequested;
        [Export] public NodePath PinLeftPath;
        [Export] public NodePath PinUpPath;
        [Export] public NodePath PinRightPath;
        [Export] public NodePath PinDownPath;
        protected Dictionary<RectangleSide, Pin> Pins;
        public DiscreteRotation _discreteRotation;
        public DiscreteRotation Rotation90
        {
            get => _discreteRotation;
            set
            {
                int rotationIntervals = _discreteRotation.CalculateCyclesTillTargetRotation(value);
                _discreteRotation = value;
                RotationDegrees = (int)Rotation90 * 90;
                for (int i = 0; i < rotationIntervals; i++)
                {
                    RotatePinsBy90();
                }
            }
        }


        public override void _Ready()
        {
            PivotOffset = Size / 2;
            Pins = new Dictionary<RectangleSide, Pin>
            {
                [RectangleSide.Left] = GetNode<Pin>(PinLeftPath),
                [RectangleSide.Up] = GetNode<Pin>(PinUpPath),
                [RectangleSide.Right] = GetNode<Pin>(PinRightPath),
                [RectangleSide.Down] = GetNode<Pin>(PinDownPath)
            };
        }


        public void ResetToDefault(Texture2D baseTexture)
        {
            Texture = baseTexture;
            PivotOffset = Size / 2;
            Visible = true;
            _discreteRotation = DiscreteRotation.R0;
            RotationDegrees = (int)Rotation90 * 90;
            Component = null;
            foreach(Pin pin in Pins.Values)
            {
                pin.Reset();
            }
        }

        public void InitializePin(RectangleSide side, string name, MatterType matterType)
        {
            Pins[side].MatterType = matterType;
            Pins[side].Name = name;
        }
        public void InitializePin(RectangleSide side, Pin pin)
        {
            Pins[side].MatterType = pin.MatterType;
            Pins[side].Name = pin.Name;
        }
        private void RotatePinsBy90()
        {
            // switch all pins around, so that they all go one to the left as with the rotation
            if (Pins != null && Pins.Count == 4)
            {
                (Pins[RectangleSide.Right], Pins[RectangleSide.Down]) = (Pins[RectangleSide.Down], Pins[RectangleSide.Right]);
                (Pins[RectangleSide.Right], Pins[RectangleSide.Left]) = (Pins[RectangleSide.Left], Pins[RectangleSide.Right]);
                (Pins[RectangleSide.Right], Pins[RectangleSide.Up]) = (Pins[RectangleSide.Up], Pins[RectangleSide.Right]);
            }
        }

        public Pin GetPinAt(RectangleSide side) // takes rotation into account
        {
            return Pins.GetValueOrDefault(side);
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
                    OnDeletionRequested?.Invoke(this);
                }
                if (mouseEvent.ButtonIndex == MouseButton.Right && mouseEvent.Pressed)
                {
                    OnRotationRequested?.Invoke(this);
                }
            }
        }

        public Tile Duplicate()
        {
            var copy = base.Duplicate() as Tile;
            copy.Rotation90 = Rotation90;
            copy.RotationDegrees = RotationDegrees;
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