using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.LayoutWindow.Model.Helpers
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
