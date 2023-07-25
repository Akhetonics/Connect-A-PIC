using ConnectAPIC.Scenes.Component;
using Godot;

namespace ConnectAPIC.Scenes.Component
{
    public partial class Pin : TextureRect
    {
        [Export] public new string Name { get; set; } // the nazca name like b0, a0, a1
        [Export] private MatterType _matterType;
        public MatterType MatterType { get => _matterType; set => SetMatterType(value); }
        public override void _Ready()
        {
            base._Ready();
            SetMatterType(this.MatterType);
        }
        public void SetMatterType(MatterType newMatterType)
        {
            _matterType = newMatterType;
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
                    Texture = null;
                    this.Visible = false;
                    break;
                default:
                    Texture = null;
                    this.Visible = false;
                    break;
            }
        }
        public void Reset()
        {
            MatterType = MatterType.None;
            Name = "";
        }
        public override string ToString()
        {
            return Name;
        }
    }
}