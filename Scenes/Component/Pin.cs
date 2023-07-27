using ConnectAPIC.Scenes.Component;
using Godot;
using System;

namespace ConnectAPIC.Scenes.Component
{
    public class Pin 
    {
        public string Name { get; set; } // the nazca name like b0, a0, a1}
        public Guid ID { get; set; } 
        public TextureRect Image;
        private MatterType _matterType;
        public MatterType MatterType { get => _matterType; set => SetMatterTypeAndUpdateImage(value); }
        public Pin(string Name, TextureRect texture, MatterType newMatterType) : this(Name, texture)
        {
            this.MatterType = newMatterType;
        }
        public Pin( string Name, TextureRect texture)
        {
            this.Image = texture;
            this.Name = Name;
            this.MatterType = MatterType.None;
            ID = Guid.NewGuid();
            Image._Ready();
        }
        public void SetMatterTypeAndUpdateImage(MatterType newMatterType)
        {
            _matterType = newMatterType;
            if (Image != null)
            {
                Image.Visible = true;
                switch (newMatterType)
                {
                    case MatterType.Electricity:
                        Image.Texture = GD.Load<Texture2D>("res://Scenes/Tiles/PinElectric.png");
                        break;
                    case MatterType.Light:
                        Image.Texture = GD.Load<Texture2D>("res://Scenes/Tiles/PinLight.png");
                        break;
                    case MatterType.None:
                        Image.Visible = false;
                        break;
                    default:
                        Image.Visible = false;
                        break;
                }
            }
        }
        public void Reset()
        {
            SetMatterTypeAndUpdateImage( MatterType.None);
            Name = "";
        }
        public override string ToString()
        {
            return Name;
        }
        public Pin Duplicate()
        {
            var duplicatedPin = new Pin(Name, Image.Duplicate() as TextureRect,MatterType);
            return duplicatedPin;
        }
    }
}