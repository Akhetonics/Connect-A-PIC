using CAP_Core.Components;
using CAP_Core.Tiles;

namespace CAP_Core.Helpers
{
    public static class DiscreteRotationExtensions
    {
        public static int CalculateCyclesTillTargetRotation(this DiscreteRotation currentRotation, DiscreteRotation targetRotation)
        {
            int maxCycles = ((int)DiscreteRotation.R270 + 1);
            int rotationIntervals = ((int)targetRotation - (int)currentRotation) % maxCycles;
            while (rotationIntervals < 0) rotationIntervals += maxCycles;
            return rotationIntervals;
        }
        public static DiscreteRotation RotateBy90CounterC(this DiscreteRotation currentRotation)
        {
            while (currentRotation < 0) currentRotation += (int)DiscreteRotation.R270 + 1;
            return (DiscreteRotation)((int)(currentRotation + 1) % (int)(DiscreteRotation.R270 + 1));
        }

        public static float ToDegreesClockwise (this DiscreteRotation currentRotation)
        {
            return (360-(float)currentRotation * 90f) % 360f;
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