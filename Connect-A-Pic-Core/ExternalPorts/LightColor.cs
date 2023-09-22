using System.Drawing;

namespace CAP_Core.ExternalPorts
{
    public enum LightColor
    {
        Red,
        Green,
        Blue
    }
    public static class LightColorExtensions
    {
        public static Color ToColor(this LightColor lc)
        {
            switch (lc)
            {
                case LightColor.Red:
                    return Color.Red;
                case LightColor.Green:
                    return Color.Green;
                case LightColor.Blue:
                    return Color.Blue;
                default:
                    throw new InvalidOperationException("Unknown LightColor-value");
            }
        }
    }
}