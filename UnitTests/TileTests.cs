using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;
using Tiles;

namespace UnitTests
{
    public class TileTests
    {
        [Fact]
        public void CalculateSideShiftingTests()
        {
            Assert.Equal(RectSide.Right, RectSide.Right.RotateSideCounterClockwise( DiscreteRotation.R0));
            Assert.Equal(RectSide.Up, RectSide.Right.RotateSideCounterClockwise( DiscreteRotation.R90));
            Assert.Equal(RectSide.Left, RectSide.Right.RotateSideCounterClockwise( DiscreteRotation.R180));
            Assert.Equal(RectSide.Down, RectSide.Right.RotateSideCounterClockwise( DiscreteRotation.R270));
            Assert.Equal(RectSide.Down, RectSide.Down.RotateSideCounterClockwise( DiscreteRotation.R0)); 
            Assert.Equal(RectSide.Right, RectSide.Down.RotateSideCounterClockwise( DiscreteRotation.R90)); 
            Assert.Equal(RectSide.Up, RectSide.Down.RotateSideCounterClockwise( DiscreteRotation.R180)); 
            Assert.Equal(RectSide.Left, RectSide.Down.RotateSideCounterClockwise( DiscreteRotation.R270)); 
            Assert.Equal(RectSide.Left, RectSide.Left.RotateSideCounterClockwise( DiscreteRotation.R0)); 
            Assert.Equal(RectSide.Down, RectSide.Left.RotateSideCounterClockwise( DiscreteRotation.R90));
            Assert.Equal(RectSide.Right, RectSide.Left.RotateSideCounterClockwise( DiscreteRotation.R180));
            Assert.Equal(RectSide.Up, RectSide.Left.RotateSideCounterClockwise( DiscreteRotation.R270)); 
            Assert.Equal(RectSide.Up, RectSide.Up.RotateSideCounterClockwise( DiscreteRotation.R0)); 
            Assert.Equal(RectSide.Left, RectSide.Up.RotateSideCounterClockwise( DiscreteRotation.R90)); 
            Assert.Equal(RectSide.Down, RectSide.Up.RotateSideCounterClockwise( DiscreteRotation.R180)); 
            Assert.Equal(RectSide.Right, RectSide.Up.RotateSideCounterClockwise( DiscreteRotation.R270)); 
        }
       
    }
}