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
                    return Color.FromArgb(0,255,0);
                case LightColor.Blue:
                    return Color.Blue;
                default:
                    throw new InvalidOperationException("Unknown LightColor-value");
            }
        }
        public static string ToReadableString(this LightColor color)
        {
            switch (color)
            {
                case LightColor.Red:
                    return "Red";
                case LightColor.Green:
                    return "Green";
                case LightColor.Blue:
                    return "Blue";
                default:
                    throw new ArgumentOutOfRangeException(nameof(color), color, null);
            }
        }
    }
}