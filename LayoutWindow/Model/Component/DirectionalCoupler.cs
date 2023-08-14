using ConnectAPIC.Scenes.Tiles;
using Godot;
using Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Tiles;
using TransferFunction;

namespace ConnectAPIC.Scenes.Component
{
    public class DirectionalCoupler : ComponentBase
    {
        public int deltaLength { get; set; } = 50;
        public override string NazcaFunctionName { get; set; } = "placeCell_DirectionalCoupler";
        public override string NazcaFunctionParameters { get => $"deltaLength = {deltaLength}"; }

        public DirectionalCoupler()
        {
            Parts = new Part[2, 1];
            Parts[0, 0] = CreatePart(RectSide.Left, RectSide.Right);
            Parts[1, 0] = CreatePart(RectSide.Left, RectSide.Right);

            // setting up the SMatrix
            var allPins = new List<Guid> {
                PinIdRight(0,0),
                PinIdLeft(0,0),
                PinIdRight(1,0),
                PinIdLeft(1,0),
            };
            Connections = new SMatrix(allPins);
            var connectionweights = new Dictionary<(Guid, Guid), Complex>()
            {
                { (PinIdLeft(0,0), PinIdRight(0,0)), new Complex(0.5, 0) },
                { (PinIdLeft(0,0), PinIdRight(0,1)), new Complex(0.5, 0) },
                { (PinIdLeft(0,1), PinIdRight(0,0)), new Complex(0.5, 0) },
                { (PinIdLeft(0,1), PinIdRight(0,1)), new Complex(0.5, 0) },

                { (PinIdRight(0,0), PinIdLeft(0,0)), new Complex(0.5, 0) },
                { (PinIdRight(0,0), PinIdLeft(0,1)), new Complex(0.5, 0) },
                { (PinIdRight(0,1), PinIdLeft(0,0)), new Complex(0.5, 0) },
                { (PinIdRight(0,1), PinIdLeft(0,1)), new Complex(0.5, 0) },

            };
            Connections.setValues(connectionweights);
        }
    }
}
