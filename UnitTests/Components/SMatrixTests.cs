using CAP_Core;
using CAP_Core.Components;
using CAP_Core.Components.ComponentHelpers;
using CAP_Core.Grid;
using CAP_Core.LightCalculation;
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
            var gridSMatrixAnalyzer = new GridLightCalculator(grid);
            var lightPropagation = await gridSMatrixAnalyzer.CalculateLightPropagationAsync(new CancellationTokenSource(), laserType.WaveLengthInNm);
            
            var directionalCouplerLightIn = lightPropagation[directionalCoupler.PinIdLeftIn()];
            var directionalCouplerLightOutUp = lightPropagation[directionalCoupler.PinIdRightOut(1, 0)];
            var directionalCouplerLightOutDown = lightPropagation[directionalCoupler.PinIdRightOut(1, 1)];


            Assert.Equal(1, directionalCouplerLightIn.Magnitude, 0.000000001);
            Assert.Equal(0.5, directionalCouplerLightOutUp.Magnitude, 0.000000001);
            Assert.Equal(0.5, directionalCouplerLightOutDown.Magnitude, 0.000000001);
        }

        [Fact]
        public async Task TestTunableDirectionalCoupler()
        {
            var directionalCoupler = TestComponentFactory.CreateComponent(TestComponentFactory.DirectionalCouplerJSON);
            var grid = new GridManager(20, 10);
            grid.PlaceComponent(0, grid.ExternalPorts[0].TilePositionY, directionalCoupler);
            var slider = directionalCoupler.GetSlider(0);
            var allSliders = directionalCoupler.GetAllSliders();
            slider.Value = 0.75;
            var laserType = grid.GetUsedExternalInputs().First().Input.LaserType;
            var gridSMatrixAnalyzer = new GridLightCalculator(grid);
            var lightPropagation = await gridSMatrixAnalyzer.CalculateLightPropagationAsync(new CancellationTokenSource(), laserType.WaveLengthInNm);
            var directionalCouplerLightIn = lightPropagation[directionalCoupler.PinIdLeftIn()];
            var directionalCouplerLightOutUp = lightPropagation[directionalCoupler.PinIdRightOut(1, 0)];
            var directionalCouplerLightOutDown = lightPropagation[directionalCoupler.PinIdRightOut(1, 1)];

            Assert.Equal(1, directionalCouplerLightIn.Magnitude, 0.000000001);
            Assert.Equal(0.25, directionalCouplerLightOutUp.Magnitude, 0.000000001);
            Assert.Equal(0.75, directionalCouplerLightOutDown.Magnitude, 0.000000001);
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
            var gridSMatrixAnalyzer = new GridLightCalculator(grid);
            var lightValues = await gridSMatrixAnalyzer.CalculateLightPropagationAsync(new CancellationTokenSource(), laserType.WaveLengthInNm);
            
            // test straightComponent light throughput
            var straightCompLightVal = lightValues[straight.PinIdRightOut()];

            // test whole circuit's light throughput
            var circuitLightVal = lightValues[secondStraight.PinIdLeftIn()]; // the light flows into the grating and therefore leaves the circuit.
            string allDebugInformation = gridSMatrixAnalyzer.ToString();

            Assert.Contains(secondStraight.PinIdLeftOut(), lightValues);
            Assert.Equal(1, straightCompLightVal.Real, 0.000000001);
            Assert.Equal(0.5, circuitLightVal.Real, 0.000000001);
            
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