using System;
using System.Text;

namespace GameOfLife.Library
{
    public static class Helper
    {
        public static int[,] StringToIntMatrix(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentException("str is null or empty");


            int width = 0;
            int height = 0;

            var strSplit = str.Split('\n');

            height = strSplit.Length;

            foreach (string s in strSplit)
            {
                if (!string.IsNullOrWhiteSpace(s))
                {
                    if (s.Length > width)
                    {
                        width = s.Length;
                    }
                }
            }


            int[,] grid = new int[width, height];


            int x = 0;
            int y = 0;

            foreach (string s in strSplit)
            {
                if (!string.IsNullOrWhiteSpace(s))
                {
                    foreach (char c in s)
                    {
                        if (char.IsDigit(c))
                            grid[x, y] = c - '0';

                        x++;
                    }
                }

                x = 0;
                y++;
            }


            return grid;
        }

        public static string IntMatrixToString(int[,] grid)
        {
            int w = grid.GetLength(0);
            int h = grid.GetLength(1);

            var sb = new StringBuilder();

            for (int y = 0; y < h; y++)
            {
                //Add a newline character if we are on the second row or greater
                if (y > 0)
                    sb.Append("\n");

                for (int x = 0; x < w; x++)
                {
                    sb.Append(grid[x, y].ToString());
                }
            }

            return sb.ToString();
        }
    }
}
