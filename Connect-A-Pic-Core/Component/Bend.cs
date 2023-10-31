using CAP_Core.Component.ComponentHelpers;
using CAP_Core.Tiles;
using CAP_Core.LightFlow;
using System.Numerics;

namespace CAP_Core.Component
{
    public class Bend: ComponentBase
    {

        public override string NazcaFunctionName { get; set; } = "placeCell_BendWG";
        public override string NazcaFunctionParameters { get; } = "";

        public Bend()
        {
            Parts = new Part[1, 1];
            Parts[0, 0] = CreatePart(RectSide.Down, RectSide.Left);

            var downIn = PinIdDownIn();
            var downOut = PinIdDownOut();
            var leftIn = PinIdLeftIn();
            var leftOut = PinIdLeftOut();
            // setting up the SMatrix
            var allPins = new List<Guid> {
                leftIn,
                leftOut,
                downIn,
                downOut
            };
            Connections = new SMatrix(allPins);
            var connectionweights = new Dictionary<(Guid, Guid), Complex>
            {
                { (leftIn, downOut ), new Complex(1, 0) },
                { (downIn, leftOut ), new Complex(1, 0) },
            };

            Connections.SetValues(connectionweights);
        }
    }
}
