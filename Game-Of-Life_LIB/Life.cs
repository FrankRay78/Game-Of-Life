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

        public string ToString(string alive, string dead)
        {
            var output = Helper.IntMatrixToString(grid);

            output = output.Replace("1", alive).Replace("0", dead);

            return output;
        }
    }
}
