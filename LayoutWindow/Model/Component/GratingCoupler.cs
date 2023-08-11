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
    public partial class GratingCoupler : ComponentBase
    {
        
        public GratingCoupler()
        {
            Parts = new Part[1, 1];
            Parts[0, 0] = new ();
            Parts[0, 0].Rotation90 = DiscreteRotation.R0;
            Parts[0, 0].InitializePin(RectangleSide.Right, "Right", MatterType.Light);
            Parts[0, 0].InitializePin(RectangleSide.Up, "Up", MatterType.None);
            Parts[0, 0].InitializePin(RectangleSide.Left, "Left", MatterType.None);
            Parts[0, 0].InitializePin(RectangleSide.Down, "Down", MatterType.None);
            
            // setting up the SMatrix
            var allPins = new List<Guid> {
                PinIdRight(0,0),
            };
            Connections = new SMatrix(allPins);
            var connectionweights = new Dictionary<(Guid, Guid), Complex>
            {
                { (Parts[0, 0].GetPinAt(RectangleSide.Right).ID, Parts[0, 0].GetPinAt(RectangleSide.Right).ID), new Complex(1, 0) }
            };

            Connections.setValues(connectionweights);
        }

        public override string NazcaFunctionName { get; set; } = "placeCell_GratingCoupler";
        public override string NazcaFunctionParameters { get; } 
    }
}
