using CAP_Core.Component.ComponentHelpers;
using CAP_Core.Tiles;
using CAP_Core.LightFlow;
using System.Numerics;

namespace CAP_Core.Component
{
    public class StraightWaveGuide : ComponentBase
    {

        public override string NazcaFunctionName { get; set; } = "placeCell_StraightWG";
        public override string NazcaFunctionParameters { get; } = "";

        public StraightWaveGuide()
        {
            Parts = new Part[1, 1];
            Parts[0, 0] = CreatePart(RectSide.Left, RectSide.Right);

            var leftIn = PinIdLeftIn();
            var leftOut = PinIdLeftOut();
            var rightIn = PinIdRightIn();
            var rightOut = PinIdRightOut();
            // setting up the SMatrix
            var allPins = new List<Guid> {
                leftIn,
                leftOut,
                rightIn,
                rightOut
            };
            Connections = new SMatrix(allPins);
            double phaseShift = PhaseShiftCalculator.Calc(WidthInTiles);

            var connectionweights = new Dictionary<(Guid, Guid), Complex>
            {
                { (leftIn, rightOut ), Complex.FromPolarCoordinates(1, 0) },
                { (rightIn, leftOut ), Complex.FromPolarCoordinates(1, 0) },
            };

            Connections.SetValues(connectionweights);
        }

        
    }
}
