using Microsoft.VisualStudio.TestTools.UnitTesting;
using WpfMinesweeper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfMinesweeper.Models.Tests
{
    [TestClass()]
    public class MinesweeperBoardValidatorTests
    {

        [TestMethod()]
        public void CreateTest()
        {
            var boardValidator = MinesweeperBoardValidator.Create();
            Assert.IsNotNull(boardValidator);
        }

        [TestMethod()]
        public void ValidateBoardTest()
        {
            var boardValidator = MinesweeperBoardValidator.Create();
            boardValidator.ValidateBoard(0, 0, 0);
            Assert.Fail();
        }

        [TestMethod()]
        public void ValidateHeightTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ValidateMineCountTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ValidateWidthTest()
        {
            var boardValidator = MinesweeperBoardValidator.Create();
            boardValidator.ValidateWidth(0);
        }
    }
}