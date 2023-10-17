using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAP_Core.Component.ComponentHelpers
{
    public class PhaseShiftCalculator
    {
        public const int TileWidthInNM = 250000;
        public const int laserWaveLengthNM = 1550;
        public const double refractionIndexSiliconNitride = 2.2;
        public static double Calc(int widthInTiles)
        {
            var phaseShift = (2 * Math.PI / laserWaveLengthNM) * (refractionIndexSiliconNitride - 1) * (TileWidthInNM * widthInTiles);
            return phaseShift;
        }
    }
}
