namespace WpfMinesweeper.Models
{
    /// <summary>
    ///     Defines a set of methods for validating a <see cref="Minesweeper" />
    ///     board by width, height, and mine count.
    /// </summary>
    public interface IMinesweeperBoardValidator
    {
        /// <summary>
        ///     Validates the board parameters and returns an error message string if there are any errors; otherwise, returns
        ///     null.
        /// </summary>
        /// <param name="width">The number of tiles per row.</param>
        /// <param name="height">The number of tiles per column.</param>
        /// <param name="mineCount">The total number of mines on the board.</param>
        /// <returns>An error message if there are any errors; otherwise, null.</returns>
        string ValidateBoard(int width, int height, int mineCount);

        /// <summary>
        ///     Validates the number of tiles per column and returns an error message string if there are any errors; otherwise,
        ///     returns null.
        /// </summary>
        /// <param name="height">The number of tiles per column.</param>
        /// <returns>An error message if there are any errors; otherwise, null.</returns>
        string ValidateHeight(int height);

        /// <summary>
        ///     Validates the total number of mines, given a board <paramref name="width" /> and height, and returns an error
        ///     message string if there are any errors; otherwise, returns null. This method does not validate the
        ///     <paramref name="width" /> and height.
        /// </summary>
        /// <param name="mineCount">The total number of mines on the board.</param>
        /// <param name="width">The number of tiles per row.</param>
        /// <param name="height">The number of tiles per column.</param>
        /// <returns>An error message if there are any errors; otherwise, null.</returns>
        string ValidateMineCount(int mineCount, int width, int height);

        /// <summary>
        ///     Validates the number of tiles per row and returns an error message string if there are any errors; otherwise,
        ///     returns null.
        /// </summary>
        /// <param name="width">The number of tiles per row.</param>
        /// <returns>An error message if there are any errors; otherwise, null.</returns>
        string ValidateWidth(int width);
    }

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
            var validateWidth = this.ValidateWidth(
                width);
            if (validateWidth != null)
            {
                return validateWidth;
            }

            var validateHeight = this.ValidateHeight(
                height);
            if (validateHeight != null)
            {
                return validateHeight;
            }

            var validateMineCount = this.ValidateMineCount(mineCount, width, height);
            if (validateMineCount != null)
            {
                return validateMineCount;
            }

            return null;
        }

        public string ValidateHeight(int height)
        {
            if (height < MinesweeperBoardValidator.minimumHeight)
            {
                return string.Format(
                    "Board height must be greater than {0}.",
                    MinesweeperBoardValidator.minimumHeight - 1);
            }

            if (height > MinesweeperBoardValidator.maximumHeight)
            {
                return string.Format(
                    "Board height must be less than {0}.",
                    MinesweeperBoardValidator.maximumHeight + 1);
            }

            return null;
        }

        public string ValidateMineCount(int mineCount, int width, int height)
        {
            if (mineCount < MinesweeperBoardValidator.minimumMineCount)
            {
                return string.Format(
                    "Mine count must be greater than {0}.",
                    MinesweeperBoardValidator.minimumMineCount - 1);
            }

            var targetMaximum = width * height - MinesweeperBoardValidator.minimumMineCountDifference;
            if (mineCount > targetMaximum)
            {
                return string.Format(
                    "Mine count on a {0}x{1} board must be less than {2}.",
                    width,
                    height,
                    targetMaximum);
            }

            return null;
        }

        public string ValidateWidth(int width)
        {
            if (width < MinesweeperBoardValidator.minimumWidth)
            {
                return string.Format(
                    "Board width must be greater than {0}.",
                    MinesweeperBoardValidator.minimumWidth - 1);
            }

            if (width > MinesweeperBoardValidator.maximumWidth)
            {
                return string.Format(
                    "Board width must be less than {0}.",
                    MinesweeperBoardValidator.maximumWidth + 1);
            }

            return null;
        }
    }
}
