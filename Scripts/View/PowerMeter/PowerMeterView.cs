using CAP_Core.ExternalPorts;
using ConnectAPIC.LayoutWindow.View;
using ConnectAPic.LayoutWindow;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Godot.Control;
using CAP_Core.LightCalculation;
using ConnectAPIC.LayoutWindow.ViewModel;
using ConnectAPIC.Scripts.ViewModel;

namespace ConnectAPIC.Scripts.View.PowerMeter
{
    public partial class PowerMeterView : TextureRect
    {
        [Export] public RichTextLabel InfoLabel { get; set; }
        public PowerMeterViewModel ViewModel { get; set; }
        public PowerMeterView() // should be empty for duplicate to work
        {
        }
        public void Initialize(PowerMeterViewModel viewModel)
        {
            ViewModel = viewModel;
            viewModel.PowerChanged += (object sender, string e) => DisplayPower(e);
        }

        public void DisplayPower(string labelText)
        {
            InfoLabel.Text = labelText;
        }
    }
}
