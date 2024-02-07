using CAP_Core.ExternalPorts;
using System.Numerics;

namespace CAP_Core.LightCalculation
{
    public class LightCalculationChangeEventArgs
    {
        public LightCalculationChangeEventArgs(Dictionary<Guid, Complex> lightVector, LaserType laser)
        {
            LightVector = lightVector;
            LaserInUse = laser;
        }
        public Dictionary<Guid, Complex> LightVector { get; set; }
        public LaserType LaserInUse { get; set; }
    }
}
