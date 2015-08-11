namespace WpfMinesweeper.ViewModels
{
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using Controls;
    using JonUtility;
    using Models;

    /// <summary>
    ///     Provides a base view model for a TileBoardView, containing all of the basic binding properties.
    /// </summary>
    public abstract class TileBoardViewModelBase : MinesweeperComponentViewModel
    {
        private static readonly Brush defaultHoverBrush = new SolidColorBrush(Color.FromArgb(110, 255, 255, 255));
        private static readonly Brush defaultSelectionBrush = new SolidColorBrush(Color.FromArgb(150, 150, 150, 150));
        private static readonly Brush defaultTileBrush = DefaultSettings.TileBrush;
        private static readonly Point<int> emptyPoint = new Point<int>(-1, -1);
        private ICommand boardInitializedCommand;
        private Brush hoverBrush = TileBoardViewModelBase.defaultHoverBrush;
        private Point<int> hoverTilePoint = TileBoardViewModelBase.emptyPoint;
        private bool isGameOver;
        private bool isTilePressed;
        private bool isVictory;
        private AnimatedTilesCollection selectedTiles;
        private Brush selectionBrush = TileBoardViewModelBase.defaultSelectionBrush;
        private Brush tileBrush = TileBoardViewModelBase.defaultTileBrush;
        private ICommand tileHoverCommand;
        private AnimatedTilesCollection tilesToUpdate;
        private ICommand tileTapCommand;

        protected TileBoardViewModelBase()
        {
            this.tileBrush = this.Settings.TileBrush;
        }

        public ICommand BoardInitializedCommand
        {
            get
            {
                return this.boardInitializedCommand;
            }
            set
            {
                if (this.boardInitializedCommand != value)
                {
                    this.boardInitializedCommand = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public Brush HoverBrush
        {
            get
            {
                return this.hoverBrush;
            }
            set
            {
                if (this.hoverBrush != value)
                {
                    this.hoverBrush = value ?? TileBoardViewModelBase.defaultHoverBrush;
                    this.OnPropertyChanged();
                }
            }
        }

        public Point<int> HoverTile
        {
            get
            {
                return this.hoverTilePoint;
            }
            set
            {
                if (this.hoverTilePoint != value)
                {
                    this.hoverTilePoint = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public bool IsGameOver
        {
            get
            {
                return this.isGameOver;
            }
            set
            {
                if (this.isGameOver != value)
                {
                    this.isGameOver = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public bool IsTilePressed
        {
            get
            {
                return this.isTilePressed;
            }
            set
            {
                if (this.isTilePressed != value)
                {
                    this.isTilePressed = value;
                    Mediator.Notify(
                        ViewModelMessages.UpdateSmileyIndex,
                        value ? SmileyState.TapDown : SmileyState.Default);
                    this.OnPropertyChanged();
                }
            }
        }

        public bool IsVictory
        {
            get
            {
                return this.isVictory;
            }
            set
            {
                if (this.isVictory != value)
                {
                    this.isVictory = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public AnimatedTilesCollection SelectedTiles
        {
            get
            {
                return this.selectedTiles;
            }
            set
            {
                if (this.selectedTiles != value)
                {
                    this.selectedTiles = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public Brush SelectionBrush
        {
            get
            {
                return this.selectionBrush;
            }
            set
            {
                if (this.selectionBrush != value)
                {
                    this.selectionBrush = value ?? TileBoardViewModelBase.defaultSelectionBrush;
                    this.OnPropertyChanged();
                }
            }
        }

        public Brush TileBrush
        {
            get
            {
                return this.tileBrush;
            }
            set
            {
                if (this.tileBrush != value)
                {
                    this.tileBrush = value;
                    this.Settings.TileBrush = value ?? TileBoardViewModelBase.defaultTileBrush;
                    this.OnPropertyChanged();
                }
            }
        }

        public ICommand TileHoverCommand
        {
            get
            {
                return this.tileHoverCommand;
            }
            set
            {
                if (this.tileHoverCommand != value)
                {
                    this.tileHoverCommand = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public AnimatedTilesCollection TilesToUpdate
        {
            get
            {
                return this.tilesToUpdate;
            }
            set
            {
                if (this.tilesToUpdate != value)
                {
                    this.tilesToUpdate = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public ICommand TileTapCommand
        {
            get
            {
                return this.tileTapCommand;
            }
            set
            {
                if (this.tileTapCommand != value)
                {
                    this.tileTapCommand = value;
                    this.OnPropertyChanged();
                }
            }
        }

        protected static Brush DefaultHoverBrush
        {
            get
            {
                return TileBoardViewModelBase.defaultHoverBrush;
            }
        }

        protected static Brush DefaultSelectionBrush
        {
            get
            {
                return TileBoardViewModelBase.defaultSelectionBrush;
            }
        }

        protected static Brush DefaultTileBrush
        {
            get
            {
                return TileBoardViewModelBase.defaultTileBrush;
            }
        }

        protected static Point<int> EmptyPoint
        {
            get
            {
                return TileBoardViewModelBase.emptyPoint;
            }
        }
    }
}
