using CAP_Core;
using CAP_Core.Components.ComponentHelpers;
using CAP_Core.Grid;
using CAP_Core.LightCalculation;
using System.Numerics;
using UnitTests.Grid;

namespace UnitTests.LightCalculation
{
    public class SMatrixTests
    {
        [Fact]
        public async Task TestDirectionalCoupler()
        {
            var directionalCoupler = TestComponentFactory.CreateDirectionalCoupler();
            var grid = GridHelpers.InitializeGridWithComponents(20, 10);
            grid.ComponentMover.PlaceComponent(0, grid.ExternalPortManager.ExternalPorts[0].TilePositionY, directionalCoupler);
            var laserType = grid.ExternalPortManager.GetUsedExternalInputs().First().Input.LaserType;
            var gridSMatrixAnalyzer = new GridLightCalculator(new SystemMatrixBuilder(grid), grid);
            var lightPropagation = await gridSMatrixAnalyzer.CalculateFieldPropagationAsync(new CancellationTokenSource(), laserType.WaveLengthInNm);

            var directionalCouplerPowerIn = lightPropagation[(Guid)directionalCoupler.PinIdLeftIn()];
            var directionalCouplerPowerOutUp = lightPropagation[(Guid)directionalCoupler.PinIdRightOut(1, 0)];
            var directionalCouplerPowerOutDown = lightPropagation[(Guid)directionalCoupler.PinIdRightOut(1, 1)];


            Assert.Equal(1, Math.Pow(directionalCouplerPowerIn.Magnitude, 2), 0.000000001);
            Assert.Equal(0.5, Math.Pow(directionalCouplerPowerOutUp.Magnitude, 2), 0.000000001);
            Assert.Equal(0.5, Math.Pow(directionalCouplerPowerOutDown.Magnitude, 2), 0.000000001);
        }

        [Fact]
        public async Task TestTunableDirectionalCoupler()
        {
            // keep in mind to hit "create new" after editing the Json files in the Resources
            var straight = TestComponentFactory.CreateComponent(TestComponentFactory.StraightWGJson);
            var directionalCoupler = TestComponentFactory.CreateComponent(TestComponentFactory.DirectionalCouplerJSON);
            var directionalCoupler2 = TestComponentFactory.CreateComponent(TestComponentFactory.DirectionalCouplerJSON);
            var grid = GridHelpers.InitializeGridWithComponents(20,10);
            var redPortY = grid.ExternalPortManager.ExternalPorts[0].TilePositionY;
            grid.ComponentMover.PlaceComponent(0, redPortY, straight);
            grid.ComponentMover.PlaceComponent(1, redPortY, directionalCoupler);
            grid.ComponentMover.PlaceComponent(3, redPortY, directionalCoupler2);
            directionalCoupler.GetSlider(0).Value = 0.83;
            var slider = directionalCoupler2.GetSlider(0);
            slider.Value = 0.76;

            //act
            var laserType = grid.ExternalPortManager.GetUsedExternalInputs().First().Input.LaserType;
            var gridSMatrixAnalyzer = new GridLightCalculator(new SystemMatrixBuilder(grid), grid);
            var lightPropagation = await gridSMatrixAnalyzer.CalculateFieldPropagationAsync(new CancellationTokenSource(), laserType.WaveLengthInNm);

            // straight WG
            var straightFieldIn = lightPropagation[(Guid)straight.PinIdLeftIn()];
            var straightFieldOut = lightPropagation[(Guid)straight.PinIdRightOut()];
            var straightPower = Math.Pow(straightFieldOut.Magnitude, 2);
            // first coupler
            var firstFieldInUp = lightPropagation[(Guid)directionalCoupler.PinIdLeftIn(0, 0)];
            var firstFieldInDown = lightPropagation[(Guid)directionalCoupler.PinIdLeftIn(0, 1)];
            var firstFieldOutUp = lightPropagation[(Guid)directionalCoupler.PinIdRightOut(1, 0)];
            var firstFieldOutDown = lightPropagation[(Guid)directionalCoupler.PinIdRightOut(1, 1)];
            var firstCouplerPower = Math.Pow(firstFieldOutUp.Magnitude, 2) + Math.Pow(firstFieldOutDown.Magnitude, 2);
            // second coupler
            var fieldInUp = lightPropagation[(Guid)directionalCoupler2.PinIdLeftIn(0, 0)];
            var fieldInDown = lightPropagation[(Guid)directionalCoupler2.PinIdLeftIn(0, 1)];
            var fieldOutUp = lightPropagation[(Guid)directionalCoupler2.PinIdRightOut(1, 0)];
            var fieldOutDown = lightPropagation[(Guid)directionalCoupler2.PinIdRightOut(1, 1)];
            var powerOutUp = Math.Pow(fieldOutUp.Magnitude, 2);
            var powerOutDown = Math.Pow(fieldOutDown.Magnitude, 2);
            var powerSum2 = powerOutUp + powerOutDown;

            double phaseShiftThroughWaveGuide = 1.2545454545433;

            var calculatedFieldOutUp = CalculateDirectionalCouplerField(fieldInUp, fieldInDown, slider.Value, phaseShiftThroughWaveGuide);
            var calculatedFieldOutDown = CalculateDirectionalCouplerField(fieldInDown, fieldInUp, slider.Value, phaseShiftThroughWaveGuide);
            var powerED = Math.Pow(calculatedFieldOutUp.Magnitude, 2);
            var powerEB = Math.Pow(calculatedFieldOutDown.Magnitude, 2);
            var powerSum = powerED + powerEB;

            Assert.True(AreComplexNumbersEqual(calculatedFieldOutUp, fieldOutUp));
            Assert.True(AreComplexNumbersEqual(calculatedFieldOutDown, fieldOutDown));
        }

