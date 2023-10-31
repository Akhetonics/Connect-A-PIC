namespace CAP_Core.ExternalPorts
{
    public class InputLaser
    {
        public InputLaser(LightColor color, int waveLengthNM)
        {
            Color = color;
            WaveLengthNM = waveLengthNM;
        }
        public LightColor Color { get; }
        public int WaveLengthNM { get; }
    }
}