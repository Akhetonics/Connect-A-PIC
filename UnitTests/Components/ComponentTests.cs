using CAP_Core;
using CAP_Core.Components;
using CAP_Core.Components.ComponentHelpers;
using CAP_Core.ExternalPorts;
using CAP_Core.Grid;
using CAP_Core.Tiles;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using Newtonsoft.Json;
using UnitTests.Grid;

namespace UnitTests
{
    public class ComponentTests
    {

        [Fact]
        public void TestDirectionalCouplerSerialization()
        {
            var coupler = TestComponentFactory.CreateDirectionalCoupler();
            var couplerText = JsonConvert.SerializeObject(coupler);
        }
        [Fact]
        public async Task TestComponentRotation()
        {
            var grid = GridHelpers.InitializeGridWithComponents(10, 10);
            var component = TestComponentFactory.CreateDirectionalCoupler();
            var portY = grid.ExternalPortManager.ExternalPorts.First().TilePositionY;
            grid.ComponentMover.PlaceComponent(0, portY, component);
            var PinLeft = component.GetPartAt(0, 0).GetPinAt(RectSide.Left);
            var PinRight = component.GetPartAt(1, 0).GetPinAt(RectSide.Right);
            var PinDownLeft = component.GetPartAt(0, 1).GetPinAt(RectSide.Left);
            var PinDownRight = component.GetPartAt(1, 1).GetPinAt(RectSide.Right);
            var PartLeft = component.GetPartAt(0, 0);
            var PartRight = component.GetPartAt(1, 0);
            var PartDownLeft = component.GetPartAt(0, 1);
            var PartDownRight = component.GetPartAt(1, 1);
            var laserType = grid.ExternalPortManager.GetUsedExternalInputs().First().Input.LaserType;
            // test if I can get the non-Null values
            component.WaveLengthToSMatrixMap[StandardWaveLengths.RedNM].GetNonNullValues();
            var command = new RotateComponentCommand(grid);
            var args = new RotateComponentArgs(component.GridXMainTile, component.GridYMainTile);
            await command.ExecuteAsync(args);
            Assert.Equal(2, component.WidthInTiles);
            Assert.Equal(2, component.HeightInTiles);
            Assert.Equal(DiscreteRotation.R90, component.Rotation90CounterClock);
            // check Parts
            Assert.Equal(PartRight, component.GetPartAt(0, 0));
            Assert.Equal(PartLeft, component.GetPartAt(0, 1));
            Assert.Equal(PartDownLeft, component.GetPartAt(1, 1));
            Assert.Equal(PartDownRight, component.GetPartAt(1, 0));
            // check Pins to be rotated by 90 counterclockwise
            Assert.Equal(component.GetPartAt(0, 1).GetPinAt(RectSide.Down), PinLeft);
            Assert.Equal(component.GetPartAt(0, 0).GetPinAt(RectSide.Up), PinRight);
            Assert.Equal(component.GetPartAt(1, 1).GetPinAt(RectSide.Down), PinDownLeft);
            Assert.Equal(component.GetPartAt(1, 0).GetPinAt(RectSide.Up), PinDownRight);
            await command.ExecuteAsync(args);
            Assert.Equal(2, component.WidthInTiles);
            Assert.Equal(2, component.HeightInTiles);
            Assert.Equal(DiscreteRotation.R180, component.Rotation90CounterClock);
            await command.ExecuteAsync(args);
            Assert.Equal(2, component.WidthInTiles);
            Assert.Equal(2, component.HeightInTiles);
            Assert.Equal(DiscreteRotation.R270, component.Rotation90CounterClock);
            await command.ExecuteAsync(args);
            Assert.Equal(2, component.WidthInTiles);
            Assert.Equal(2, component.HeightInTiles);
            Assert.Equal(DiscreteRotation.R0, component.Rotation90CounterClock);
        }
        [Fact]
        public void TestComponentPinIDsInConnectionMatrixAfterCloning()
        {
            var grid = GridHelpers.InitializeGridWithComponents(10,10);
            var componentOld = TestComponentFactory.CreateDirectionalCoupler();
            // we also test the clone function here - it should be able to properly clone the object
            var component = componentOld.Clone() as Component;
            var inputLaserY = grid.ExternalPortManager.ExternalPorts[0].TilePositionY;
            grid.ComponentMover.PlaceComponent(0, inputLaserY, component);
            var inputLaserType = grid.ExternalPortManager.GetUsedExternalInputs().First().Input.LaserType;

            // after cloning the connections should have the new PinIDs
            var connections = component.WaveLengthToSMatrixMap[inputLaserType.WaveLengthInNm].GetNonNullValues();
            // for debug reasons
            var oldIDs = componentOld.WaveLengthToSMatrixMap[inputLaserType.WaveLengthInNm].GetNonNullValues().Keys;
            var newIDs = component.WaveLengthToSMatrixMap[inputLaserType.WaveLengthInNm].GetNonNullValues().Keys;
            var allNewIDs = Component.GetAllPins(component.Parts).SelectMany(p=>new[] { p.IDInFlow, p.IDOutFlow });
            var allOldIDs = Component.GetAllPins(componentOld.Parts).SelectMany(p=>new[] { p.IDInFlow, p.IDOutFlow });

            // every light based Pin of the whole component that is defined should exist in the connection-S-Matrix.
            foreach (var part in component.Parts)
            {
                if (part == null) continue;
                foreach (var pin in part.Pins)
                {
                    if (pin.MatterType != MatterType.Light) continue;
                    var isPinInFlowFromPresent = connections.Any(conn => conn.Key.PinIdStart == pin.IDInFlow);
                    var isPinInFlowToPresent = connections.Any(conn => conn.Key.PinIdEnd == pin.IDInFlow);
                    var isPinOutFlowFromPresent = connections.Any(conn => conn.Key.PinIdStart == pin.IDOutFlow);
                    var isPinOutFlowToPresent = connections.Any(conn => conn.Key.PinIdEnd == pin.IDOutFlow);

                    Assert.True(isPinInFlowFromPresent || isPinInFlowToPresent, $"PinID {pin.IDInFlow} not found in connection matrix.");
                    Assert.True(isPinOutFlowFromPresent || isPinOutFlowToPresent, $"PinID {pin.IDOutFlow} not found in connection matrix.");
                }
            }

            // also every PinID in the SMatrix should be existing in the component
            foreach(var connection in connections)
            {
                var PinStart = connection.Key.PinIdStart;
                var PinEnd = connection.Key.PinIdEnd;
                var allComponentPins = component.GetAllPins();
                var isConnPinStartPresent = allComponentPins.Any(p => p.IDInFlow == PinStart) || allComponentPins.Any(p => p.IDOutFlow == PinStart);
                var isConnPinEndPresent = allComponentPins.Any(p => p.IDInFlow == PinEnd) || allComponentPins.Any(p => p.IDOutFlow == PinEnd);
                Assert.True(isConnPinStartPresent, $"PinID {PinStart} not found on component.");
                Assert.True(isConnPinEndPresent, $"PinID {PinEnd} not found on component.");
            }
        }

