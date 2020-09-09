﻿using System;
using Game_Of_Life_LIB;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Game_Of_Life_TESTS
{
    [TestClass]
    public class Cell_Tests
    {
        [TestMethod]
        public void AliveCell()
        {
            var grid = new int[1, 1];

            int x = 0;
            int y = 0;

            //Mark the cell as alive initially
            grid[x, y] = 1;

            Cell c = new Cell(x, y, grid);

            Assert.AreEqual("1", c.ToString());
        }

        [TestMethod]
        public void DeadCell()
        {
            var grid = new int[1, 1];

            int x = 0;
            int y = 0;

            Cell c = new Cell(x, y, grid);

            Assert.AreEqual("0", c.ToString());
        }

        [TestMethod]
        public void AliveThenDeadCell_ChangingUnderlyingGrid()
        {
            var grid = new int[1, 1];

            int x = 0;
            int y = 0;

            //Mark the cell as alive initially
            grid[x, y] = 1;

            Cell c = new Cell(x, y, grid);

            Assert.AreEqual("1", c.ToString());

            //Mark the cell as dead
            grid[x, y] = 0;

            Assert.AreEqual("0", c.ToString());
        }

        [TestMethod]
        public void AliveThenDeadCell_CallingCellUpdateStateMethod()
        {
            var grid = new int[1, 1];

            int x = 0;
            int y = 0;

            //Mark the cell as alive initially
            grid[x, y] = 1;

            Cell c = new Cell(x, y, grid);

            Assert.AreEqual("1", c.ToString());

            //Move the cell onto it's next generation
            c.UpdateState();

            Assert.AreEqual("0", c.ToString());
        }
    }
}
