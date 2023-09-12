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
            Parts = new Part[2, 2];
            Parts[0, 0] = new Part(new List<Pin>() { 
                new Pin("west0", MatterType.Light, RectSide.Left),
            });
            Parts[1, 0] = new Part(new List<Pin>() {
                new Pin("east0", MatterType.Light, RectSide.Right),
            });
            Parts[0, 1] = new Part(new List<Pin>() {
                new Pin("west1", MatterType.Light, RectSide.Left),
            });
            Parts[1, 1] = new Part(new List<Pin>() {
                new Pin("east1", MatterType.Light, RectSide.Right),
            });

            var leftUp = PinIdLeft(0, 0);
            var leftDown = PinIdLeft(0, 1);
            var rightUp = PinIdRight(1, 0);
            var rightDown = PinIdRight(1, 1);
            // setting up the SMatrix
            var allPins = new List<Guid> {
                leftUp,
                leftDown,
                rightUp,
                rightDown,
            };
            Connections = new SMatrix(allPins);
            var connectionweights = new Dictionary<(Guid, Guid), Complex>()
            {
                { (leftUp, rightUp), new Complex(0.5, 0) },
                { (leftUp, rightDown), new Complex(0.5, 0) },
                { (leftDown, rightUp), new Complex(0.5, 0) },
                { (leftDown, rightDown), new Complex(0.5, 0) },

                { (rightUp, leftUp), new Complex(0.5, 0) },
                { (rightUp, leftDown), new Complex(0.5, 0) },
                { (rightDown, leftUp), new Complex(0.5, 0) },
                { (rightDown, leftDown), new Complex(0.5, 0) },

            };
            Connections.SetValues(connectionweights);
        }
    }
}
