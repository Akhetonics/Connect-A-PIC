using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAP_Core.Component.ComponentHelpers
{
    public class PhaseShiftCalculator
    {
        public const double TileWidthInNM = 250000.0;
        public const double laserWaveLengthNM = 1550.0;
        public const double refractionIndexSiliconNitride = 2.0;
        public static double Calc(int widthInTiles)
        {
            var waveGuideLength = TileWidthInNM * widthInTiles;

            var phaseShift = 2 * Math.PI * refractionIndexSiliconNitride * waveGuideLength / laserWaveLengthNM;
            phaseShift %= (2 * Math.PI);
            return phaseShift;
        }
    }
}
