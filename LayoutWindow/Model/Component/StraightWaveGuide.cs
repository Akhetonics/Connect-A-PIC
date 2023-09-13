using ConnectAPIC.Scenes.Tiles;
using Godot;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Tiles;
using TransferFunction;

namespace ConnectAPIC.Scenes.Component
{
    public partial class StraightWaveGuide : ComponentBase
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
            var connectionweights = new Dictionary<(Guid, Guid), Complex>
            {
                { (leftIn, rightOut ), new Complex(1, 0) },
                { (rightIn, leftOut ), new Complex(1, 0) },
            };

            Connections.SetValues(connectionweights);
        }
    }
}
