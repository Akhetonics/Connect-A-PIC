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

            // setting up the SMatrix
            var allPins = new List<Guid> {
                PinIdLeft(0,0),
                PinIdRight(0,0),
            };
            Connections = new SMatrix(allPins);
            var connectionweights = new Dictionary<(Guid, Guid), Complex>
            {
                { (PinIdLeft(0,0), PinIdRight(0,0) ), new Complex(1, 0) },
                { (PinIdRight(0,0), PinIdLeft(0,0) ), new Complex(1, 0) },
            };

            Connections.setValues(connectionweights);
        }
    }
}
