using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Of_Life_LIB
{
    public class Cell
    {
        public int X { get; }
        public int Y { get; }

        private readonly int[,] grid;

        public Cell(int x, int y, int[,] grid)
        {
            X = x;
            Y = y;
            this.grid = grid;
        }

        public void UpdateState()
        {
            //TODO: implement Cell.UpdateState()
        }

        public override string ToString()
        {
            return grid[X, Y].ToString();
        }
    }
}
