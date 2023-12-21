using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAP_Core.Components.ComponentHelpers
{
    public class PhaseShiftCalculator
    {
        public const double TileWidthInNM = 250000.0;
        public const double refractionIndexSiliconNitride = 2.0;
        public static double GetDegrees(double waveGuideLength, double laserWaveLength)
        {
            var phaseShift = 360 * refractionIndexSiliconNitride * waveGuideLength / laserWaveLength;
            phaseShift %= (360);
            return phaseShift;
        }
    }
}
