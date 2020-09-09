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
        public void Blank1x1Life()
        {
            int x = 0;
            int y = 0;

            var l = new Life(x, y);

            Assert.AreEqual(Cell.DEAD_PRINT_CHARACTERS, l.ToString());
        }
    }
}
