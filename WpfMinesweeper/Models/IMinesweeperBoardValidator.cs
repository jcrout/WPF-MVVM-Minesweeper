namespace WpfMinesweeper.Models
{
    /// <summary>
    ///     <para>
    ///         Defines a set of methods for validating a
    ///         <see cref="Minesweeper" />
    ///     </para>
    ///     <para>board by width, height, and mine count.</para>
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
}
