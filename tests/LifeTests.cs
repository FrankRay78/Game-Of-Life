using System;
using GameOfLife.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameOfLife.Tests
{
    [TestClass]
    public class LifeTests
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

        [TestMethod]
        public void Generation_NewLife_IsZero()
        {
            var l = new Life(3, 3);

            Assert.AreEqual(0, l.Generation);
        }

        [TestMethod]
        public void Generation_AfterOneUpdateState_IsOne()
        {
            var l = new Life(3, 3);

            l.UpdateState();

            Assert.AreEqual(1, l.Generation);
        }

        [TestMethod]
        public void Generation_AfterThreeUpdateStates_IsThree()
        {
            var l = new Life(3, 3);

            l.UpdateState();
            l.UpdateState();
            l.UpdateState();

            Assert.AreEqual(3, l.Generation);
        }

        [TestMethod]
        public void Grid_NewLife_HasCorrectDimensions()
        {
            // Dimension 0 is x (width), dimension 1 is y (height) — per project convention.
            var l = new Life(5, 3);

            Assert.AreEqual(5, l.Grid.GetLength(0));
            Assert.AreEqual(3, l.Grid.GetLength(1));
        }

        [TestMethod]
        public void ApplyPattern_PatternExceedsGridBoundary_ThrowsIndexOutOfRangeException()
        {
            // ApplyPattern has no bounds check. This test documents the current behaviour
            // as a fixture: placing a 2x2 pattern at (9,9) on a 10x10 grid throws.
            // Any future change that adds a guard should update this test intentionally.
            var l = new Life(10, 10);

            Assert.ThrowsExactly<IndexOutOfRangeException>(() =>
                l.ApplyPattern(Patterns.Still_Life_Block, 9, 9));
        }
    }
}
