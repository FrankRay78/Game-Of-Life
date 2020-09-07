using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Game_Of_Life
{
    //https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life

    class Program
    {
        const int WAIT = 500;

        static void Main(string[] args)
        {
            //DrawSpinner();

            DrawRandomisedCanvas();
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
