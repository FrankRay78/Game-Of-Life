using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Game_Of_Life_LIB;

namespace Game_Of_Life
{
    /// <summary>
    /// Console implementation of Conway's Game of Life (aka 80's retro style)
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life"/>
    class Program
    {
        public const string ALIVE_CELL_PRINT_CHARACTERS = "\u25A0"; //Unicode box character
        public const string DEAD_CELL_PRINT_CHARACTERS = "-";

        const int GRID_WIDTH = 50;
        const int GRID_HEIGHT = 50;

        const int WAIT = 5; //time between each tick, in ms

        const int Y_OFFSET_WHEN_RENDERING = 2;


        static void Main(string[] args)
        {
            var life = new Life(GRID_WIDTH, GRID_HEIGHT);


            //Apply patterns

            //life.ApplyPattern(Patterns.Still_Life_Block, 5, 5);
            //life.ApplyPattern(Patterns.Oscillator_Blinker, 10, 10);

            //life.ApplyPattern(Patterns.Acorn, 5, 5);

            life.ApplyPattern(Patterns.R_Pentomino, 25, 25);


            //Start the game

            bool keepGoing = true;

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.CursorVisible = false;

            if (Console.WindowWidth < GRID_WIDTH + 5) Console.WindowWidth = GRID_WIDTH + 5;
            if (Console.WindowHeight < GRID_HEIGHT + 5) Console.WindowHeight = GRID_HEIGHT + 5;


            //Draw the starting life and pause until the user signals to start
            Console.SetCursorPosition(0, 2);
            RenderGridToConsole(life.Grid);
            Console.ReadLine();

            do
            {
                //Cache the current generation before updating (to be used later in the delta only rendering)
                int[,] previousGrid = (int[,])life.Grid.Clone();

                life.UpdateState();

                Console.SetCursorPosition(0, 0);
                Console.Write(life.Generation);

                //RenderGridToConsole(life.Grid);
                RenderGridToConsoleDeltasOnly(previousGrid, life.Grid);

                Thread.Sleep(WAIT);

                if (Console.KeyAvailable)
                {
                    //break out of the while loop and terminate the console
                    keepGoing = false;
                }
            } while (keepGoing);
        }

        #region Grid Rendering Routines

        /// <summary>
        /// Splat the full canvas on to the console, each and every time, irrespective of if anything has changed
        /// TODO: consider writing only changes to a buffer and pushing this instead
        /// </summary>
        private static void RenderGridToConsole(int[,] grid)
        {
            var output = Helper.IntMatrixToString(grid);

            output = output.Replace("1", ALIVE_CELL_PRINT_CHARACTERS).Replace("0", DEAD_CELL_PRINT_CHARACTERS);

            Console.SetCursorPosition(0, Y_OFFSET_WHEN_RENDERING);

            Console.Write(output);
        }

        //TODO: https://stackoverflow.com/questions/29920056/c-sharp-something-faster-than-console-write

        /// <summary>
        /// Writing to the console each new generation (irrespective of what has changed) is slow and CPU intensive.
        /// eg: Console.Write(life.ToDisplayString());
        /// 
        /// I'd like to be able to render the deltas so I can effectively speed up the cycle time
        /// The below method aims to do this
        /// </summary>
        private static void RenderGridToConsoleDeltasOnly(int[,] previousGrid, int[,] nextGrid)
        {
            int width = previousGrid.GetLength(0);
            int height = previousGrid.GetLength(1);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int currentCellValue = previousGrid[x, y];
                    int nextCellValue = nextGrid[x, y];

                    if (currentCellValue != nextCellValue)
                    {
                        //Cell value has changed between generations

                        Console.SetCursorPosition(x, Y_OFFSET_WHEN_RENDERING + y);

                        Console.Write(nextCellValue.ToString().Replace("1", ALIVE_CELL_PRINT_CHARACTERS).Replace("0", DEAD_CELL_PRINT_CHARACTERS));
                    }
                }
            }
        }

        #endregion
    }
}
