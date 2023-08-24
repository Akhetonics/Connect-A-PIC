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
            var directionalCoupler = new DirectionalCoupler();
            var grid = new Grid(20,10);
            grid.PlaceComponent(3, 3, directionalCoupler);
            var gridSMatrixAnalyzer = new GridSMatrixAnalyzer(grid);
            var systemMatrix = gridSMatrixAnalyzer.CreateSystemSMatrix();
            var connectionPairs = systemMatrix.GetValues();

            // test directionalCoupler
            var directionalCompLightValUp = connectionPairs[(directionalCoupler.PinIdLeft(0, 0), directionalCoupler.PinIdRight(1, 0))];
            var directionalCompLightValDown = connectionPairs[(directionalCoupler.PinIdLeft(0, 1), directionalCoupler.PinIdRight(1, 1))];
            var directionalCompLightValUpDown = connectionPairs[(directionalCoupler.PinIdLeft(0, 0), directionalCoupler.PinIdRight(1, 1))];
            var directionalCompLightValDownUp = connectionPairs[(directionalCoupler.PinIdLeft(0, 1), directionalCoupler.PinIdRight(1, 0))];
            Assert.Equal(new Complex(0.5, 0), directionalCompLightValUp);
            Assert.Equal(new Complex(0.5, 0), directionalCompLightValDown);
            Assert.Equal(new Complex(0.5, 0), directionalCompLightValUpDown);
            Assert.Equal(new Complex(0.5, 0), directionalCompLightValDownUp);
            
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
            var systemMatrix = gridSMatrixAnalyzer.CreateSystemSMatrix();
            systemMatrix.setToPower(1);
            var connectionPairs = systemMatrix.GetValues();

            // test straightcomponent light throughput
            var straightCompLightVal = connectionPairs[(straight.PinIdLeft(), straight.PinIdRight())];
            Assert.Equal(Complex.One, straightCompLightVal);

            // test directionalCoupler
            var directionalCompLightValUp = connectionPairs[(directionalCoupler.PinIdLeft(0, 0), directionalCoupler.PinIdRight(1, 0))];
            var directionalCompLightValDown = connectionPairs[(directionalCoupler.PinIdLeft(0, 1), directionalCoupler.PinIdRight(1, 1))];
            var directionalCompLightValUpDown = connectionPairs[(directionalCoupler.PinIdLeft(0, 0), directionalCoupler.PinIdRight(1, 1))];
            var directionalCompLightValDownUp = connectionPairs[(directionalCoupler.PinIdLeft(0, 1), directionalCoupler.PinIdRight(1, 0))];
            Assert.Equal(new Complex(0.5, 0), directionalCompLightValUp);
            Assert.Equal(new Complex(0.5, 0), directionalCompLightValDown);
            Assert.Equal(new Complex(0.5, 0), directionalCompLightValUpDown);
            Assert.Equal(new Complex(0.5, 0), directionalCompLightValDownUp);

            // test whole circuit's light throughput
            var circuitLightVal = connectionPairs[(straight.PinIdLeft(), Grating.PinIdLeft())];
            Assert.Equal(circuitLightVal, Complex.One);
        }

    }
}