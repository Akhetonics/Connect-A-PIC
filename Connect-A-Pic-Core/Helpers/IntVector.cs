
using CAP_Contracts;
using CAP_Core.Tiles;

namespace CAP_Core.Helpers
{
    public class IntVector : IEquatable<IntVector>
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
        public static implicit operator IntVector(RectSide edgeSide)
        {
            if (edgeSide == RectSide.Right) return Right;
            if (edgeSide == RectSide.Down) return Down;
            if (edgeSide == RectSide.Left) return Left;
            return Up;
        }
        public static implicit operator RectSide(IntVector vector)
        {
            if (vector.X == Right.X && vector.Y == Right.Y)
                return RectSide.Right;
            if (vector.X == Down.X && vector.Y == Down.Y)
                return RectSide.Down;
            if (vector.X == Left.X && vector.Y == Left.Y)
                return RectSide.Left;
            if (vector.X == Up.X && vector.Y == Up.Y)
                return RectSide.Up;
            throw new ArgumentException("vector does not match any rectangleSide");
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as IntVector);
        }

        public bool Equals(IntVector? other)
        {
            return other != null && X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }


    }

}
