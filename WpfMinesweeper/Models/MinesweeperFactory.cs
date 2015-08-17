namespace WpfMinesweeper.Models
{
    public class MinesweeperFactory
    {
        private static readonly ISettingsProvider settings = SettingsProvider.Instance;

        public static IMinesweeper GetFromSettings()
        {
            return Minesweeper.Create(MinesweeperFactory.settings.LastBoardSize.Width, MinesweeperFactory.settings.LastBoardSize.Height, MinesweeperFactory.settings.LastBoardSize.MineCount);
        }

        public static IMinesweeper Create(int width, int height, int mineCount)
        {
            return Minesweeper.Create(width, height, mineCount);
        }

        public static IMinesweeper Create(BoardSize board)
        {
            return Minesweeper.Create(board.Width, board.Height, board.MineCount);
        }

        public static IMinesweeper Create(IMinesweeper minesweeper)
        {
            return Minesweeper.Create(minesweeper.Tiles.Width, minesweeper.Tiles.Height, minesweeper.MineCount);
        }
    }
}
