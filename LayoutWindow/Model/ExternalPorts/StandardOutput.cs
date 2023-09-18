namespace ConnectAPIC.LayoutWindow.Model.ExternalPorts
{
    public class StandardOutput : ExternalPort
    {
        public StandardOutput(string pinName, int tilePositionY) : base(pinName, LightColor.Red, 0, tilePositionY)
        {
        }
    }
}