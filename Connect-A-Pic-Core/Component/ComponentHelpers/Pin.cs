using CAP_Core.Tiles;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace CAP_Core.Component.ComponentHelpers
{
    public class Pin : INotifyPropertyChanged, ICloneable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string Name { get; set; } // the nazca name like b0, a0, a1}
        public Guid IDInFlow { get; set; }
        public Guid IDOutFlow { get; set; }
        [JsonIgnore] public Complex LightInflow { get; set; } // the light flowing into this component at this pin
        [JsonIgnore] public Complex LightOutflow { get; set; } // the light exiting this component at this pin
        private RectSide _side;
        public RectSide Side
        {
            get => _side;
            private set
            {
                _side = value;
                NotifyPropertyChanged();
            }
        }
        private MatterType _matterType;
        public MatterType MatterType
        {
            get => _matterType;
            set
            {
                SetMatterType(value);
                NotifyPropertyChanged();
            }
        }
        public Pin(string Name, MatterType newMatterType, RectSide side) : this(Name, side)
        {
            MatterType = newMatterType;
        }
        protected Pin(string Name, RectSide side)
        {
            Side = side;
            this.Name = Name;
            MatterType = MatterType.None;
            IDInFlow = Guid.NewGuid();
            IDOutFlow = Guid.NewGuid();
        }

        public void SetMatterType(MatterType newMatterType)
        {
            _matterType = newMatterType;
        }
        public void Reset()
        {
            SetMatterType(MatterType.None);
            Name = "";
        }
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public override string ToString()
        {
            return $"{Name}: {MatterType}";
        }

        public object Clone()
        {
            var clonedPin = new Pin(Name, MatterType, Side);
            return clonedPin;
        }
    }
}