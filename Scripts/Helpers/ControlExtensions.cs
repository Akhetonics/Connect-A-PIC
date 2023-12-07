using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.Scripts.Helpers
{
    public static class ControlExtensions
    {
        public static Vector2 GetBiggestSize(this Control control)
        { 
            float largestWidth = control.Size.X;
            float largestHeight = control.Size.Y;

            foreach (Node child in control.GetChildren())
            {
                if (child is Control controlChild)
                {
                    Vector2 childSize = controlChild.Size;
                    largestWidth = Math.Max(largestWidth, childSize.X);
                    largestHeight = Math.Max(largestHeight, childSize.Y);
                }
            }
            return new Vector2(largestWidth, largestHeight);
        }
    }
}
