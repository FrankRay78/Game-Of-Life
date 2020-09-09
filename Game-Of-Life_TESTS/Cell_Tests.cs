using System;
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

            Assert.AreEqual(Cell.ALIVE_PRINT_CHARACTERS, c.ToString());
        }

        [TestMethod]
        public void DeadCell()
        {
            var grid = new int[1, 1];

            int x = 0;
            int y = 0;

            Cell c = new Cell(x, y, grid);

            Assert.AreEqual(Cell.DEAD_PRINT_CHARACTERS, c.ToString());
        }

        [TestMethod]
        public void AliveThenDeadCell()
        {
            var grid = new int[1, 1];

            int x = 0;
            int y = 0;

            //Mark the cell as alive initially
            grid[x, y] = 1;

            Cell c = new Cell(x, y, grid);

            Assert.AreEqual(Cell.ALIVE_PRINT_CHARACTERS, c.ToString());

            //Mark the cell as dead
            grid[x, y] = 0;

            Assert.AreEqual(Cell.DEAD_PRINT_CHARACTERS, c.ToString());
        }



        //[TestMethod]
        //public void SingleDeadCellinMiddleofDead3x3Grid()
        //{
        //    var grid = new int[3, 3]; //should be 0 - 2, on both axes

        //    int x = 1;
        //    int y = 1;

        //    Cell c = new Cell(x, y, grid);

        //    Assert.AreEqual(0, c.Alive);

        //    c.UpdateState();

        //    Assert.AreEqual(0, c.Alive);
        //}

        //[TestMethod]
        //public void SingleAliveCellinMiddleofDead3x3Grid()
        //{
        //    var grid = new int[3, 3]; //should be 0 - 2, on both axes

        //    int x = 1;
        //    int y = 1;

        //    Cell c = new Cell(x, y, grid);

        //    //Mark the cell as alive initially
        //    grid[x, y] = 1;

        //    Assert.AreEqual(1, c.Alive);

        //    c.UpdateState();

        //    Assert.AreEqual(0, c.Alive);
        //}

        ////Blocks are static (ie. don't change during generations):
        ////0000
        ////0110
        ////0110
        ////0000
        //[TestMethod]
        //public void TopLeftAliveCellinBlock()
        //{
        //    //TODO
        //    throw new NotImplementedException();
        //}
    }
}
