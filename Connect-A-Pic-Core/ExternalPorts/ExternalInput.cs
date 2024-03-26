
using System.Numerics;

namespace CAP_Core.ExternalPorts
{
    public class ExternalInput : ExternalPort
    {
        private LaserType _laserType;
        public LaserType LaserType
        {
            get => _laserType;
            set
            {
                _laserType = value;
                OnPropertyChanged();
            }
        }
        public Complex _inFlowPower;
        public Complex InFlowPower
        {
            get => _inFlowPower;
            set
            {
                _inFlowPower = value;
                OnPropertyChanged();
            }
        }
		public Complex InFlowField => Complex.FromPolarCoordinates(Math.Sqrt(InFlowPower.Magnitude), InFlowPower.Phase);
		public ExternalInput(string pinName, LaserType inputLaserType, int tilePositionY, Complex inflowPower) : base(pinName, tilePositionY)
		{
            LaserType = inputLaserType;
            InFlowPower = inflowPower;
		}
	}
}
