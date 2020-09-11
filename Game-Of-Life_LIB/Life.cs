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

        private int generation = 0;
        private int[,] grid;

        public int Generation { get => generation; }
        public int[,] Grid { get => grid; }

        public Life(int width, int height)
        {
            this.width = width;
            this.height = height;

            grid = new int[width, height];
        }

        public Life(string startingPattern)
        {
            grid = Helper.StringToIntMatrix(startingPattern);

            this.width = grid.GetLength(0);
            this.height = grid.GetLength(1);
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

            generation++;
        }

        public override string ToString()
        {
            return Helper.IntMatrixToString(grid);
        }
    }
}
