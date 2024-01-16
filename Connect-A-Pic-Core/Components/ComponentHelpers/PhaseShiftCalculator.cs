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

        public static double CalculateInRad(double waveGuideLength, double laserWaveLength)
        {
            var phaseShift = 2 * Math.PI * refractionIndexSiliconNitride * waveGuideLength / laserWaveLength;
            phaseShift %= (2 * Math.PI);
            return phaseShift;
        }
    }
}
