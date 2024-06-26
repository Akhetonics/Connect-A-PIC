using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CAP_Core.ExternalPorts
{
    public abstract class ExternalPort: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ExternalPort(string pinName, int tilePositionY, bool isLeftPort = true)
        {
            PinName = pinName;
            TilePositionY = tilePositionY;
            IsLeftPort = isLeftPort;
        }

        public string PinName { get; }
        public int TilePositionY { get; }
        public bool IsLeftPort { get; }

       
    }
}
