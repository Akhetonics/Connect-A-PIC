using CAP_Core.Components.ComponentHelpers;
using System.Drawing;



namespace CAP_Core.ExternalPorts
{
    
    public class LaserType
    {
        public static LaserType Red = new LaserType(LightColor.Red, StandardWaveLengths.RedNM);
        public static LaserType Green = new LaserType(LightColor.Green, StandardWaveLengths.GreenNM);
        public static LaserType Blue = new LaserType(LightColor.Blue, StandardWaveLengths.BlueNM);

        public LaserType(LightColor lightColor, double waveLengthInNm)
        {
            Color = lightColor;
            WaveLengthInNm = waveLengthInNm;
        }
        public LightColor Color { get; }
        public double WaveLengthInNm { get; }
        public override string ToString()
        {
            return $"color: {Color.ToReadableString()}, WaveLength: {WaveLengthInNm}nm";
        }
    }

}