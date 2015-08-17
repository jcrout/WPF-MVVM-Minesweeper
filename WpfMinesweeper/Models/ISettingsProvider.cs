namespace WpfMinesweeper.Models
{
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Media;

    public interface ISettingsProvider
    {
        /// <summary>
        ///     Gets or sets the most recent board size used.
        /// </summary>
        BoardSize LastBoardSize { get; set; }

        /// <summary>
        ///     Gets or sets the last main window location.
        /// </summary>
        Point LastLocation { get; set; }

        /// <summary>
        ///     Gets or sets the last main window size.
        /// </summary>
        Size LastWindowMinSize { get; set; }

        /// <summary>
        ///     Gets or sets the list of individual game statistics.
        /// </summary>
        ObservableCollection<IStatisticsModule> Statistics { get; set; }

        /// <summary>
        ///     Gets or sets the brush used to paint the tiles on a Minesweeper tile board.
        /// </summary>
        Brush TileBrush { get; set; }

        /// <summary>
        ///     Saves all settings.
        /// </summary>
        void Save();
    }
}
