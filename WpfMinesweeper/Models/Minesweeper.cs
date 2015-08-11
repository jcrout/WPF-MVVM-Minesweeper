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
        ///     Gets the collection of <see cref="Tile"/>s that make up the board.
        /// </summary>
        ITileCollection Tiles { get; }

        /// <summary>
        ///     Gets the total amount of time that has elapsed since the game began.
        /// </summary>
        int TimeElapsed { get; set; }
    }

    /// <summary>
    ///     The default implementation of the <see cref="IMinesweeper" /> interface.
    /// </summary>
    public class Minesweeper : IMinesweeper
    {
        private int mineCount;

        private int minesLeft;

        private ITileCollection tiles;

        private int timeElapsed;

        /// <summary>
        ///     Prevents a default instance of the <see cref="Minesweeper"/> class from being created.
        /// </summary>
        private Minesweeper()
        {
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Gets the total number of mines on the board.
        /// </summary>
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

        /// <summary>
        ///     Gets the total number of mines remaining (unflagged) on the board.
        /// </summary>
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

        /// <summary>
        ///     Gets the collection of <see cref="Tile"/>s that make up the board.
        /// </summary>
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

        /// <summary>
        ///     Gets the total amount of time that has elapsed since the game began.
        /// </summary>
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

        /// <summary>
        ///     Creates the a new IMinesweeper instance.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="mineCount">The mine count.</param>
        /// <returns>Minesweeper.</returns>
        /// <exception cref="System.ArgumentException"></exception>
        public static IMinesweeper Create(int width, int height, int mineCount)
        {
            var validationResult = MinesweeperBoardValidator.Create().ValidateBoard(width, height, mineCount);

            if (validationResult != null)
            {
                throw new ArgumentException(validationResult);
            }
        
            var minesweeper = new Minesweeper
            {
                tiles = TileCollection.Create(width, height),
                mineCount = mineCount,
                minesLeft = mineCount,
                timeElapsed = 0
            };

            return minesweeper;
        }

        protected void RaisePropertyChanged([CallerMemberName] string prop = "")
        {
            var propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }

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
