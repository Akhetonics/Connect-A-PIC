using CAP_Core.ExternalPorts;
using CAP_Core.Tiles;
using System.Numerics;

namespace ConnectAPIC.LayoutWindow.View
{
    public record struct LightAtPin(int partOffsetX, int partOffsetY, RectSide side, LaserType lightType, Complex lightFieldInFlow , Complex lightFieldOutFlow)
    {
        public override string ToString()
        {
            string shortSide = side.ToString()[..3]; // Assuming the enum names have at least 1 character
            return $"x: {partOffsetX}, y: {partOffsetY},{shortSide}, color: {lightType.Color.ToReadableString()}, in: {lightFieldInFlow}, out: {lightFieldOutFlow}";
        }

        public static implicit operator (int partOffsetX, int partOffsetY, RectSide side, LaserType lightType, Complex lightFieldInFlow, Complex lightFieldOutFlow) (LightAtPin value)
        {
            return (value.partOffsetX, value.partOffsetY, value.side, value.lightType, value.lightFieldInFlow, value.lightFieldOutFlow);
        }

        public static implicit operator LightAtPin((int partOffsetX, int partOffsetY, RectSide side, LaserType lightType, Complex lightFieldInFlow, Complex lightFieldOutFlow) value)
        {
            return new LightAtPin(value.partOffsetX, value.partOffsetY, value.side, value.lightType, value.lightFieldInFlow, value.lightFieldOutFlow);
        }
    }
}
