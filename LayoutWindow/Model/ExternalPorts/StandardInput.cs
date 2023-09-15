
using System.Numerics;

namespace ConnectAPIC.LayoutWindow.Model.ExternalPorts
{
    public class StandardInput : ExternalPort
    {
        public Complex LightInflow { get; set; }
        public StandardInput(string pinName, LightCycleColor color, int waveLength, int tilePositionY, Complex lightInflow) : base(pinName, color, waveLength, tilePositionY)
        {
            LightInflow = lightInflow;
        }
    }
}