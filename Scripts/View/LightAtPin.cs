using CAP_Core.ExternalPorts;
using CAP_Core.Tiles;
using System.Numerics;

namespace ConnectAPIC.LayoutWindow.View
{
    public record struct LightAtPin(int partOffsetX, int partOffsetY, RectSide side, LightColor color, Complex lightInFlow , Complex lightOutFlow)
    {
        public override string ToString()
        {
            string shortSide = side.ToString().Substring(0, 1); // Assuming the enum names have at least 1 character
            return $"x: {partOffsetX}, y: {partOffsetY},{shortSide}, clr: {color.ToReadableString()}, in: {lightInFlow}, out: {lightOutFlow}";
        }

        public static implicit operator (int partOffsetX, int partOffsetY, RectSide side, LightColor color, Complex lightInFlow, Complex lightOutFlow) (LightAtPin value)
        {
            return (value.partOffsetX, value.partOffsetY, value.side, value.color, value.lightInFlow, value.lightOutFlow);
        }

        public static implicit operator LightAtPin((int partOffsetX, int partOffsetY, RectSide side, LightColor color, Complex lightInFlow, Complex lightOutFlow) value)
        {
            return new LightAtPin(value.partOffsetX, value.partOffsetY, value.side, value.color, value.lightInFlow, value.lightOutFlow);
        }
    }
}
