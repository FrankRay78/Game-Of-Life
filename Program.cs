using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Game_Of_Life_LIB;

namespace Game_Of_Life
{
    //https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life

    class Program
    {
        public const string ALIVE_CELL_PRINT_CHARACTERS = "\u25A0";
        public const string DEAD_CELL_PRINT_CHARACTERS = " ";

        const int GRID_WIDTH = 5;
        const int GRID_HEIGHT = 5;

        const int WAIT = 500; //time between each tick, in ms


        static void Main(string[] args)
        {
            var life = new Life(GRID_WIDTH, GRID_HEIGHT);


            bool keepGoing = true;

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.CursorVisible = false;


            //Draw the starting life and pause until the user signals to start
            Console.SetCursorPosition(0, 0);
            //TODO: Console.Write(life.ToString().Replace("1", ALIVE_CELL_PRINT_CHARACTERS).Replace("0", DEAD_CELL_PRINT_CHARACTERS));
            Console.Write(life.ToString());
            Console.ReadLine();

            do
            {
                life.UpdateState();


                //Splat an entirely updated cavas, each and every time, on to the console
                //TODO: consider writing only changes to a buffer and pushing this instead

                Console.SetCursorPosition(0, 0);

                Console.Write(life.ToString());

                Thread.Sleep(WAIT);

                if (Console.KeyAvailable)
                {
                    //break out of the while loop and terminate the console
                    keepGoing = false;
                }
            } while (keepGoing);
        }


        #region Randomised output on fixed dimension canvas example

        const int CANVAS_WIDTH = 10;
        const int CANVAS_HEIGHT = 10;

        static void DrawRandomisedCanvas()
        {
            int x = 0; 
            int y = 0;

            Random r = new Random();
            bool keepGoing = true;

            Console.CursorVisible = false;

            do
            {
                //Clear the previous character written
                Console.SetCursorPosition(x, y);
                Console.Write(' ');

                //New x, y coordinates
                x = r.Next(CANVAS_WIDTH - 1); //zero based canvas dimensions, ie. 0 to CANVAS_WIDTH - 1
                y = r.Next(CANVAS_HEIGHT - 1); //ditto

                Console.SetCursorPosition(x, y);
                Console.Write('1');

                Thread.Sleep(WAIT);

                if (Console.KeyAvailable)
                {
                    //break out of the while loop and terminate the console
                    keepGoing = false;
                }
            } while (keepGoing);
        }

        #endregion

        #region Simple spinner example

        const string SPINNER = "/―\\|"; //Use a unicode bar character rather than the ASCII equavalent

        static void DrawSpinner()
        {
            int index = 0;
            bool keepGoing = true;

            Console.OutputEncoding = System.Text.Encoding.UTF8;

            do
            {
                Console.SetCursorPosition(1, 1);

                Console.WriteLine(SPINNER[index]);

                index++;
                if (index > SPINNER.Length - 1)
                    index = 0;

                Thread.Sleep(WAIT);

                if (Console.KeyAvailable)
                {
                    //break out of the while loop and terminate the console
                    keepGoing = false;
                }
            } while (keepGoing);
        }

        #endregion
    }
}
