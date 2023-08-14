using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Tiles;

namespace ConnectAPIC.Scenes.Component
{
    public class Pin : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string Name { get; set; } // the nazca name like b0, a0, a1}
        public Guid ID { get; set; }
        private RectangleSide _side;
        public RectangleSide Side {
            get => _side;
            private set {
                _side = value;
                NotifyPropertyChanged();
            } 
        }
        private MatterType _matterType;
        public MatterType MatterType { 
            get => _matterType; 
            set {
                SetMatterType(value); 
                NotifyPropertyChanged(); 
            } 
        }
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
            var duplicatedPin = new Pin(Name,MatterType, Side);
            return duplicatedPin;
        }
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}