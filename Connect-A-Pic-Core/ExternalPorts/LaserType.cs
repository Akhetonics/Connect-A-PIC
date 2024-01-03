using CAP_Core.Components.ComponentHelpers;
using System.Drawing;



namespace CAP_Core.ExternalPorts
{
    
    public class LaserType
    {
        public static LaserType Red = new (LightColor.Red);
        public static LaserType Green = new (LightColor.Green);
        public static LaserType Blue = new (LightColor.Blue);

        public LaserType(LightColor lightColor)
        {
            Color = lightColor;
            switch (lightColor)
            {
                case LightColor.Red:
                    WaveLengthInNm = StandardWaveLengths.RedNM;
                    break;
                case LightColor.Green:
                    WaveLengthInNm = StandardWaveLengths.GreenNM;
                    break;
                case LightColor.Blue:
                    WaveLengthInNm = StandardWaveLengths.BlueNM;
                    break;
            }
        }
        public LightColor Color { get; }
        public double WaveLengthInNm { get; }
        public override string ToString()
        {
            return $"color: {Color.ToReadableString()}, WaveLength: {WaveLengthInNm}nm";
        }
    }

}