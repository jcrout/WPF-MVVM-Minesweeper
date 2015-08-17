namespace WpfMinesweeper.Models
{
    using System.ComponentModel;

    /// <summary>
    ///     This <see langword="interface" /> implements the core aspects of the
    ///     game Minesweeper.
    /// </summary>
    public interface IMinesweeper : INotifyPropertyChanged
    {
        /// <summary>
        ///     Gets the total number of mines on the board.
        /// </summary>
        int MineCount { get; }

        /// <summary>
        ///     Gets the total number of mines remaining (unflagged) on the board.
        /// </summary>
        int MinesRemaining { get; set; }

        /// <summary>
        ///     Gets the collection of <see cref="Tile" />s that make up the board.
        /// </summary>
        ITileCollection Tiles { get; }

        /// <summary>
        ///     Gets the total amount of time that has elapsed since the game began.
        /// </summary>
        int TimeElapsed { get; set; }
    }
}
