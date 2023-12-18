
using System.Numerics;

namespace CAP_Core.ExternalPorts
{
	public class ExternalInput : ExternalPort
	{
		public Complex LightInflow { get; set; }
		public ExternalInput(string pinName, LightColor color, int waveLength, int tilePositionY, Complex lightInflow) : base(pinName, color, waveLength, tilePositionY)
		{
			LightInflow = lightInflow;
		}
	}
}
