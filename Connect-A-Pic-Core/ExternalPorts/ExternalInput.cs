
using System.Numerics;

namespace CAP_Core.ExternalPorts
{
    public class ExternalInput : ExternalPort
    {
        public LaserType LaserType { get; set; } //TODO: discuss if this is ok
        public Complex InFlowPower { get; set; }
		public Complex InFlowField => Complex.FromPolarCoordinates(Math.Sqrt(InFlowPower.Magnitude), InFlowPower.Phase);
		public ExternalInput(string pinName, LaserType inputLaserType, int tilePositionY, Complex inflowPower) : base(pinName, tilePositionY)
		{
            LaserType = inputLaserType;
            InFlowPower = inflowPower;
		}
	}
}
