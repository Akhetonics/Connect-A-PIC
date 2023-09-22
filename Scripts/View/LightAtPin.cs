using CAP_Core.ExternalPorts;
using CAP_Core.Tiles;
using System.Numerics;

namespace ConnectAPIC.LayoutWindow.View
{
    public record struct LightAtPin(int partOffsetX, int partOffsetY, RectSide side, LightColor color, Complex lightInFlow , Complex lightOutFlow)
    {
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
