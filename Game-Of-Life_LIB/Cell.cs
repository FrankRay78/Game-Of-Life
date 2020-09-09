using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Of_Life_LIB
{
    public class Cell
    {
        public const string ALIVE_PRINT_CHARACTERS = "\u25A0";
        public const string DEAD_PRINT_CHARACTERS = " ";

        public int X { get; }
        public int Y { get; }

        private readonly int[,] grid;

        /// <summary>
        ///  Use the underlying grid member variable as the backing store for this cell's state
        /// </summary>
        public int Alive
        {
            get
            {
                return grid[X, Y];
            }
            set
            {
                grid[X, Y] = value;
            }
        }

        public Cell(int x, int y, int[,] grid)
        {
            X = x;
            Y = y;
            this.grid = grid;
        }

        public void UpdateState()
        {
            //TODO
        }

        public override string ToString()
        {
            return (Alive == 1 ? ALIVE_PRINT_CHARACTERS : DEAD_PRINT_CHARACTERS);
        }
    }
}
