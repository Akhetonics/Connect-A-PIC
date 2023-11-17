using CAP_Core;
using CAP_Core.Component;
using CAP_Core.Component.ComponentHelpers;
using CAP_Core.LightFlow;
using CAP_Core.Tiles;
using ConnectAPIC.LayoutWindow.ViewModel.Commands;
using Newtonsoft.Json;
using System.Numerics;

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
        public void TestComponentRotation()
        {
            var grid = new Grid(10, 10);
            var component = TestComponentFactory.CreateDirectionalCoupler();
            grid.PlaceComponent(0, 0, component);
            var command = new RotateComponentCommand(grid);
            var args = new RotateComponentArgs(0, 0);
            var PinLeft = component.GetPartAt(0,0).GetPinAt(RectSide.Left);
            var PinRight = component.GetPartAt(1,0).GetPinAt(RectSide.Right);
            var PinDownLeft = component.GetPartAt(0,1).GetPinAt(RectSide.Left);
            var PinDownRight= component.GetPartAt(1,1).GetPinAt(RectSide.Right);
            var PartLeft = component.GetPartAt(0, 0);
            var PartRight = component.GetPartAt(1, 0);
            var PartDownLeft = component.GetPartAt(0, 1);
            var PartDownRight = component.GetPartAt(1, 1);
            
            command.Execute(args);
            Assert.Equal(2, component.WidthInTiles);
            Assert.Equal(2, component.HeightInTiles);
            Assert.Equal(DiscreteRotation.R90, component.Rotation90CounterClock);
            // check Parts
            Assert.Equal(PartRight,component.GetPartAt(0, 0));
            Assert.Equal(PartLeft,component.GetPartAt(0, 1));
            Assert.Equal(PartDownLeft,component.GetPartAt(1, 1));
            Assert.Equal(PartDownRight,component.GetPartAt(1, 0));
            // check Pins
            Assert.Equal(component.GetPartAt(0, 1).GetPinAt(RectSide.Down), PinLeft);
            Assert.Equal(component.GetPartAt(0, 0).GetPinAt(RectSide.Up), PinRight);
            Assert.Equal(component.GetPartAt(1, 1).GetPinAt(RectSide.Down), PinDownLeft);
            Assert.Equal(component.GetPartAt(1, 0).GetPinAt(RectSide.Up), PinDownRight);
            command.Execute(args);
            Assert.Equal(2, component.WidthInTiles);
            Assert.Equal(2, component.HeightInTiles);
            Assert.Equal(DiscreteRotation.R180, component.Rotation90CounterClock);
            command.Execute(args);
            Assert.Equal(2, component.WidthInTiles);
            Assert.Equal(2, component.HeightInTiles);
            Assert.Equal(DiscreteRotation.R270, component.Rotation90CounterClock);
            command.Execute(args);
            Assert.Equal(2, component.WidthInTiles);
            Assert.Equal(2, component.HeightInTiles);
            Assert.Equal(DiscreteRotation.R0, component.Rotation90CounterClock);
        }
    }
}
