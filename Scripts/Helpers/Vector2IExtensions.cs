using CAP_Core.Helpers;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.Scripts.Helpers
{
    public static class Vector2IExtensions
    {
        public static IntVector ToIntVector(this Vector2I vector)
        {
            return new IntVector(vector.X, vector.Y);
        }
    }
}
