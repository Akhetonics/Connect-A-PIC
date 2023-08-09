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
            Assert.Equal(RectangleSide.Right, RectangleSide.Right.RotateSideCounterClockwise( DiscreteRotation.R0));
            Assert.Equal(RectangleSide.Up, RectangleSide.Right.RotateSideCounterClockwise( DiscreteRotation.R90));
            Assert.Equal(RectangleSide.Left, RectangleSide.Right.RotateSideCounterClockwise( DiscreteRotation.R180));
            Assert.Equal(RectangleSide.Down, RectangleSide.Right.RotateSideCounterClockwise( DiscreteRotation.R270));
            Assert.Equal(RectangleSide.Down, RectangleSide.Down.RotateSideCounterClockwise( DiscreteRotation.R0)); 
            Assert.Equal(RectangleSide.Right, RectangleSide.Down.RotateSideCounterClockwise( DiscreteRotation.R90)); 
            Assert.Equal(RectangleSide.Up, RectangleSide.Down.RotateSideCounterClockwise( DiscreteRotation.R180)); 
            Assert.Equal(RectangleSide.Left, RectangleSide.Down.RotateSideCounterClockwise( DiscreteRotation.R270)); 
            Assert.Equal(RectangleSide.Left, RectangleSide.Left.RotateSideCounterClockwise( DiscreteRotation.R0)); 
            Assert.Equal(RectangleSide.Down, RectangleSide.Left.RotateSideCounterClockwise( DiscreteRotation.R90));
            Assert.Equal(RectangleSide.Right, RectangleSide.Left.RotateSideCounterClockwise( DiscreteRotation.R180));
            Assert.Equal(RectangleSide.Up, RectangleSide.Left.RotateSideCounterClockwise( DiscreteRotation.R270)); 
            Assert.Equal(RectangleSide.Up, RectangleSide.Up.RotateSideCounterClockwise( DiscreteRotation.R0)); 
            Assert.Equal(RectangleSide.Left, RectangleSide.Up.RotateSideCounterClockwise( DiscreteRotation.R90)); 
            Assert.Equal(RectangleSide.Down, RectangleSide.Up.RotateSideCounterClockwise( DiscreteRotation.R180)); 
            Assert.Equal(RectangleSide.Right, RectangleSide.Up.RotateSideCounterClockwise( DiscreteRotation.R270)); 
        }
       
    }
}