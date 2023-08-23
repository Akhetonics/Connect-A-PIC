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
        public void TestSMatrix()
        {
            var straight = new StraightWaveGuide();
            var directionalCoupler = new DirectionalCoupler();
            var Grating = new GratingCoupler();
            
            var grid = new Grid(20,10);
            grid.PlaceComponent(0, 3, straight);
            grid.PlaceComponent(1, 3, directionalCoupler);
            grid.PlaceComponent(3, 3, Grating);
            var gridSMatrixAnalyzer = new GridSMatrixAnalyzer(grid);
            var systemMatrix = gridSMatrixAnalyzer.CreateSystemSMatrix();
            var connectionPairs = systemMatrix.GetValues();
            var startPinId = straight.PinIdLeft(0, 0);
            var endPinId = Grating.PinIdLeft(0, 0);
            var LightValue = connectionPairs[(startPinId, endPinId)];
            // Ensure result is valid according to constraints
            Assert.Equal(LightValue, Complex.One);
            
        }

    }
}