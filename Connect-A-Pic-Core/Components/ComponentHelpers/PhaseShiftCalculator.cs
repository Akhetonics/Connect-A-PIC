using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CAP_Core.Components.ComponentHelpers
{
    public class PhaseShiftCalculator
    {
        public const double TileWidthInNM = 250000.0;
        public const double refractionIndexSiliconNitride = 2.0;

        public static Complex CalculateWave(double waveGuideLengthNM, double laserWaveLengthNM)
        {
            var phaseShift = 2 * Math.PI * refractionIndexSiliconNitride * waveGuideLengthNM / laserWaveLengthNM;
            phaseShift %= (2 * Math.PI);

            // Create a complex number with magnitude 1 and the calculated phase shift
            Complex wave = Complex.FromPolarCoordinates(1, phaseShift);
            return wave;
        }

        public static double CalculateImaginaryPart(double waveGuideLengthNM, double laserWaveLengthNM)
        {
            var imaginaryPart = refractionIndexSiliconNitride * waveGuideLengthNM / laserWaveLengthNM;
            return CalculateWave(waveGuideLengthNM,laserWaveLengthNM).Imaginary;
        }
    }
}
