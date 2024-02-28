using CAP_Core.CodeExporter;
using CAP_Core.ExternalPorts;
using System.Numerics;
using CAP_Core.Components.ComponentHelpers;
using CAP_Core.Grid;

namespace UnitTests
{
    public class PhaseShiftTests
    {

        [Fact]
        public static void ComplexFactorTest()
        {
            var power = 1.0;
            var waveLengthInNm = 196349;
            var phaseShift = PhaseShiftCalculator.CalculateWave(waveLengthInNm, StandardWaveLengths.RedNM).Imaginary; // from a to c  "e^i*PI" = -1


            // I use this test to calculate the different imaginary values of the phase shift for given wire lengths to get realistic dummy values
            var imaginaryRedAToC = PhaseShiftCalculator.CalculateWave(waveLengthInNm, StandardWaveLengths.RedNM) * Math.Sqrt(power); // from a to c  "e^i*PI" = -1
            var imaginaryRedBToC = PhaseShiftCalculator.CalculateWave(waveLengthInNm, StandardWaveLengths.RedNM) * Math.Sqrt(power) * -1; // from e^i*PI = -1

            var imaginaryRedAToCGreen = PhaseShiftCalculator.CalculateWave(waveLengthInNm, StandardWaveLengths.GreenNM) * Math.Sqrt(power); // from a to c  "e^i*PI" = -1
            var imaginaryRedBToCGreen = PhaseShiftCalculator.CalculateWave(waveLengthInNm, StandardWaveLengths.GreenNM) * Math.Sqrt(power) * -1; // from e^i*PI = -1

            var imaginaryRedAToCBlue = PhaseShiftCalculator.CalculateWave(waveLengthInNm, StandardWaveLengths.BlueNM) * Math.Sqrt(power); // from a to c  "e^i*PI" = -1
            var imaginaryRedBToCBlue = PhaseShiftCalculator.CalculateWave(waveLengthInNm, StandardWaveLengths.BlueNM) * Math.Sqrt(power) * -1; // from e^i*PI = -1
            var powerAtC2 = imaginaryRedAToC.Magnitude * imaginaryRedBToC.Magnitude;
            Console.WriteLine();
        }
        
    }
}
