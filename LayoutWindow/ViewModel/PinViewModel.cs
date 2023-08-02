using ConnectAPIC.LayoutWindow.View;
using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles;

namespace ConnectAPIC.LayoutWindow.ViewModel
{
    public class PinViewModel
    {
        private readonly Pin pin;
        private readonly PinView pinView;

        public PinViewModel(Pin pin, PinView pinView)
        {
            this.pin = pin;
            this.pinView = pinView;
            this.pinView.SetPinRelativePosition(pin.Side);
            this.pinView.SetMatterType(pin.MatterType);
            this.pin.PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) =>
            {
                if (e.PropertyName == nameof(pin.MatterType))
                {
                    this.pinView.SetMatterType(pin.MatterType);
                }
                if(e.PropertyName == nameof(pin.Side))
                {
                    pinView.SetPinRelativePosition(pin.Side);
                }
            };
        }
    }
}
