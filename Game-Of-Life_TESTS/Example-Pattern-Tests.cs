using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game_Of_Life_LIB;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Game_Of_Life_TESTS
{
    //ref: https://en.wikipedia.org/wiki/Conway%27s_Game_of_Life

    [TestClass]
    public class Example_Pattern_Tests
    {
        [TestMethod]
        public void Still_Life_Block()
        {
            var startingPattern = "0000\n0110\n0110\n0000";

            var life = new Life(startingPattern);

            var expected = "0000\n0110\n0110\n0000";

            Assert.AreEqual(expected, life.ToString());

            life.UpdateState();

            Assert.AreEqual(expected, life.ToString());

            life.UpdateState();

            Assert.AreEqual(expected, life.ToString());
        }

        [TestMethod]
        public void Still_Life_Tub()
        {
            var startingPattern = "00000\n00100\n01010\n00100\n00000";

            var life = new Life(startingPattern);

            var expected = "00000\n00100\n01010\n00100\n00000";

            Assert.AreEqual(expected, life.ToString());

            life.UpdateState();

            Assert.AreEqual(expected, life.ToString());

            life.UpdateState();

            Assert.AreEqual(expected, life.ToString());
        }

        [TestMethod]
        public void Oscillator_Blinker()
        {
            var startingPattern = "00000\n00000\n01110\n00000\n00000"; //Oscillator_Blinker
            var expected1 = "00000\n00000\n01110\n00000\n00000";
            var expected2 = "00000\n00100\n00100\n00100\n00000";

            var life = new Life(startingPattern);

            Assert.AreEqual(expected1, life.ToString());

            life.UpdateState();

            Assert.AreEqual(expected2, life.ToString());

            life.UpdateState();

            Assert.AreEqual(expected1, life.ToString());
        }
        

        [TestMethod]
        public void Life_10x10_Apply_Plattern_Block()
        {
            int x = 10;
            int y = 10;

            var pattern = Patterns.Still_Life_Block;

            int patternStartX = 1;
            int patternStartY = 2;

            var expected1 = "0000000000\n0000000000\n0000000000\n0000000000\n0000000000\n0000000000\n0000000000\n0000000000\n0000000000\n0000000000";
            var expected2 = "0000000000\n0000000000\n0110000000\n0110000000\n0000000000\n0000000000\n0000000000\n0000000000\n0000000000\n0000000000";

            var life = new Life(x, y);

            Assert.AreEqual(expected1, life.ToString());

            life.ApplyPattern(pattern, patternStartX, patternStartY);

            Assert.AreEqual(expected2, life.ToString());
        }

        [TestMethod]
        public void Life_20x20_Apply_Oscillator_Blinker()
        {
            int x = 10;
            int y = 10;

            var pattern = Patterns.Oscillator_Blinker;

            int patternStartX = 7;
            int patternStartY = 9;

            var expected1 = "0000000000\n0000000000\n0000000000\n0000000000\n0000000000\n0000000000\n0000000000\n0000000000\n0000000000\n0000000000";
            var expected2 = "0000000000\n0000000000\n0000000000\n0000000000\n0000000000\n0000000000\n0000000000\n0000000000\n0000000000\n0000000111";

            var life = new Life(x, y);

            Assert.AreEqual(expected1, life.ToString());

            life.ApplyPattern(pattern, patternStartX, patternStartY);

            Assert.AreEqual(expected2, life.ToString());
        }
    }
}
