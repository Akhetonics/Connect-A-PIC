using CAP_Core.Components.ComponentHelpers;
using CAP_Core.LightFlow;
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
                new Pin("west0", MatterType.Light, RectSide.Left), 
                new Pin("east0", MatterType.Light, RectSide.Right)
            });

            var leftUpIn = parts[0, 0].GetPinAt(RectSide.Left).IDInFlow;
            var leftUpOut = parts[0, 0].GetPinAt(RectSide.Left).IDOutFlow;
            var rightUpIn = parts[0, 0].GetPinAt(RectSide.Right).IDInFlow;
            var rightUpOut = parts[0, 0].GetPinAt(RectSide.Right).IDOutFlow;
            // setting up the SMatrix
            var allPins = new List<Guid> {
                leftUpIn,
                leftUpOut,
                rightUpIn,
                rightUpOut,
            };
            var connections = new SMatrix(allPins);
            var ConnectionWeights = new Dictionary<(Guid, Guid), Complex>()
            {
                { (leftUpIn, rightUpOut), new Complex(1, 0) },
                { (rightUpIn, leftUpOut), new Complex(1, 0) },
            };

            connections.SetValues(ConnectionWeights);
            return new Component(connections, "placeCell_StraightWG", "", parts, 0, DiscreteRotation.R0);
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

            var leftUpIn = parts[0, 0].GetPinAt(RectSide.Left).IDInFlow;
            var leftUpOut = parts[0, 0].GetPinAt(RectSide.Left).IDOutFlow;
            var leftDownIn = parts[0, 1].GetPinAt(RectSide.Left).IDInFlow;
            var leftDownOut = parts[0, 1].GetPinAt(RectSide.Left).IDOutFlow;
            var rightUpIn = parts[1, 0].GetPinAt(RectSide.Right).IDInFlow;
            var rightUpOut = parts[1, 0].GetPinAt(RectSide.Right).IDOutFlow;
            var rightDownIn = parts[1, 1].GetPinAt(RectSide.Right).IDInFlow;
            var rightDownOut = parts[1, 1].GetPinAt(RectSide.Right).IDOutFlow;
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
            var connections = new SMatrix(allPins);
            var ConnectionWeights = new Dictionary<(Guid, Guid), Complex>()
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

            connections.SetValues(ConnectionWeights);
            return new Component(connections, "placeCell_DirectionalCoupler", "", parts, 0, DiscreteRotation.R0);
        }
    
    }
}
