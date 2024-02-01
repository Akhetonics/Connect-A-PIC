using CAP_Core;
using CAP_Core.Components;
using CAP_Core.Components.ComponentHelpers;
using CAP_Core.Grid;
using CAP_Core.Tiles.Grid;
using System.Numerics;

namespace UnitTests.Components
{
    public class SMatrixTests
    {
        [Fact]
        public async Task TestDirectionalCoupler()
        {
            var directionalCoupler = TestComponentFactory.CreateDirectionalCoupler();
            var grid = new GridManager(20, 10);
            grid.PlaceComponent(0, grid.ExternalPorts[0].TilePositionY, directionalCoupler);
            var laserType = grid.GetUsedExternalInputs().First().Input.LaserType;
            var gridSMatrixAnalyzer = new GridSMatrixAnalyzer(grid, laserType.WaveLengthInNm);
            var lightPropagation = await gridSMatrixAnalyzer.CalculateLightPropagationAsync(new CancellationTokenSource());

            // test directionalCoupler
            var allComponentsSMatrices = gridSMatrixAnalyzer.GetAllComponentsSMatrices(gridSMatrixAnalyzer.LaserWaveLengthInNm);
            var debug_directionalComponentMatrix = gridSMatrixAnalyzer.ToString();
            var connections = allComponentsSMatrices[0].GetNonNullValues();
            var UpperToRightConnection = connections.Single(e => e.Key == (directionalCoupler.PinIdLeftIn(0, 0), directionalCoupler.PinIdRightOut(1, 0))).Value;
            var LowerToRightConnection = connections.Single(e => e.Key == (directionalCoupler.PinIdLeftIn(0, 1), directionalCoupler.PinIdRightOut(1, 1))).Value;
            var UpperToLeftConnection = connections.Single(e => e.Key == (directionalCoupler.PinIdRightIn(1, 0), directionalCoupler.PinIdLeftOut(0, 0))).Value;
            var LowerToLeftConnection = connections.Single(e => e.Key == (directionalCoupler.PinIdRightIn(1, 1), directionalCoupler.PinIdLeftOut(0, 1))).Value;

            var directionalCouplerLightIn = lightPropagation[directionalCoupler.PinIdLeftIn()];
            var directionalCouplerLightOutUp = lightPropagation[directionalCoupler.PinIdRightOut(1, 0)];
            var directionalCouplerLightOutDown = lightPropagation[directionalCoupler.PinIdRightOut(1, 1)];

            Assert.Equal(0.5, UpperToRightConnection.Magnitude);
            Assert.Equal(0.5, LowerToRightConnection.Magnitude);
            Assert.Equal(0.5, UpperToLeftConnection.Magnitude);
            Assert.Equal(0.5, LowerToLeftConnection.Magnitude);

            Assert.Equal(1, directionalCouplerLightIn.Magnitude);
            Assert.Equal(0.5, directionalCouplerLightOutUp.Magnitude);
            Assert.Equal(0.5, directionalCouplerLightOutDown.Magnitude);
        }

        [Fact]
        public async Task TestTunableDirectionalCoupler()
        {
            var directionalCoupler = TestComponentFactory.CreateComponent(TestComponentFactory.DirectionalCouplerJSON);
            var grid = new GridManager(20, 10);
            grid.PlaceComponent(0, grid.ExternalPorts[0].TilePositionY, directionalCoupler);
            var slider = directionalCoupler.GetSlider(0);
            var allSliders = directionalCoupler.GetAllSliders();
            slider.Value = 0.5;
            var laserType = grid.GetUsedExternalInputs().First().Input.LaserType;
            var gridSMatrixAnalyzer = new GridSMatrixAnalyzer(grid, laserType.WaveLengthInNm);
            var lightPropagation = await gridSMatrixAnalyzer.CalculateLightPropagationAsync(new CancellationTokenSource());

            // test directionalCoupler
            var allComponentsSMatrices = gridSMatrixAnalyzer.GetAllComponentsSMatrices(gridSMatrixAnalyzer.LaserWaveLengthInNm);
            var debug_directionalComponentMatrix = gridSMatrixAnalyzer.ToString();
            var nonLinearConnections = allComponentsSMatrices[0].NonLinearConnections;
            var UpperToRightConnection = nonLinearConnections.Single(e => e.Key == (directionalCoupler.PinIdLeftIn(0, 0), directionalCoupler.PinIdRightOut(1, 0))).Value;
            var LowerToRightConnection = nonLinearConnections.Single(e => e.Key == (directionalCoupler.PinIdLeftIn(0, 1), directionalCoupler.PinIdRightOut(1, 1))).Value;
            var UpperToLeftConnection = nonLinearConnections.Single(e => e.Key == (directionalCoupler.PinIdRightIn(1, 0), directionalCoupler.PinIdLeftOut(0, 0))).Value;
            var LowerToLeftConnection = nonLinearConnections.Single(e => e.Key == (directionalCoupler.PinIdRightIn(1, 1), directionalCoupler.PinIdLeftOut(0, 1))).Value;

            var directionalCouplerLightIn = lightPropagation[directionalCoupler.PinIdLeftIn()];
            var directionalCouplerLightOutUp = lightPropagation[directionalCoupler.PinIdRightOut(1, 0)];
            var directionalCouplerLightOutDown = lightPropagation[directionalCoupler.PinIdRightOut(1, 1)];

            foreach (var item in nonLinearConnections)
            {
                Assert.False(item.Value.IsInnerLoopFunction); // they should all not be in the inner loop as they don't make use of any PIN variable
                var weight =  UpperToRightConnection.CalcConnectionWeightAsync(allComponentsSMatrices[0].SliderReference.Values.Cast<object>().ToList()).Magnitude;
                Assert.Equal(0.5,weight);
            }
            Assert.Equal(1, allComponentsSMatrices[0]?.SliderReference.Count);
            
            Assert.Equal(1, directionalCouplerLightIn.Magnitude);
            Assert.Equal(0.5, directionalCouplerLightOutUp.Magnitude);
            Assert.Equal(0.5, directionalCouplerLightOutDown.Magnitude);
        }

