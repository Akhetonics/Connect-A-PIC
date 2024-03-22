namespace CAP_Core.ExternalPorts
{
    public abstract class ExternalPort
    {
        public ExternalPort(string pinName, int tilePositionY)
        {
            PinName = pinName;
            TilePositionY = tilePositionY;
        }

        public string PinName { get; }
        public int TilePositionY { get; }
    }
}
