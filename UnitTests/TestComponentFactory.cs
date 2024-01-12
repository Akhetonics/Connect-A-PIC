using CAP_Core.Components;
using CAP_Core.Components.ComponentHelpers;
using CAP_Core.Tiles;
using System.Numerics;

namespace UnitTests
{
    public class TestComponentFactory
    {
        public static Component CreateStraightWaveGuide()
        {
            int widthInTiles = 1;
            int heightInTiles = 1;

            Part[,] parts = new Part[widthInTiles, heightInTiles];

            parts[0, 0] = new Part(new List<Pin>() {
                new ("west0", MatterType.Light, RectSide.Left),
                new ("east0", MatterType.Light, RectSide.Right)
            });

            var leftIn = parts[0, 0].GetPinAt(RectSide.Left).IDInFlow;
            var rightOut = parts[0, 0].GetPinAt(RectSide.Right).IDOutFlow;
            var rightIn = parts[0, 0].GetPinAt(RectSide.Right).IDInFlow;
            var leftOut = parts[0, 0].GetPinAt(RectSide.Left).IDOutFlow;

            var connections = new List<Connection>
            {
                new () { FromPin = leftIn, ToPin = rightOut, Magnitude = 1, WireLengthNM = 0 },
                new () { FromPin = rightIn, ToPin = leftOut, Magnitude = 1, WireLengthNM = 0 }
            };

            return new Component(connections, "placeCell_StraightWG", "", parts, 0, "Straight", DiscreteRotation.R0);
        }

        public static Component CreateDirectionalCoupler()
        {
            int widthInTiles = 2;
            int heightInTiles = 2;

            Part[,] parts = new Part[widthInTiles, heightInTiles];


            parts[0, 0] = new Part(new List<Pin>() { new Pin("west0", MatterType.Light, RectSide.Left) });
            parts[1, 0] = new Part(new List<Pin>() { new Pin("east0", MatterType.Light, RectSide.Right) });
            parts[1, 1] = new Part(new List<Pin>() { new Pin("east1", MatterType.Light, RectSide.Right) });
            parts[0, 1] = new Part(new List<Pin>() { new Pin("west1", MatterType.Light, RectSide.Left) });

            // setting up the connections
            var leftUpIn = parts[0, 0].GetPinAt(RectSide.Left).IDInFlow;
            var leftUpOut = parts[0, 0].GetPinAt(RectSide.Left).IDOutFlow;
            var leftDownIn = parts[0, 1].GetPinAt(RectSide.Left).IDInFlow;
            var leftDownOut = parts[0, 1].GetPinAt(RectSide.Left).IDOutFlow;
            var rightUpIn = parts[1, 0].GetPinAt(RectSide.Right).IDInFlow;
            var rightUpOut = parts[1, 0].GetPinAt(RectSide.Right).IDOutFlow;
            var rightDownIn = parts[1, 1].GetPinAt(RectSide.Right).IDInFlow;
            var rightDownOut = parts[1, 1].GetPinAt(RectSide.Right).IDOutFlow;
            
            var connections = new List<Connection>
            {
                new() { FromPin = leftUpIn, ToPin = rightUpOut, Magnitude = 0.5f, WireLengthNM = 0 },
                new() { FromPin = leftUpIn, ToPin = rightDownOut, Magnitude = 0.5f, WireLengthNM = 0 },
                new() { FromPin = leftDownIn, ToPin = rightUpOut, Magnitude = 0.5f, WireLengthNM = 0 },
                new() { FromPin = leftDownIn, ToPin = rightDownOut, Magnitude = 0.5f, WireLengthNM = 0 },
                new() { FromPin = rightUpIn, ToPin = leftUpOut, Magnitude = 0.5f, WireLengthNM = 0 },
                new() { FromPin = rightUpIn, ToPin = leftDownOut, Magnitude = 0.5f, WireLengthNM = 0 },
                new() { FromPin = rightDownIn, ToPin = leftUpOut, Magnitude = 0.5f, WireLengthNM = 0 },
                new() { FromPin = rightDownIn, ToPin = leftDownOut, Magnitude = 0.5f, WireLengthNM = 0 }
            };
            return new Component(connections, "placeCell_DirectionalCoupler", "", parts, 0,"DirectionalCoupler", DiscreteRotation.R0);
        }

    }
}
