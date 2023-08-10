using ConnectAPIC.Scenes.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.LayoutWindow.Model.Helpers
{
    public static class RectangleSideHelpers 
    {
        public static RectangleSide GetOppositeDirection(this RectangleSide direction)
        {
            RectangleSide newDirection = RectangleSide.Left;
            if (direction == RectangleSide.Left)
            {
                newDirection = RectangleSide.Right;
            }
            if (direction == RectangleSide.Up)
            {
                newDirection = RectangleSide.Down;
            }
            if (direction == RectangleSide.Down)
            {
                newDirection = RectangleSide.Up;
            }
            return newDirection;
        }
    }
}
