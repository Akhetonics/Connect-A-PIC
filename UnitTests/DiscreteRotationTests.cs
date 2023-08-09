using ConnectAPIC.Scenes.Component;
using ConnectAPIC.Scenes.Tiles;

namespace UnitTests
{
    public class DiscreteRotationTests
    {
        [Fact]
        public void CalculateCyclesTillTargetTests()
        {

            DiscreteRotation rotation90 = DiscreteRotation.R90;
            DiscreteRotation rotation180 = DiscreteRotation.R180;
            
            int Cycles0 = rotation90.CalculateCyclesTillTargetRotation( DiscreteRotation.R90);
            int Cycles1 = rotation90.CalculateCyclesTillTargetRotation( DiscreteRotation.R180);
            int Cycles2 = rotation90.CalculateCyclesTillTargetRotation( DiscreteRotation.R270);
            int Cycles3 = DiscreteRotationExtensions.CalculateCyclesTillTargetRotation(DiscreteRotation.R0, DiscreteRotation.R270);
            int Cycles4 = DiscreteRotationExtensions.CalculateCyclesTillTargetRotation(DiscreteRotation.R270, DiscreteRotation.R0);
            Assert.True(Cycles0 == 0);
            Assert.True(Cycles1 == 1);
            Assert.True(Cycles2 == 2);
            Assert.True(Cycles3 == 3);
            Assert.True(Cycles4 == 1);
        }
        [Fact]
        public void Rotation90SideCorellationTests()
        {
            Assert.True((int)DiscreteRotation.R0 == (int)RectangleSide.Right);
            Assert.True((int)DiscreteRotation.R90 == (int)RectangleSide.Down, "Because both the rotation and Orientation should be clockwise as it is in the Godot Engine to be easily compatible");
            Assert.True((int)DiscreteRotation.R180 == (int)RectangleSide.Left);
            Assert.True((int)DiscreteRotation.R270 == (int)RectangleSide.Up);
        }
       
    }
}