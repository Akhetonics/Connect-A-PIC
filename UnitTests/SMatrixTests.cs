using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.TransferFunction;
using Model;
using Moq;
using System.ComponentModel;
using System.Numerics;
using TransferFunction;

namespace UnitTests
{
    public class SMatrixTests
    {
        [Fact]
        public void TestDirectionalCoupler()
        {
            var directionalCoupler = new DirectionalCoupler();
            var grid = new Grid(20,10);
            grid.PlaceComponent(1, 3, directionalCoupler);
            var gridSMatrixAnalyzer = new GridSMatrixAnalyzer(grid);
            var connectionPairs = gridSMatrixAnalyzer.CalculateLightPropagation(4);

            // test directionalCoupler
            var directionalCompLightValUp = connectionPairs[(directionalCoupler.PinIdLeftIn(0, 0), directionalCoupler.PinIdRightOut(1, 0))];
            var directionalCompLightValDown = connectionPairs[(directionalCoupler.PinIdLeftIn(0, 1), directionalCoupler.PinIdRightOut(1, 1))];
            var directionalCompLightValUpDown = connectionPairs[(directionalCoupler.PinIdLeftIn(0, 0), directionalCoupler.PinIdRightOut(1, 1))];
            var directionalCompLightValDownUp = connectionPairs[(directionalCoupler.PinIdLeftIn(0, 1), directionalCoupler.PinIdRightOut(1, 0))];
            Assert.True(directionalCompLightValUp.Real > 0);
            Assert.True(directionalCompLightValDown.Real > 0);
            Assert.True(directionalCompLightValUpDown.Real > 0);
            Assert.True(directionalCompLightValDownUp.Real > 0);
        }

        [Fact] 
        public void TestLightPropagationOverTime()
        {
            // I want to calculate the lightvalues at each PIN for the whole lifetime of the matrix.
            // I will go from n = 1 to 200 and add all matrices to one and just get the light values for each pin
            // first I might want to create a grid and some components.
            
        }
        [Fact]
        public void TestSMatrixForGrid()
        {
            var straight = new StraightWaveGuide();
            var directionalCoupler = new DirectionalCoupler();
            var Grating = new GratingCoupler();

            var grid = new Grid(20, 10);
            grid.PlaceComponent(0, 3, straight);
            grid.PlaceComponent(1, 3, directionalCoupler);
            grid.PlaceComponent(3, 3, Grating);
            var gridSMatrixAnalyzer = new GridSMatrixAnalyzer(grid);
            var systemMatrixConnections = gridSMatrixAnalyzer.CalculateLightPropagation(100);

            // test straightcomponent light throughput
            var straightCompLightVal = systemMatrixConnections[(straight.PinIdLeftIn(), straight.PinIdRightOut())];

            // test whole circuit's light throughput
            var connectionPairFromBeginningToEndComponent = (straight.PinIdLeftIn(), Grating.PinIdLeftOut());
            string debugInfo = systemMatrixConnections.ToString() ?? "";
            debugInfo += gridSMatrixAnalyzer.CreateSystemSMatrix().ToString().Replace("+0,0i" , "\t");
            // connections I would expect are:
            // straight to directional
            var straightToDirectionalVal = systemMatrixConnections[(straight.PinIdLeftIn() , directionalCoupler.PinIdRightOut(1,0))];
            // directional to grating
            // grating Pin ID
            var gratingPinID = Grating.PinIdLeftOut();
            var circuitLightVal = systemMatrixConnections[connectionPairFromBeginningToEndComponent];
            Assert.Contains(connectionPairFromBeginningToEndComponent, systemMatrixConnections);
            Assert.True(straightCompLightVal.Real > 0);
            Assert.True(circuitLightVal.Real > 0 );
        }
        [Fact]
        public void TestSystemSMatrixManually()
        {
            // component matrix one
            var pinID1 = Guid.NewGuid();
            var pinID2 = Guid.NewGuid();
            List<Guid> PinIDs = new() { pinID1, pinID2 };
            SMatrix matrix = new(PinIDs);
            matrix.SetValues(new Dictionary<(Guid, Guid), Complex>() { { (pinID1, pinID2 ), new Complex(0.5f, 0) } });
            matrix.SetValues(new Dictionary<(Guid, Guid), Complex>() { { (pinID2, pinID1), new Complex(0.5f, 0) } });

            // component matrix two
            var m2_pinID1 = Guid.NewGuid();
            var m2_pinID2 = Guid.NewGuid();
            List<Guid> m2_PinIDs = new() { m2_pinID1, m2_pinID2 };
            SMatrix m2_matrix = new(m2_PinIDs);
            m2_matrix.SetValues(new Dictionary<(Guid, Guid), Complex>() { { (m2_pinID1, m2_pinID2), new Complex(0.5f, 0) } });
            m2_matrix.SetValues(new Dictionary<(Guid, Guid), Complex>() { { (m2_pinID2, m2_pinID1), new Complex(0.5f, 0) } });

            // connection matrix
            List<Guid> con_PinIDs = new() { pinID2, m2_pinID1 };
            SMatrix con_matrix = new(con_PinIDs);
            con_matrix.SetValues(new Dictionary<(Guid, Guid), Complex>() { { (pinID2, m2_pinID1), new Complex(0.5f, 0) } });
            con_matrix.SetValues(new Dictionary<(Guid, Guid), Complex>() { { (m2_pinID1, pinID2), new Complex(0.5f, 0) } });

            // system matrix
            var sysMatrix = SMatrix.CreateSystemSMatrix(new List<SMatrix>() { matrix, m2_matrix, con_matrix });
            var connections = sysMatrix.GetNonNullValues();
            Assert.Equal(0.5f, connections[(pinID2, m2_pinID1)].Real);
        }
    }
}