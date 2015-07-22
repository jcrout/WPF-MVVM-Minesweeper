using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfMinesweeper.Models
{
    public interface IMinesweeperBoardValidator
    {
        string ValidateBoard(int width, int height, int mineCount);
        string ValidateWidth(int width);
        string ValidateHeight(int height);
        string ValidateMineCount(int mineCount, int width, int height);
    }

    public class MinesweeperBoardValidator : IMinesweeperBoardValidator
    {
        private static readonly IMinesweeperBoardValidator defaultMinesweeperBoardValidator;
        private static int minimumWidth = 8;
        private static int minimumHeight = 8;
        private static int maximumWidth = 100;
        private static int maximumHeight = 100;
        private static int minimumMineCount = 1;
        private static int minimumMineCountDifference = 1;

        static MinesweeperBoardValidator()
        {
            defaultMinesweeperBoardValidator = new MinesweeperBoardValidator();
        }

        public static IMinesweeperBoardValidator Create()
        {
            return defaultMinesweeperBoardValidator;
        }

        public string ValidateBoard(int width, int height, int mineCount)
        {
            string validateWidth = this.ValidateWidth(width);
            if (validateWidth != null)
            {
                return validateWidth;
            }

            string validateHeight = this.ValidateHeight(height);
            if (validateHeight != null)
            {
                return validateHeight;
            }

            string validateMineCount = this.ValidateMineCount(mineCount: mineCount, width: width, height: height);
            if (validateMineCount != null)
            {
                return validateMineCount;
            }

            return null;
        }

        public string ValidateWidth(int width)
        {
            if (width < minimumWidth)
            {
                return string.Format("Board width must be greater than {0}.", minimumWidth - 1);
            }

            if (width > maximumWidth)
            {
                return string.Format("Board width must be less than {0}.", maximumWidth + 1);
            }

            return null;
        }

        public string ValidateHeight(int height)
        {
            if (height < minimumHeight)
            {
                return string.Format("Board height must be greater than {0}.", minimumHeight - 1);
            }

            if (height > maximumHeight)
            {
                return string.Format("Board height must be less than {0}.", maximumHeight + 1);
            }

            return null;
        }

        public string ValidateMineCount(int mineCount, int width, int height)
        {
            if (mineCount < minimumMineCount)
            {
                return string.Format("Mine count must be greater than {0}.", minimumMineCount - 1);
            }

            int targetMaximum = width * height - minimumMineCountDifference;
            if (mineCount > targetMaximum)
            {
                return string.Format("Mine count on a {0}x{1} board must be less than {2}.", width, height, targetMaximum);
            }

            return null;
        }

        private MinesweeperBoardValidator()
        {
        }
    }
}
