namespace CAP_Core.ExternalPorts
{
    public abstract class ExternalPort
    {
        public ExternalPort(string pinName, LightColor color, int waveLength, int tilePositionY)
        {
            PinName = pinName;
            Color = color;
            WaveLength = waveLength;
            TilePositionY = tilePositionY;
        }

        public string PinName { get; }
        public LightColor Color { get; }
        public int WaveLength { get; }
        public int TilePositionY { get; }
    }
}