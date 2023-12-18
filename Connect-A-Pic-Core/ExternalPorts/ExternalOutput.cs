namespace CAP_Core.ExternalPorts
{
    public class ExternalOutput : ExternalPort
    {
        public ExternalOutput(string pinName, int tilePositionY) : base(pinName, LightColor.Red, 0, tilePositionY)
        {
        }
    }
}