using CAP_Core.Components;
using CAP_Core.Components.ComponentHelpers;
using CAP_Core.ExternalPorts;
using CAP_Core.Grid;
using CAP_Core.LightCalculation;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTests.Grid;

namespace UnitTests.LightCalculation
{
    public class SystemMatrixBuilderTests
    {
        [Fact]
        public void SystemMatrixBuildingTest()
        {
            GridManager grid = GridHelpers.InitializeGridWithComponents(40, 40);
            var component = TestComponentFactory.CreateComponent(TestComponentFactory.MMI3x3);
            component.HeightInTiles.ShouldBe(3);

            var RedLaserYPos = grid.ExternalPortManager.GetAllExternalInputs().FirstOrDefault(i => i.LaserType == LaserType.Red).TilePositionY;
            grid.ComponentMover.PlaceComponent(0, RedLaserYPos, component);

            // create systemMatrix of this two components
            SystemMatrixBuilder builder = new SystemMatrixBuilder(grid);
            var systemMatrix = builder.GetSystemSMatrix(LaserType.Red.WaveLengthInNm);
            var allConnections = systemMatrix.GetNonNullValues();
            allConnections.Count.ShouldBe(18);
            var pinLeftUp = (Guid)component.PinIdLeftIn();
            var pinRightUp = (Guid)component.PinIdRightOut(2, 0);
            var expectedField = Math.Sqrt(1.0 / 3.0);
            allConnections[(pinLeftUp, pinRightUp)].Magnitude.ShouldBe(expectedField, 0.000000001);
        }
    }
}
