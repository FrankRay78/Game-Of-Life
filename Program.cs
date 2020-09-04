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
        const string SPINNER = "/-\\|";

        static void Main(string[] args)
        {
            int index = 0;
            bool keepGoing = true;

            do
            {
                Console.SetCursorPosition(0, 0);

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
    }
}
