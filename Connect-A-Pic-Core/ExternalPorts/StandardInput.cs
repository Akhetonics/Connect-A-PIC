
using System.Numerics;

namespace CAP_Core.ExternalPorts
{
    public class StandardInput : ExternalPort
    {
        public Complex LightInflow { get; set; }
        public StandardInput(string pinName, LightColor color, int waveLength, int tilePositionY, Complex lightInflow) : base(pinName, color, waveLength, tilePositionY)
        {
            LightInflow = lightInflow;
        }
    }
}