        [Fact]
        public async Task TestSMatrixForGrid()
        {
            var straight = TestComponentFactory.CreateStraightWaveGuide();
            var rotatedStraight = TestComponentFactory.CreateStraightWaveGuide();
            var directionalCoupler = TestComponentFactory.CreateDirectionalCoupler();
            var secondStraight = TestComponentFactory.CreateStraightWaveGuide();

            rotatedStraight.RotateBy90CounterClockwise();
            var grid = new GridManager(20, 10);
            var inputPort = grid.ExternalPorts[0];
            grid.PlaceComponent(0, inputPort.TilePositionY, straight);
            grid.PlaceComponent(1, inputPort.TilePositionY, directionalCoupler);
            grid.PlaceComponent(3, inputPort.TilePositionY, secondStraight);
            grid.PlaceComponent(0, inputPort.TilePositionY + 1, rotatedStraight);

            var laserType = grid.GetUsedExternalInputs().First().Input.LaserType;
            var gridSMatrixAnalyzer = new GridSMatrixAnalyzer(grid, laserType.WaveLengthInNm);
            var lightValues = await gridSMatrixAnalyzer.CalculateLightPropagationAsync(new CancellationTokenSource());
            var allComponentsSMatrices = gridSMatrixAnalyzer.GetAllComponentsSMatrices(laserType.WaveLengthInNm);
            var Straight_LiRoConnection = allComponentsSMatrices[0].GetNonNullValues().Single(b => b.Key == (straight.PinIdLeftIn(), straight.PinIdRightOut())).Value;
            var Straight_RiLoConnection = allComponentsSMatrices[0].GetNonNullValues().Single(b => b.Key == (straight.PinIdRightIn(), straight.PinIdLeftOut())).Value;

            // test straightComponent light throughput
            var straightCompLightVal = lightValues[straight.PinIdRightOut()];

            // test whole circuit's light throughput
            var circuitLightVal = lightValues[secondStraight.PinIdLeftIn()]; // the light flows into the grating and therefore leaves the circuit.
            string allDebugInformation = gridSMatrixAnalyzer.ToString();

            Assert.Contains(secondStraight.PinIdLeftOut(), lightValues);
            Assert.Equal(1, straightCompLightVal.Real);
            Assert.Equal(0.5, circuitLightVal.Real);
            Assert.Equal(1, Straight_LiRoConnection.Real);
            Assert.Equal(1, Straight_RiLoConnection.Real);

        }
        [Fact]
        public void TestSystemSMatrixManually()
        {
            // component matrix one
            var pinID1 = Guid.NewGuid();
            var pinID2 = Guid.NewGuid();
            List<Guid> PinIDs = new() { pinID1, pinID2 };
            SMatrix matrix = new(PinIDs, new());
            matrix.SetValues(new Dictionary<(Guid, Guid), Complex>() { { (pinID1, pinID2), new Complex(0.5f, 0) } });
            matrix.SetValues(new Dictionary<(Guid, Guid), Complex>() { { (pinID2, pinID1), new Complex(0.5f, 0) } });

            // component matrix two
            var m2_pinID1 = Guid.NewGuid();
            var m2_pinID2 = Guid.NewGuid();
            List<Guid> m2_PinIDs = new() { m2_pinID1, m2_pinID2 };
            SMatrix m2_matrix = new(m2_PinIDs, new());
            m2_matrix.SetValues(new Dictionary<(Guid, Guid), Complex>() { { (m2_pinID1, m2_pinID2), new Complex(0.5f, 0) } });
            m2_matrix.SetValues(new Dictionary<(Guid, Guid), Complex>() { { (m2_pinID2, m2_pinID1), new Complex(0.5f, 0) } });

            // connection matrix
            List<Guid> con_PinIDs = new() { pinID2, m2_pinID1 };
            SMatrix con_matrix = new(con_PinIDs, new());
            con_matrix.SetValues(new Dictionary<(Guid, Guid), Complex>() { { (pinID2, m2_pinID1), new Complex(0.5f, 0) } });
            con_matrix.SetValues(new Dictionary<(Guid, Guid), Complex>() { { (m2_pinID1, pinID2), new Complex(0.5f, 0) } });

            // system matrix
            var sysMatrix = SMatrix.CreateSystemSMatrix(new List<SMatrix>() { matrix, m2_matrix, con_matrix });
            var connections = sysMatrix.GetNonNullValues();
            Assert.Equal(0.5f, connections[(pinID2, m2_pinID1)].Real);
        }
    }
}