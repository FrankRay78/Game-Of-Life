using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game_Of_Life_LIB;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Game_Of_Life_TESTS
{
    [TestClass]
    public class Life_Tests
    {
        [TestMethod]
        public void Blank0x0Life()
        {
            int x = 0;
            int y = 0;

            var l = new Life(x, y);

            Assert.AreEqual("", l.ToString());
        }

        [TestMethod]
        public void Blank1x1Life()
        {
            int x = 1;
            int y = 1;

            var l = new Life(x, y);

            Assert.AreEqual("0", l.ToString());
        }

        [TestMethod]
        public void Blank3x3Life()
        {
            var expected = "000\n000\n000";

            int x = 3;
            int y = 3;

            var l = new Life(x, y);

            Assert.AreEqual(expected, l.ToString());
        }

        [TestMethod]
        public void FullyAlive3x3Life()
        {
            var input = "111\n111\n111";
            var expected = "111\n111\n111";

            var l = new Life(input);

            Assert.AreEqual(expected, l.ToString());
        }

        [TestMethod]
        public void PartiallyAlive_BadCharacters_RaggedArray_5x5Life()
        {
            var input = "0_1\n00001\n11ABC";
            var expected = "00100\n00001\n11000";

            var l = new Life(input);

            Assert.AreEqual(expected, l.ToString());
        }
    }
}
