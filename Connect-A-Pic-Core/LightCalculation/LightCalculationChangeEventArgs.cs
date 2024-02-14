using CAP_Core.ExternalPorts;
using System.Numerics;

namespace CAP_Core.LightCalculation
{
    public class LightCalculationChangeEventArgs
    {
        public LightCalculationChangeEventArgs(Dictionary<Guid, Complex> lightFieldVector, LaserType laser)
        {
            LightFieldVector = lightFieldVector;
            LaserInUse = laser;
        }
        public Dictionary<Guid, Complex> LightFieldVector { get; set; }
        public LaserType LaserInUse { get; set; }
    }
}
