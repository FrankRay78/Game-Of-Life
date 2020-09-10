using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Of_Life_LIB
{
    public class Life
    {
        private readonly int width;
        private readonly int height;

        private int[,] grid;

        private string display_alive;
        private string display_dead;


        public Life(int width, int height, string alive = "1", string dead = " ")
        {
            this.width = width;
            this.height = height;

            grid = new int[width, height];

            display_alive = alive;
            display_dead = dead;
        }

        public Life(string startingPattern, string alive = "1", string dead = " ")
        {
            grid = Helper.StringToIntMatrix(startingPattern);

            this.width = grid.GetLength(0);
            this.height = grid.GetLength(1);

            display_alive = alive;
            display_dead = dead;
        }


        public void ApplyPattern(string pattern, int startX, int startY)
        {
            var patternGrid = Helper.StringToIntMatrix(pattern);

            int patternWidth = patternGrid.GetLength(0);
            int patternHeight = patternGrid.GetLength(1);

            for (int y = 0; y < patternHeight; y++)
            {
                for (int x = 0; x < patternWidth; x++)
                {
                    grid[startX + x, startY + y] = patternGrid[x, y];

                }
            }
        }

        public void UpdateState()
        {
            int[,] nextGenerationGrid = (int[,])grid.Clone();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Cell.UpdateState(x, y, grid, nextGenerationGrid);
                }
            }

            grid = nextGenerationGrid;
        }

        public override string ToString()
        {
            return Helper.IntMatrixToString(grid);
        }

        public string ToDisplayString()
        {
            var output = Helper.IntMatrixToString(grid);

            output = output.Replace("1", display_alive).Replace("0", display_dead);

            return output;
        }
    }
}
