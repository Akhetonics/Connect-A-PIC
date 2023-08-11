using ConnectAPIC.Scenes.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace ConnectAPIC.LayoutWindow.Model.Helpers
{
    public class IntVector
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public IntVector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static IntVector Right { get { return new IntVector(1, 0); } }
        public static IntVector Left { get { return new IntVector(-1, 0); } }
        public static IntVector Up { get { return new IntVector(0, -1); } }
        public static IntVector Down { get { return new IntVector(0, 1); } }

        public static IntVector operator +(IntVector vector1, IntVector vector2)
        {
            return new IntVector(vector1.X + vector2.X, vector1.Y + vector2.Y);
        }
        public static IntVector operator *(int scalar, IntVector vector)
        {
            return new IntVector(scalar * vector.X, scalar * vector.Y);
        }
        public static IntVector operator *(IntVector vector, int scalar)
        {
            return scalar * vector;
        }
        public static implicit operator IntVector(RectangleSide edgeSide)
        {
            if (edgeSide == RectangleSide.Right) return IntVector.Right;
            if (edgeSide == RectangleSide.Down) return IntVector.Down;
            if (edgeSide == RectangleSide.Left) return IntVector.Left;
            return IntVector.Up;
        }
        public static implicit operator RectangleSide(IntVector vector)
        {
            if (vector.X == IntVector.Right.X && vector.Y == IntVector.Right.Y)
                return RectangleSide.Right;
            if (vector.X == IntVector.Down.X && vector.Y == IntVector.Down.Y)
                return RectangleSide.Down;
            if (vector.X == IntVector.Left.X && vector.Y == IntVector.Left.Y)
                return RectangleSide.Left;
            if(vector.X == IntVector.Up.X && vector.Y == IntVector.Up.Y) 
                return RectangleSide.Up;
            throw new ArgumentException("vector does not match any rectangleSide");
        }



    }

}
