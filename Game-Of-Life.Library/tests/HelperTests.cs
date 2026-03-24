using System;
using GameOfLife.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameOfLife.Tests
{
    [TestClass]
    public class HelperTests
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

        [TestMethod]
        public void StringToIntMatrix_NullInput_ThrowsArgumentException()
        {
            Assert.ThrowsExactly<ArgumentException>(() => Helper.StringToIntMatrix(null));
        }

        [TestMethod]
        public void StringToIntMatrix_EmptyString_ThrowsArgumentException()
        {
            Assert.ThrowsExactly<ArgumentException>(() => Helper.StringToIntMatrix(""));
        }

        [TestMethod]
        public void StringToIntMatrix_WhitespaceOnly_ThrowsArgumentException()
        {
            Assert.ThrowsExactly<ArgumentException>(() => Helper.StringToIntMatrix("   "));
        }

        [TestMethod]
        public void StringToIntMatrix_DigitGreaterThanOne_PreservedInGrid()
        {
            // Non-zero digits 2–9 are stored as-is via c - '0'; this test pins that contract.
            int[,] grid = Helper.StringToIntMatrix("2");

            Assert.AreEqual(2, grid[0, 0]);
        }

        [TestMethod]
        public void StringToIntMatrix_LeadingAndTrailingNewlines_ParsedCorrectly()
        {
            // Empty rows from leading/trailing newlines count toward grid height.
            // The resulting grid is 3 rows tall with the middle row containing "010".
            string input = "\n010\n";
            string expected = "000\n010\n000";

            int[,] grid = Helper.StringToIntMatrix(input);
            string output = Helper.IntMatrixToString(grid);

            Assert.AreEqual(expected, output);
        }
    }
}
