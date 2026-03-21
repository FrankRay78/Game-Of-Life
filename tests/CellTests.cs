using GameOfLife.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameOfLife.Tests
{
    [TestClass]
    public class CellTests
    {
        // Cell.UpdateState writes into nextGenerationGrid.
        // nextGenerationGrid is initialised as a clone of grid, mirroring how Life.UpdateState uses it.
        // On survival, the method returns early without writing, so the cloned value of 1 is preserved.
        // On death or birth, it explicitly sets the value to 0 or 1 respectively.

        [TestMethod]
        public void UpdateState_LiveCellWithNoNeighbours_Dies()
        {
            // Arrange
            int[,] grid = new int[3, 3];
            grid[1, 1] = 1;
            int[,] nextGenerationGrid = (int[,])grid.Clone();

            // Act
            Cell.UpdateState(1, 1, grid, nextGenerationGrid);

            // Assert
            Assert.AreEqual(0, nextGenerationGrid[1, 1]);
        }

        [TestMethod]
        public void UpdateState_LiveCellWithOneNeighbour_Dies()
        {
            // Arrange
            int[,] grid = new int[3, 3];
            grid[1, 1] = 1;
            grid[0, 0] = 1;
            int[,] nextGenerationGrid = (int[,])grid.Clone();

            // Act
            Cell.UpdateState(1, 1, grid, nextGenerationGrid);

            // Assert
            Assert.AreEqual(0, nextGenerationGrid[1, 1]);
        }

        [TestMethod]
        public void UpdateState_LiveCellWithTwoNeighbours_Survives()
        {
            // Arrange
            int[,] grid = new int[3, 3];
            grid[1, 1] = 1;
            grid[0, 0] = 1;
            grid[1, 0] = 1;
            int[,] nextGenerationGrid = (int[,])grid.Clone();

            // Act
            Cell.UpdateState(1, 1, grid, nextGenerationGrid);

            // Assert
            Assert.AreEqual(1, nextGenerationGrid[1, 1]);
        }

        [TestMethod]
        public void UpdateState_LiveCellWithThreeNeighbours_Survives()
        {
            // Arrange
            int[,] grid = new int[3, 3];
            grid[1, 1] = 1;
            grid[0, 0] = 1;
            grid[1, 0] = 1;
            grid[2, 0] = 1;
            int[,] nextGenerationGrid = (int[,])grid.Clone();

            // Act
            Cell.UpdateState(1, 1, grid, nextGenerationGrid);

            // Assert
            Assert.AreEqual(1, nextGenerationGrid[1, 1]);
        }

        [TestMethod]
        public void UpdateState_LiveCellWithFourNeighbours_Dies()
        {
            // Arrange
            int[,] grid = new int[3, 3];
            grid[1, 1] = 1;
            grid[0, 0] = 1;
            grid[1, 0] = 1;
            grid[2, 0] = 1;
            grid[0, 1] = 1;
            int[,] nextGenerationGrid = (int[,])grid.Clone();

            // Act
            Cell.UpdateState(1, 1, grid, nextGenerationGrid);

            // Assert
            Assert.AreEqual(0, nextGenerationGrid[1, 1]);
        }

        [TestMethod]
        public void UpdateState_DeadCellWithTwoNeighbours_StaysDead()
        {
            // Arrange
            int[,] grid = new int[3, 3];
            grid[0, 0] = 1;
            grid[1, 0] = 1;
            int[,] nextGenerationGrid = (int[,])grid.Clone();

            // Act
            Cell.UpdateState(1, 1, grid, nextGenerationGrid);

            // Assert
            Assert.AreEqual(0, nextGenerationGrid[1, 1]);
        }

        [TestMethod]
        public void UpdateState_DeadCellWithThreeNeighbours_BecomesAlive()
        {
            // Arrange
            int[,] grid = new int[3, 3];
            grid[0, 0] = 1;
            grid[1, 0] = 1;
            grid[2, 0] = 1;
            int[,] nextGenerationGrid = (int[,])grid.Clone();

            // Act
            Cell.UpdateState(1, 1, grid, nextGenerationGrid);

            // Assert
            Assert.AreEqual(1, nextGenerationGrid[1, 1]);
        }

        [TestMethod]
        public void UpdateState_DeadCellWithFourNeighbours_StaysDead()
        {
            // Arrange
            int[,] grid = new int[3, 3];
            grid[0, 0] = 1;
            grid[1, 0] = 1;
            grid[2, 0] = 1;
            grid[0, 1] = 1;
            int[,] nextGenerationGrid = (int[,])grid.Clone();

            // Act
            Cell.UpdateState(1, 1, grid, nextGenerationGrid);

            // Assert
            Assert.AreEqual(0, nextGenerationGrid[1, 1]);
        }

        [TestMethod]
        public void UpdateState_CornerCellWithThreeLiveNeighbours_BecomesAlive()
        {
            // A dead corner cell (0,0) with all three possible in-bounds neighbours alive
            // should become alive. This exercises the GetCellState boundary guard.

            // Arrange
            int[,] grid = new int[2, 2];
            grid[1, 0] = 1;
            grid[0, 1] = 1;
            grid[1, 1] = 1;
            int[,] nextGenerationGrid = (int[,])grid.Clone();

            // Act
            Cell.UpdateState(0, 0, grid, nextGenerationGrid);

            // Assert
            Assert.AreEqual(1, nextGenerationGrid[0, 0]);
        }
    }
}
