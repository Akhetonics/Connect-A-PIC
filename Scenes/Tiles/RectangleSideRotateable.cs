using ConnectAPIC.Scenes.Component;

namespace ConnectAPIC.Scenes.Tiles
{
    public static class RectangleSideRotateable
    {
        public static RectangleSide Rotate(this RectangleSide side, DiscreteRotation deg)
        {
            return (RectangleSide)((int)side + deg);
        }
        public static RectangleSide Right(DiscreteRotation deg = DiscreteRotation.R0)
        {
            return Rotate(RectangleSide.Right, deg);
        }
        public static RectangleSide Up(DiscreteRotation deg = DiscreteRotation.R0)
        {
            return Rotate(RectangleSide.Up, deg);
        }
        public static RectangleSide Left(DiscreteRotation deg = DiscreteRotation.R0)
        {
            return Rotate(RectangleSide.Left, deg);
        }
        public static RectangleSide Down(DiscreteRotation deg = DiscreteRotation.R0)
        {
            return Rotate(RectangleSide.Down, deg);
        }
        
    }
}
