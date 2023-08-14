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
    public class DirectionalCoupler : ComponentBase
    {
        public int deltaLength { get; set; } = 50;
        public override string NazcaFunctionName { get; set; } = "placeCell_DirectionalCoupler";
        public override string NazcaFunctionParameters { get => $"deltaLength = {deltaLength}"; }

        public DirectionalCoupler()
        {
            Parts = new Part[2, 1];
            Parts[0, 0] = CreatePart(RectangleSide.Left, RectangleSide.Right);
            Parts[1, 0] = CreatePart(RectangleSide.Left, RectangleSide.Right);

            // setting up the SMatrix
            var allPins = new List<Guid> {
                PinIdRight(0,0),
                PinIdLeft(0,0),
                PinIdRight(1,0),
                PinIdLeft(1,0),
            };
            Connections = new SMatrix(allPins);
            var connectionweights = new Dictionary<(Guid, Guid), Complex>();
            float lightPower = 0.1f;
            foreach (RectangleSide side in Enum.GetValues(typeof(RectangleSide)))
            {
                foreach (RectangleSide side2 in Enum.GetValues(typeof(RectangleSide)))
                {
                    lightPower += 0.1f;
                    connectionweights.Add((Parts[0,0].GetPinAt(side).ID, Parts[0,0].GetPinAt(side2).ID), new Complex(lightPower, 0.2));
                    lightPower += 0.1f;
                    connectionweights.Add((Parts[0,0].GetPinAt(side).ID, Parts[1,0].GetPinAt(side2).ID), new Complex(lightPower, 0.2));
                    lightPower += 0.1f;
                    connectionweights.Add((Parts[1,0].GetPinAt(side).ID, Parts[0,0].GetPinAt(side2).ID), new Complex(lightPower, 0.2));
                    lightPower += 0.1f;
                    connectionweights.Add((Parts[1,0].GetPinAt(side).ID, Parts[1,0].GetPinAt(side2).ID), new Complex(lightPower, 0.2));
                }
            }
            
            Connections.setValues(connectionweights);
        }
    }
}
