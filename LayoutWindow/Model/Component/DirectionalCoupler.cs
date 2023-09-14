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
        private Dictionary<(Guid, Guid), Complex> Connectionweights;

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

            var leftUpIn = PinIdLeftIn(0, 0);
            var leftUpOut = PinIdLeftOut(0, 0);
            var leftDownIn = PinIdLeftIn(0, 1);
            var leftDownOut = PinIdLeftOut(0, 1);
            var rightUpIn = PinIdRightIn(1, 0);
            var rightUpOut = PinIdRightOut(1, 0);
            var rightDownIn = PinIdRightIn(1, 1);
            var rightDownOut = PinIdRightOut(1, 1);
            // setting up the SMatrix
            var allPins = new List<Guid> {
                leftUpIn,
                leftUpOut,
                leftDownIn,
                leftDownOut,
                rightUpIn,
                rightUpOut,
                rightDownIn,
                rightDownOut,
            };
            Connections = new SMatrix(allPins);
            Connectionweights = new Dictionary<(Guid, Guid), Complex>()
            {
                { (leftUpIn, rightUpOut), new Complex(0.5, 0) },
                { (leftUpIn, rightDownOut), new Complex(0.5, 0) },
                { (leftDownIn, rightUpOut), new Complex(0.5, 0) },
                { (leftDownIn, rightDownOut), new Complex(0.5, 0) },

                { (rightUpIn, leftUpOut), new Complex(0.5, 0) },
                { (rightUpIn, leftDownOut), new Complex(0.5, 0) },
                { (rightDownIn, leftUpOut), new Complex(0.5, 0) },
                { (rightDownIn, leftDownOut), new Complex(0.5, 0) },

            };
            
            Connections.SetValues(Connectionweights);
        }

    }
}
