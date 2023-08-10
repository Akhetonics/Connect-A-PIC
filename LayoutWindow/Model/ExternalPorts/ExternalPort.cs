namespace ConnectAPIC.LayoutWindow.Model.ExternalPorts
{
    public abstract class ExternalPort
    {
        public ExternalPort(string pinName, LightCycleColor color, int waveLength, int tilePositionY)
        {
            PinName = pinName;
            Color = color;
            WaveLength = waveLength;
            TilePositionY = tilePositionY;
        }

        public string PinName { get; }
        public LightCycleColor Color { get; }
        public int WaveLength { get; }
        public int TilePositionY { get; }
    }
}