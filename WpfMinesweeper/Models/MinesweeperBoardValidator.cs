namespace WpfMinesweeper.Models
{
    public class MinesweeperBoardValidator : IMinesweeperBoardValidator
    {
        private static readonly IMinesweeperBoardValidator defaultMinesweeperBoardValidator;
        private static readonly int maximumHeight = 100;
        private static readonly int maximumWidth = 100;
        private static readonly int minimumHeight = 8;
        private static readonly int minimumMineCount = 1;
        private static readonly int minimumMineCountDifference = 1;
        private static readonly int minimumWidth = 8;

        static MinesweeperBoardValidator()
        {
            MinesweeperBoardValidator.defaultMinesweeperBoardValidator = new MinesweeperBoardValidator();
        }

        private MinesweeperBoardValidator()
        {
        }

        public static IMinesweeperBoardValidator Create()
        {
            return MinesweeperBoardValidator.defaultMinesweeperBoardValidator;
        }

        public string ValidateBoard(int width, int height, int mineCount)
        {
            var validateWidth = this.ValidateWidth(width);
            if (validateWidth != null)
            {
                return validateWidth;
            }

            var validateHeight = this.ValidateHeight(height);
            if (validateHeight != null)
            {
                return validateHeight;
            }

            var validateMineCount = this.ValidateMineCount(mineCount, width, height);
            return validateMineCount;
        }

        public string ValidateHeight(int height)
        {
            if (height < MinesweeperBoardValidator.minimumHeight)
            {
                return string.Format("Board height must be greater than {0}.", MinesweeperBoardValidator.minimumHeight - 1);
            }

            if (height > MinesweeperBoardValidator.maximumHeight)
            {
                return string.Format("Board height must be less than {0}.", MinesweeperBoardValidator.maximumHeight + 1);
            }

            return null;
        }

        public string ValidateMineCount(int mineCount, int width, int height)
        {
            if (mineCount < MinesweeperBoardValidator.minimumMineCount)
            {
                return string.Format("Mine count must be greater than {0}.", MinesweeperBoardValidator.minimumMineCount - 1);
            }

            var targetMaximum = width * height - MinesweeperBoardValidator.minimumMineCountDifference;
            if (mineCount > targetMaximum)
            {
                return string.Format("Mine count on a {0}x{1} board must be less than {2}.", width, height, targetMaximum);
            }

            return null;
        }

        public string ValidateWidth(int width)
        {
            if (width < MinesweeperBoardValidator.minimumWidth)
            {
                return string.Format("Board width must be greater than {0}.", MinesweeperBoardValidator.minimumWidth - 1);
            }

            if (width > MinesweeperBoardValidator.maximumWidth)
            {
                return string.Format("Board width must be less than {0}.", MinesweeperBoardValidator.maximumWidth + 1);
            }

            return null;
        }
    }
}
