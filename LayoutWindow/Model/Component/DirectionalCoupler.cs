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
    public partial class DirectionalCoupler : ComponentBase
    {
        
        public DirectionalCoupler()
        {
            Parts = new Part[2, 1];
            Parts[0, 0] = new ();
            Parts[0, 0].Rotation90 = DiscreteRotation.R0;
            Parts[0, 0].InitializePin(RectangleSide.Right, "Right", MatterType.None);
            Parts[0, 0].InitializePin(RectangleSide.Up, "Up", MatterType.Light);
            Parts[0, 0].InitializePin(RectangleSide.Left, "Left", MatterType.Light);
            Parts[0, 0].InitializePin(RectangleSide.Down, "Down", MatterType.Light);
            Parts[1, 0] = new ();
            Parts[1, 0].InitializePin(RectangleSide.Right, "1", MatterType.Light);
            Parts[1, 0].InitializePin(RectangleSide.Up, "2", MatterType.Light);
            Parts[1, 0].InitializePin(RectangleSide.Left, "3", MatterType.None);
            Parts[1, 0].InitializePin(RectangleSide.Down, "4", MatterType.Light);

            // setting up the SMatrix
            var allPins = new List<Guid> {
                PinIdUp(0,0),
                PinIdLeft(0,0),
                PinIdDown(0,0),
                PinIdUp(1,0),
                PinIdRight(1,0),
                PinIdDown(1,0),
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
