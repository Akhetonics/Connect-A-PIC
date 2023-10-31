namespace CAP_Core.Helpers
{
    public static class RotationHelper
    {
        public static float NormalizeTo360(float angle)
        {
            angle = angle % 360;
            if (angle < 0)
            {
                angle += 360;
            }
            return angle;
        }
        public static float ToClockwise(float counterClockwiseAngel)
        {
            return NormalizeTo360(360 - counterClockwiseAngel);
        }
    }
}
