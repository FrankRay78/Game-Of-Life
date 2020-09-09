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
        private List<Cell> cells;

        public Life(int width, int height)
        {
            this.width = width;
            this.height = height;

            grid = new int[width, height];
            cells = new List<Cell>();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //Initialise a given grid coordinate and create a respective cell for it too

                    grid[x, y] = 0; //Default the cell to dead

                    var c = new Cell(x, y, grid);

                    cells.Add(c);
                }
            }
        }

        public void UpdateState()
        {
            foreach (Cell c in cells)
            {
                c.UpdateState();
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                }
            }

            return sb.ToString();
        }
    }
}
