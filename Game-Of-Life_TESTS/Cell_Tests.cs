using System;
using Game_Of_Life_LIB;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Game_Of_Life_TESTS
{
    [TestClass]
    public class Cell_Tests
    {
        [TestMethod]
        public void Cell_1x1_AliveThenDeadCell()
        {
            var grid = new int[1, 1];

            int x = 0;
            int y = 0;

            //Mark the cell as alive initially
            grid[x, y] = 1;

            //Move the cell onto it's next generation
            Cell.UpdateState(x, y, grid);

            Assert.AreEqual(0, grid[x, y]);
        }
    }
}
