namespace WpfMinesweeper.Models
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Miscellanious;

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
        ///     Gets the collection of tiles that make up the board.
        /// </summary>
        ITileCollection Tiles { get; }

        /// <summary>
        ///     Gets the total amount of time that has elapsed since the game began.
        /// </summary>
        int TimeElapsed { get; set; }
    }

    /// <summary>
    ///     THe default implementation of the <see cref="IMinesweeper" /> interface.
    /// </summary>
    public class Minesweeper : IMinesweeper
    {
        /// <summary>
        ///     Total number of mines on the board.
        /// </summary>
        private int mineCount;

        /// <summary>
        ///     Total number of mines remaining (unflagged) on the board.
        /// </summary>
        private int minesLeft;

        /// <summary>
        ///     The collection of <see cref="tiles" /> that make up the board.
        /// </summary>
        private ITileCollection tiles;

        //private IEnumerable<>

        /// <summary>
        ///     Total amount of time that has elapsed since the game began.
        /// </summary>
        private int timeElapsed;

        private Minesweeper()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int MineCount
        {
            get
            {
                return this.mineCount;
            }
            private set
            {
                if (this.mineCount != value)
                {
                    this.mineCount = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public int MinesRemaining
        {
            get
            {
                return this.minesLeft;
            }
            set
            {
                if (this.minesLeft != value)
                {
                    this.minesLeft = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public ITileCollection Tiles
        {
            get
            {
                return this.tiles;
            }
            private set
            {
                if (this.tiles != value)
                {
                    this.tiles = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public int TimeElapsed
        {
            get
            {
                return this.timeElapsed;
            }
            set
            {
                if (this.timeElapsed != value)
                {
                    this.timeElapsed = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public static Minesweeper Create(int width, int height, int mineCount)
        {
            var validationResult = MinesweeperBoardValidator.Create().ValidateBoard(
                width,
                height,
                mineCount);

            if (validationResult != null)
            {
                throw new ArgumentException(validationResult);
            }

            var minesweeper = new Minesweeper();
            minesweeper.tiles = TileCollection.Create(
                width,
                height);
            minesweeper.mineCount = mineCount;
            minesweeper.minesLeft = mineCount;
            minesweeper.timeElapsed = 0;

            return minesweeper;
        }

        protected void RaisePropertyChanged([CallerMemberName] string prop = "")
        {
            var propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(
                    this,
                    new PropertyChangedEventArgs(prop));
            }
        }
    }

    public class MinesweeperFactory
    {
        private static readonly ISettingsProvider settings = SettingsProvider.Instance;

        public static Minesweeper GetFromSettings()
        {
            return Minesweeper.Create(
                MinesweeperFactory.settings.LastBoardSize.Width,
                MinesweeperFactory.settings.LastBoardSize.Height,
                MinesweeperFactory.settings.LastBoardSize.MineCount);
        }

        public static Minesweeper Create(int width, int height, int mineCount)
        {
            return Minesweeper.Create(
                width,
                height,
                mineCount);
        }

        public static Minesweeper Create(BoardSize board)
        {
            return Minesweeper.Create(
                board.Width,
                board.Height,
                board.MineCount);
        }

        public static Minesweeper Create(IMinesweeper minesweeper)
        {
            return Minesweeper.Create(
                minesweeper.Tiles.Width,
                minesweeper.Tiles.Height,
                minesweeper.MineCount);
        }
    }
}
