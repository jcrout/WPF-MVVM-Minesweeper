namespace WpfMinesweeper.Unit_Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Models;

    [TestClass]
    public class MinesweeperTest
    {
        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void MyTestMethod()
        {
            var minesweeper = Minesweeper.Create(0, 0, 10);

            minesweeper = Minesweeper.Create(55, 0, 10);

            minesweeper = Minesweeper.Create(0, 66, 10);

            minesweeper = Minesweeper.Create(55, 22, 00);
        }
    }
}