        private static Complex CalculateDirectionalCouplerField(Complex inputUp, Complex inputDown, double lightFlipRatio, double waveGuidePhaseShift)
        {
            return Complex.Sqrt(1 - lightFlipRatio) * inputUp * Complex.Exp(Complex.ImaginaryOne * waveGuidePhaseShift) +
                   Complex.Sqrt(lightFlipRatio) * inputDown * Complex.Exp(Complex.ImaginaryOne * (0.5 * Math.PI + waveGuidePhaseShift));// for now d = 0.5 because the phaseShift with d = slider * pi + PhiNew odes not work
        }

        private static bool AreComplexNumbersEqual(Complex num1, Complex num2, double Tolerance = 1e-6)
        {
            bool magnitudeCloseEnough = Math.Abs(num1.Magnitude - num2.Magnitude) < Tolerance;
            bool phaseCloseEnough = Math.Abs(num1.Phase - num2.Phase) < Tolerance;
            return magnitudeCloseEnough && phaseCloseEnough;
        }
        [Fact]
        public async Task TestSMatrixForGrid()
        {
            var straight = TestComponentFactory.CreateStraightWaveGuide();
            var rotatedStraight = TestComponentFactory.CreateStraightWaveGuide();
            var directionalCoupler = TestComponentFactory.CreateDirectionalCoupler();
            var secondStraight = TestComponentFactory.CreateStraightWaveGuide();

            rotatedStraight.RotateBy90CounterClockwise();
            var grid = GridHelpers.InitializeGridWithComponents(20, 10);
            var inputPort = grid.ExternalPortManager.ExternalPorts[0];
            grid.ComponentMover.PlaceComponent(0, inputPort.TilePositionY, straight);
            grid.ComponentMover.PlaceComponent(1, inputPort.TilePositionY, directionalCoupler);
            grid.ComponentMover.PlaceComponent(3, inputPort.TilePositionY, secondStraight);
            grid.ComponentMover.PlaceComponent(0, inputPort.TilePositionY + 1, rotatedStraight);

            var laserType = grid.ExternalPortManager.GetUsedExternalInputs().First().Input.LaserType;
            var gridSMatrixAnalyzer = new GridLightCalculator(new SystemMatrixBuilder(grid), grid);
            var lightValues = await gridSMatrixAnalyzer.CalculateFieldPropagationAsync(new CancellationTokenSource(), laserType.WaveLengthInNm);

            // test straightComponent light throughput
            var straightCompLightVal = lightValues[(Guid)straight.PinIdRightOut()];

            // test whole circuit's light throughput
            var circuitLightVal = lightValues[(Guid)secondStraight.PinIdLeftIn()]; // the light flows into the grating and therefore leaves the circuit.
            string allDebugInformation = gridSMatrixAnalyzer.ToString();

            Assert.Contains((Guid)secondStraight.PinIdLeftOut(), lightValues);
            Assert.Equal(1, Math.Pow(straightCompLightVal.Real, 2), 0.000000001);
            Assert.Equal(0.5, Math.Pow(circuitLightVal.Real, 2), 0.000000001);

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
