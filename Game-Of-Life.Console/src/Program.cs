using System;
using System.Diagnostics;
using System.Threading;
using GameOfLife.Library;

namespace GameOfLife
{
    /// <summary>
    /// Console implementation of Conway's Game of Life (aka 80's retro style)
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life"/>
    class Program
    {
        public const string AliveCellPrintCharacters = "\u25A0"; //Unicode box character
        public const string DeadCellPrintCharacters = "-";

        const int GridWidth = 50;
        const int GridHeight = 25;

        const int Wait = 100; //time between each tick, in ms
        const int MaximumGenerations = 0;

        const int YOffsetWhenRendering = 2;
        const int WindowPadding = 5;


        static void Main(string[] args)
        {
            var life = new Life(GridWidth, GridHeight);


            //Apply patterns

            //life.ApplyPattern(Patterns.Still_Life_Block, 5, 5);
            //life.ApplyPattern(Patterns.Oscillator_Blinker, 10, 10);

            //life.ApplyPattern(Patterns.Acorn, 50, 25);

            life.ApplyPattern(Patterns.R_Pentomino, 25, 10);


            //Start the game

            bool keepGoing = true;

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.CursorVisible = false;


            //Try and expand the console width and height to accommodate the full grid
            if (Console.WindowWidth < GridWidth + WindowPadding)
            {
                if (GridWidth + WindowPadding < Console.LargestWindowWidth)
                    Console.WindowWidth = GridWidth + WindowPadding;
                else
                    Console.WindowWidth = Console.LargestWindowWidth;
            }
            if (Console.WindowHeight < GridHeight + WindowPadding)
            {
                if (GridHeight + WindowPadding < Console.LargestWindowHeight)
                    Console.WindowHeight = GridHeight + WindowPadding;
                else
                    Console.WindowHeight = Console.LargestWindowHeight;
            }


            //Draw the starting life and pause until the user signals to start
            Console.SetCursorPosition(0, 2);
            RenderGridToConsole(life.Grid);
            Console.ReadLine();


            //Diagnostics
            Stopwatch timer = new Stopwatch();
            timer.Start();


            do
            {
                //Cache the current generation before updating (to be used later in the delta only rendering)
                int[,] previousGrid = (int[,])life.Grid.Clone();

                life.UpdateState();

                Console.SetCursorPosition(0, 0);
                Console.Write(life.Generation);

                //RenderGridToConsole(life.Grid);
                RenderGridToConsoleDeltasOnly(previousGrid, life.Grid);

                Thread.Sleep(Wait);

                if (Console.KeyAvailable)
                {
                    //break out of the while loop and terminate the console
                    keepGoing = false;
                }

                if (MaximumGenerations > 0 && life.Generation >= MaximumGenerations)
                {
                    //break out of the while loop and terminate the console
                    keepGoing = false;
                }
            } while (keepGoing);


            //Diagnostics
            timer.Stop();
            var timeTaken = string.Format("{0} minutes, {1} seconds, {2} milliseconds", (int)timer.Elapsed.TotalMinutes, timer.Elapsed.Seconds, timer.Elapsed.Milliseconds);
            Debug.WriteLine("Iterations: {0}, Time taken: {1}", life.Generation, timeTaken);
        }

        #region Grid Rendering Routines

        /// <summary>
        /// Splat the full canvas on to the console, each and every time, irrespective of if anything has changed
        /// TODO: consider writing only changes to a buffer and pushing this instead
        /// </summary>
        private static void RenderGridToConsole(int[,] grid)
        {
            var output = Helper.IntMatrixToString(grid);

            output = output.Replace("1", AliveCellPrintCharacters).Replace("0", DeadCellPrintCharacters);

            Console.SetCursorPosition(0, YOffsetWhenRendering);

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
                        Console.SetCursorPosition(x, YOffsetWhenRendering + y);
                        Console.Write(CellValueToChar(nextCellValue));
                    }
                }
            }
        }

        /// <summary>
        /// Converts a cell value to its display character(s).
        /// </summary>
        private static string CellValueToChar(int cellValue)
        {
            return cellValue == 1 ? AliveCellPrintCharacters : DeadCellPrintCharacters;
        }

        #endregion
    }
}
