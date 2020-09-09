using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Of_Life_LIB
{
    //RULES:
    //1. Any live cell with two or three live neighbours survives.
    //2. Any dead cell with three live neighbours becomes a live cell.
    //3. All other live cells die in the next generation.Similarly, all other dead cells stay dead.
    //ref: https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life

    public static class Cell
    {
        public static void UpdateState(int x, int y, int[,] grid, int[,] nextGenerationGrid)
        {
            int livingNeighbourCount = 0;

            //(x-1, y-1)    (x-1, y)    (x-1, y+1)
            //(x,   y-1)    (X,   y)    (x,   y+1)
            //(x+1, y-1)    (x+1, y)    (x+1, y+1)

            livingNeighbourCount += GetCellState(x - 1, y - 1, grid);
            livingNeighbourCount += GetCellState(x - 1, y, grid);
            livingNeighbourCount += GetCellState(x - 1, y + 1, grid);
            livingNeighbourCount += GetCellState(x, y - 1, grid);
            livingNeighbourCount += GetCellState(x, y + 1, grid);
            livingNeighbourCount += GetCellState(x + 1, y - 1, grid);
            livingNeighbourCount += GetCellState(x + 1, y, grid);
            livingNeighbourCount += GetCellState(x + 1, y + 1, grid);


            int thisCellState = grid[x, y];

            //LIFE RULES

            if (thisCellState == 1)
            {
                //Alive

                if (livingNeighbourCount == 2 || livingNeighbourCount == 3) return; //Condition 1
            }

            if (thisCellState == 0)
            {
                //Dead

                if (livingNeighbourCount == 3)
                {
                    nextGenerationGrid[x, y] = 1; //Condition 2

                    return; 
                }
            }

            nextGenerationGrid[x, y] = 0; //Condition 3
        }


        private static int GetCellState(int x, int y, int[,] grid)
        {
            int state = 0;

            //HACK: TryGet approach is lazy, because we are basically swallowing the exceptions when the references fall off the edge of the grid
            //TODO: Implement this to check the x, y against the boundaries of the grid, and don't proceed if they are off the edge
            TryGetCellState(x, y, grid, out state);

            return state;
        }

        private static bool TryGetCellState(int x, int y, int[,] grid, out int state)
        {
            state = 0;

            try
            {
                state = grid[x, y];

                return true;
            }
            catch (Exception)
            {
                //Ignore
            }

            return false;
        }
    }
}
