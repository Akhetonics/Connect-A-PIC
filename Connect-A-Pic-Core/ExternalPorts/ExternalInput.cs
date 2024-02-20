
using System.Numerics;

namespace CAP_Core.ExternalPorts
{
	public class ExternalInput : ExternalPort
	{
		public LaserType LaserType { get; }
		public Complex LightInflow { get; set; }
		public ExternalInput(string pinName, LaserType inputLaserType, int tilePositionY, Complex lightInflow) : base(pinName, tilePositionY)
		{
			LaserType = inputLaserType;
			LightInflow = lightInflow;
		}
	}
}
