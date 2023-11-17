using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    

    public class BinaryTests
    {
        //public double CalculateOpticalPower(double fluenceInMicroJoulesPerCm2, double timeToRecoverInPicoseconds, double areaInCm2)
        //{
        //    double fluenceInJoulesPerCm2 = fluenceInMicroJoulesPerCm2 * 1e-6;
        //    double timeToRecoverInSeconds = timeToRecoverInPicoseconds * 1e-12;

        //    double powerInWatts = (fluenceInJoulesPerCm2 * areaInCm2) / timeToRecoverInSeconds;
        //    return powerInWatts;
        //}
        //[Fact]
        //public void CalculateOpticalPowerTests()
        //{
        //    var output = CalculateOpticalPower(300, 2, 85 / 1e8);
        //}

        //static readonly ushort[] wavePoints = new ushort[] { 0x1000, 0x2000, 0x3000, 0x4000, 0x5000, 0x6000, 0x7000, 0xff7f };
        //[Fact]
        //public void CreateWaveFile()
        //{
        //    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        //    using (var binWriter = new BinaryWriter(File.Open(desktopPath + "\\wave3.bin", FileMode.Create)))
        //    {
        //        foreach (var point in wavePoints)
        //        {
        //            binWriter.Write(BitConverter.GetBytes(point));
        //        }
        //    }
        //}
    }
}
