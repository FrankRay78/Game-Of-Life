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
    public class Helper_Tests
    {
        [TestMethod]
        public void Test_000()
        {
            string input = "000";
            string expected = "000";

            int[,] g = Helper.StringToIntMatrix(input);

            string output = Helper.IntMatrixToString(g);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void Test_0A0()
        {
            string input = "0A0";
            string expected = "000";

            int[,] g = Helper.StringToIntMatrix(input);

            string output = Helper.IntMatrixToString(g);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void Test_000n000()
        {
            string input = "000\n000";
            string expected = "000\n000";

            int[,] g = Helper.StringToIntMatrix(input);

            string output = Helper.IntMatrixToString(g);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void Test_010n000()
        {
            string input = "010\n000";
            string expected = "010\n000";

            int[,] g = Helper.StringToIntMatrix(input);

            string output = Helper.IntMatrixToString(g);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void Test_000n010()
        {
            string input = "000\n010";
            string expected = "000\n010";

            int[,] g = Helper.StringToIntMatrix(input);

            string output = Helper.IntMatrixToString(g);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void Test_010n101()
        {
            string input = "010\n101";
            string expected = "010\n101";

            int[,] g = Helper.StringToIntMatrix(input);

            string output = Helper.IntMatrixToString(g);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void Test_StaggeredArray()
        {
            string input = "010010\n101\n11";
            string expected = "010010\n101000\n110000";

            int[,] g = Helper.StringToIntMatrix(input);

            string output = Helper.IntMatrixToString(g);

            Assert.AreEqual(expected, output);
        }
    }
}
