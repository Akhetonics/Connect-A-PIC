namespace CAP_Core.ExternalPorts
{
    public class ExternalOutput : ExternalPort
    {
        public ExternalOutput(string pinName, int tilePositionY, bool isLeftPort = true) : base(pinName, tilePositionY, isLeftPort)
        {
        }
    }
}
