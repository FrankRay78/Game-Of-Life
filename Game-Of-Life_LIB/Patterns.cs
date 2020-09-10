using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Of_Life_LIB
{
    //ref: https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life

    public static class Patterns
    {
        public static string Still_Life_Block =
            "11\n" +
            "11";

        public static string Still_Life_Tub =
            "010\n" +
            "101\n" +
            "010";

        public static string Oscillator_Blinker =
            "111";

        public static string Acorn =
            "000000000\n" +
            "001000000\n" +
            "000010000\n" +
            "011001110\n" +
            "000000000";

        public static string R_Pentomino =
            "011\n" +
            "110\n" +
            "010";
    }
}
