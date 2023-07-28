using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using System;
using Tiles;

namespace ConnectAPIC.Scenes.Component
{
    public class Pin 
    {
        public string Name { get; set; } // the nazca name like b0, a0, a1}
        public Guid ID { get; set; } 
        public RectangleSide Side { get; private set; }
        private MatterType _matterType;
        public MatterType MatterType { get => _matterType; set => SetMatterType(value); }
        public Pin(string Name, MatterType newMatterType, RectangleSide side) : this(Name, side)
        {
            this.MatterType = newMatterType;
        }
        public Pin( string Name, RectangleSide side)
        {
            this.Side = side;
            this.Name = Name;
            this.MatterType = MatterType.None;
            ID = Guid.NewGuid();
            SetPinRelativePosition(side);
        }

        

        public void SetMatterType(MatterType newMatterType)
        {
            _matterType = newMatterType;
        }
        public void Reset()
        {
            SetMatterType( MatterType.None);
            Name = "";
        }
        public override string ToString()
        {
            return Name;
        }
        public Pin Duplicate()
        {
            var duplicatedPin = new Pin(Name, Texture.Duplicate() as TextureRect,MatterType, Side);
            return duplicatedPin;
        }
    }
}