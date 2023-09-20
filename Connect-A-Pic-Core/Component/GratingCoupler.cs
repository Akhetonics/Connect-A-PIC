using CAP_Core.Component.ComponentHelpers;
using CAP_Core.Tiles;
using CAP_Core.LightFlow;
using System.Numerics;

namespace CAP_Core.Component
{
    public partial class GratingCoupler : ComponentBase
    {

        public GratingCoupler()
        {
            Parts = new Part[1, 1];
            Parts[0, 0] = Parts[0, 0] = CreatePart(RectSide.Left);

            var leftIn = PinIdLeftIn();
            var leftOut = PinIdLeftOut();
            // setting up the SMatrix
            var allPins = new List<Guid> {
                leftIn,
                leftOut
            };
            Connections = new SMatrix(allPins);
            var connectionweights = new Dictionary<(Guid, Guid), Complex>
            {
            };

            Connections.SetValues(connectionweights);
        }

        public override string NazcaFunctionName { get; set; } = "placeCell_GratingCoupler";
        public override string NazcaFunctionParameters { get; }
    }
}