        [Fact]
        public void TestSecondComponentHasDifferentPinIDs()
        {
            var grid = new GridManager(10,10);
            var componentOld = TestComponentFactory.CreateDirectionalCoupler();
            var component = componentOld.Clone() as Component;
            var component2 = componentOld.Clone() as Component;
            var componentY = grid.ExternalPortManager.ExternalPorts.First().TilePositionY;
            grid.ComponentMover.PlaceComponent(0, componentY, component);
            grid.ComponentMover.PlaceComponent(2, componentY, component2);

            var allComponentPins = component.GetAllPins();
            var allComponent2Pins = component2.GetAllPins();
            var isIDConflict = allComponentPins.Any(p => allComponent2Pins.Any(p2 => p2.IDInFlow == p.IDInFlow))
                || allComponentPins.Any(p => allComponent2Pins.Any(p2 => p2.IDOutFlow == p.IDOutFlow))
                || allComponentPins.Any(p => allComponent2Pins.Any(p2 => p2.IDInFlow == p.IDOutFlow))
                || allComponentPins.Any(p => allComponent2Pins.Any(p2 => p2.IDOutFlow == p.IDInFlow));
            
            // make the guids all readable
            var allComponentGuids = allComponentPins.Select(p => p.IDInFlow.ToString()[..3]).ToList();
            allComponentGuids.AddRange(allComponentPins.Select(p => p.IDOutFlow.ToString()[..3]).ToList());
            var allComponent2Guids = allComponent2Pins.Select(p => p.IDInFlow.ToString()[..3]).ToList();
            allComponent2Guids.AddRange(allComponent2Pins.Select(p => p.IDOutFlow.ToString()[..3]).ToList());
            string guidsReadable = String.Join(',', allComponentGuids);
            string guids2Readable = String.Join(',', allComponent2Guids);

            Assert.False(isIDConflict, "there some IDs seem to overlap. Here is the first ID \n" + guidsReadable + " 1st of second comp: \n" + guids2Readable);
        }
    }

}
