using CAP_Core.Components.ComponentHelpers;

namespace CAP_Core.Helpers
{
    public static class ArrayExtensions
    {

        public static T[,] RotateClockwise<T>(this T[,] array)
        {
            int rows = array.GetLength(0);
            int columns = array.GetLength(1);
            T[,] rotatedArray = new T[columns, rows];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    rotatedArray[columns - 1 - j, i] = array[i, j];
                }
            }

            return rotatedArray;
        }
        public static T[,] RotateCounterClockwise<T>(this T[,] array)
        {
            int rows = array.GetLength(0);
            int columns = array.GetLength(1);
            T[,] rotatedArray = new T[columns, rows];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    rotatedArray[j, rows - 1 - i] = array[i, j];
                }
            }

            return rotatedArray;
        }

    }
}