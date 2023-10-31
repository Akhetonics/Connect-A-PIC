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
        public static double laserWaveLengthRedNM { get; set; } = 1550.0;
        public static double laserWaveLengthGreenNM { get; set; } = 1310.0;
        public static double laserWaveLengthBlueNM { get; set; } = 980.0;
        public const double refractionIndexSiliconNitride = 2.0;
        public static double Calc(int widthInTiles)
        {
            var waveGuideLength = TileWidthInNM * widthInTiles;

            var phaseShift = 2 * Math.PI * refractionIndexSiliconNitride * waveGuideLength / laserWaveLengthRedNM;
            phaseShift %= (2 * Math.PI);
            return phaseShift;
        }
    }
}
