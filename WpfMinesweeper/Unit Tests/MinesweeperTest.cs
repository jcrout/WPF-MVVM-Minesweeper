namespace WpfMinesweeper.Unit_Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using WpfMinesweeper.Models;

    [TestClass]
    public class MinesweeperTest
    {
        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void MyTestMethod()
        {
            var minesweeper = Minesweeper.Create(0, 0, 10);

            minesweeper = Minesweeper.Create(55, 0, 10);

            minesweeper = Minesweeper.Create(0, 66, 10);

            minesweeper = Minesweeper.Create(55, 22, 00);
        }
    }
}
