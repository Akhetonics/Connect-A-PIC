using CAP_Core.Component.ComponentHelpers;
using CAP_Core.Tiles;

namespace CAP_Core.Helpers
{
    public static class DiscreteRotationExtensions
    {
        public static int CalculateCyclesTillTargetRotation(this DiscreteRotation currentRotation, DiscreteRotation targetRotation)
        {
            int rotationIntervals = ((int)targetRotation - (int)currentRotation) % ((int)DiscreteRotation.R270 + 1);
            while (rotationIntervals < 0) rotationIntervals += (int)DiscreteRotation.R270 + 1;
            return rotationIntervals;
        }
        public static DiscreteRotation RotateBy90(this DiscreteRotation currentRotation)
        {
            while (currentRotation < 0) currentRotation += (int)DiscreteRotation.R270 + 1;
            return (DiscreteRotation)((int)(currentRotation + 1) % (int)(DiscreteRotation.R270 + 1));
        }

        public static RectSide RotateSideCounterClockwise(this RectSide side, DiscreteRotation rotation)
        {
            int sideCount = Enum.GetValues(typeof(RectSide)).Length;
            int rotationIndex = (int)rotation;
            int currentSide = (int)side;

            int newSide = (sideCount - rotationIndex) % sideCount;
            newSide = (newSide + currentSide) % sideCount;

            return (RectSide)newSide;
        }
        public static RectSide RotateSideClockwise(this RectSide side, DiscreteRotation rotation)
        {
            int sideCount = Enum.GetValues(typeof(RectSide)).Length;
            int rotationIndex = (int)rotation;

            int newSide = (rotationIndex + (int)side) % sideCount;

            return (RectSide)newSide;
        }
    }
}