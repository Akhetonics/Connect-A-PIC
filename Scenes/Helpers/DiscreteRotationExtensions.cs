using ConnectAPIC.Scenes.Component;

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
}
