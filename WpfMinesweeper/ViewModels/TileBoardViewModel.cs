namespace WpfMinesweeper.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using WpfMinesweeper.Models;
    using WpfMinesweeper.Controls;
    using System.Windows.Input;
    using System.Timers;

    public class TileBoardViewModel : ViewModelBase
    {
        private static Random randomGenerator = new Random();
        private IMinesweeper minesweeper;
        private ICommand tileHoverCommand;
        private ICommand tileTapCommand;
        private ICommand boardInitializedCommand;
        private AnimatedTilesCollection tilesToUpdate;
        private AnimatedTilesCollection selectedTiles;
        private Point hoverTilePoint = new Point(-5, -5);
        private bool boardInitialized;
        private bool isGameOver;
        private bool areQuestionMarksEnabled = true;
        private bool leftAndRightMouseDown = false;

        public TileBoardViewModel(IMinesweeper minesweeper)
        {
            this.minesweeper = minesweeper;
            this.TileHoverCommand = new Command(o => this.OnTileHover((Controls.TileEventArgs)o), () => this.CanInteractWithBoard());
            this.TileTapCommand = new Command(o => this.OnTileTap((Controls.TileTapEventArgs)o), () => this.CanInteractWithBoard());
            this.boardInitializedCommand = new Command(o => OnSizeChaned(o));
        }

        public IMinesweeper Minesweeper
        {
            get
            {
                return this.minesweeper;
            }
            set
            {
                this.minesweeper = value;
                this.NewBoardAnimation();
            }
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

        public Point HoverTile
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

        private void OnSizeChaned(object parameter)
        {
            Mediator.Instance.Notify(ViewModelMessages.SizeChanged, parameter);
        }

        private void OnTileHover(Controls.TileEventArgs e)
        {
            if (!e.Tile.Shown)
            {
                this.SelectedTiles = null;
                if (e.X > -1 && e.Y > -1)
                {
                    this.HoverTile = new Point(e.X, e.Y);
                }
            }
            else
            {
                this.HoverTile = new Point(-1, -1);
                if (this.selectedTiles != null)
                {
                    this.SelectSurroundingTiles(e);
                }
            }
        }

        private void OnTileTap(Controls.TileTapEventArgs e)
        {
            if (e.AllButtonStates.HasFlag(InputButtons.Left) && e.AllButtonStates.HasFlag(InputButtons.Right))
            {
                if (e.TileEventArgs.Tile.Shown)
                {
                    if (e.PressedDown)
                    {
                        this.LeftRightDown(e.TileEventArgs);
                    }
                    else
                    {

                    }
                }

                return;
            }

            if (this.selectedTiles != null)
            {
                this.SelectedTiles = null;
            }

            if (e.Button == InputButtons.Left)
            {
                if (e.DoubleClicked)
                {
                    this.LeftDoubleClicked(e.TileEventArgs);
                }
                else if (!e.PressedDown)
                {
                    if (e.TileEventArgs.Tile.Shown)
                    {
                    }
                    else
                    {
                        this.TileTapLeftUp(e.TileEventArgs);
                    }
                    this.leftAndRightMouseDown = false;
                }
                else
                {

                }
            }
            else if (e.Button == InputButtons.Right)
            {
                if (e.PressedDown)
                {
                    this.TileTapRightDown(e.TileEventArgs);
                }
                else
                {
                    this.leftAndRightMouseDown = false;
                }
            }
        }

        private List<Point> GetSurroundingTiles(int x, int y)
        {
            var list = new List<Point>();
            int left = x > 0 ? x - 1 : x;
            int right = x < this.minesweeper.Tiles.Width - 1 ? x + 1 : x;
            int top = y > 0 ? y - 1 : y;
            int bottom = y < this.minesweeper.Tiles.Height - 1 ? y + 1 : y;

            for (int r = left; r <= right; r++)
            {
                for (int c = top; c <= bottom; c++)
                {
                    if (r != x || c != y)
                    {
                        list.Add(new Point(r, c));
                    }
                }
            }

            return list;
        }

        private void SelectSurroundingTiles(Controls.TileEventArgs e)
        {
            var tiles = (from tilePoint in this.GetSurroundingTiles(e.X, e.Y)
                         let tile = this.minesweeper.Tiles[(int)tilePoint.X, (int)tilePoint.Y]
                         where (!tile.Shown) && (tile.ExtraTileData == ExtraTileData.None)
                         select tilePoint).ToList();

            this.SelectedTiles = tiles.Count > 0 ? AnimatedTilesCollection.Create(tiles) : null;
        }

        private void LeftDoubleClicked(Controls.TileEventArgs e)
        {
            if (!e.Tile.Shown || !e.Tile.Type.IsNumber())
            {
                return;
            }

            var surroundingTiles = this.GetSurroundingTiles(e.X, e.Y);
            var tiles = (from tilePoint in surroundingTiles
                         let tile = this.minesweeper.Tiles[(int)tilePoint.X, (int)tilePoint.Y]
                         where (!tile.Shown) && (tile.ExtraTileData == ExtraTileData.Flag)
                         select tilePoint).ToList();

            int mineCount = e.Tile.Type.Value;

            if (tiles.Count == mineCount)
            {
                foreach (var tile in tiles)
                {
                    if (this.minesweeper.Tiles[(int)tile.X, (int)tile.Y].Type != TileType.Mine)
                    {
                        this.GameOver();
                        return;
                    }
                }

                foreach (var tilePoint in surroundingTiles)
                {
                    var tile = this.minesweeper.Tiles[(int)tilePoint.X, (int)tilePoint.Y];
                    if (!tile.Shown && tile.ExtraTileData != ExtraTileData.Flag)
                    {
                        var list = new List<Point>();
                        this.CheckSurroundingTiles(list, (int)tilePoint.X, (int)tilePoint.Y);
                        this.TilesToUpdate = AnimatedTilesCollection.Create(list);
                    }
                }
            }
        }

        private void LeftRightDown(Controls.TileEventArgs e)
        {
            this.leftAndRightMouseDown = true;
            if (e.Tile.Type == TileType.EmptySpace)
            {
                return;
            }

            this.SelectSurroundingTiles(e);
        }

        private void TileTapLeftDown(Controls.TileEventArgs e)
        {

        }

        private void TileTapLeftUp(Controls.TileEventArgs e)
        {
            if (!this.boardInitialized)
            {
                this.boardInitialized = true;
                this.GenerateMinefield(e.X, e.Y);
                Mediator.Instance.Notify(ViewModelMessages.GameStarted);
            }

            if (e.Tile.Shown || e.Tile.ExtraTileData != ExtraTileData.None)
            {
                return;
            }

            if (e.Tile.Type == TileType.Mine)
            {
                this.GameOver();
            }
            else
            {
                var list = new List<Point>();
                this.CheckSurroundingTiles(list, e.X, e.Y);
                this.TilesToUpdate = AnimatedTilesCollection.Create(list); //, TileAnimation.FadeInRectangles);
            }
        }

        private void TileTapRightDown(Controls.TileEventArgs e)
        {
            if (!this.boardInitialized)
            {
                return;
            }

            if (!e.Tile.Shown)
            {
                if (e.Tile.ExtraTileData == ExtraTileData.None)
                {
                    this.minesweeper.Tiles[e.X, e.Y] = new Tile(
                        e.Tile.Type,
                        e.Tile.Shown,
                        ExtraTileData.Flag);
                }
                else if (e.Tile.ExtraTileData == ExtraTileData.Flag)
                {
                    this.minesweeper.Tiles[e.X, e.Y] = new Tile(
                        e.Tile.Type,
                        e.Tile.Shown,
                        (this.areQuestionMarksEnabled) ? ExtraTileData.QuestionMark : ExtraTileData.None);
                }
                else
                {
                    this.minesweeper.Tiles[e.X, e.Y] = new Tile(
                        e.Tile.Type,
                        e.Tile.Shown,
                        ExtraTileData.None);
                }

                this.TilesToUpdate = AnimatedTilesCollection.Create(
                    new List<Point>(1) { 
                        new Point(e.X, e.Y) 
                    });
            }
        }

        private bool CanInteractWithBoard()
        {
            return !this.isGameOver;
        }

        /// <summary>
        /// This method is called on GameOver, which occurs when a mine tile is left-clicked.
        /// </summary>
        private void GameOver()
        {
            var list = new List<Point>();
            this.isGameOver = true;

            for (int r = 0; r < this.minesweeper.Tiles.Width; r++)
            {
                for (int c = 0; c < this.minesweeper.Tiles.Height; c++)
                {
                    var tile = this.minesweeper.Tiles[r, c];
                    if (tile.Type == TileType.Mine)
                    {
                        this.minesweeper.Tiles[r, c] = new Tile(tile.Type, true);
                        list.Add(new Point(r, c));
                    }
                }
            }

            this.HoverTile = new Point(-1, -1);
            this.TilesToUpdate = AnimatedTilesCollection.Create(list);
            Mediator.Instance.Notify(ViewModelMessages.GameOver);
            Mediator.Instance.Notify(ViewModelMessages.UpdateSmileyIndex, SmileyState.GameOver);
        }

        /// <summary>
        /// This method checks each of the un-flipped surrounding tiles centered on the x and y coordinate parameters.
        /// Each call adds the tile point to the list. If there are zero mines surrounding this coordinate, an empty
        /// tile is set and this method is called recursively on each of the valid surrounding tiles.
        /// </summary>
        /// <param name="tiles">The cumulative list of tile points to populate.</param>
        /// <param name="x">The X coordinate of the tile.</param>
        /// <param name="y">The Y coordinate of the tile.</param>
        private void CheckSurroundingTiles(List<Point> tiles, int x, int y)
        {
            int count = 0;
            int left = x > 0 ? x - 1 : x;
            int right = x < this.minesweeper.Tiles.Width - 1 ? x + 1 : x;
            int top = y > 0 ? y - 1 : y;
            int bottom = y < this.minesweeper.Tiles.Height - 1 ? y + 1 : y;

            for (int r = left; r <= right; r++)
            {
                for (int c = top; c <= bottom; c++)
                {
                    if ((r != x || c != y) && this.minesweeper.Tiles[r, c].Type == TileType.Mine)
                    {
                        count++;
                    }
                }
            }

            tiles.Add(new Point(x, y));
            if (count > 0)
            {
                this.minesweeper.Tiles[x, y] = new Tile(TileType.Number(count), true);
            }
            else
            {
                this.minesweeper.Tiles[x, y] = new Tile(TileType.EmptySpace, true);
                for (int r = left; r <= right; r++)
                {
                    for (int c = top; c <= bottom; c++)
                    {
                        if (r != x || c != y)
                        {
                            var tile = this.minesweeper.Tiles[r, c];
                            if (tile.Type == TileType.Unset)
                            {
                                CheckSurroundingTiles(tiles, r, c);
                            }
                        }
                    }
                }
            }
        }

        private void NewBoardAnimation()
        {

        }

        private void GenerateMinefield(int clickX, int clickY)
        {
            int spaceCount = this.minesweeper.Tiles.Width * this.minesweeper.Tiles.Height;
            int mineCount = this.minesweeper.MineCount;
            int clickIndex = clickX * clickY;
            var spaceList = new List<int>(spaceCount);

            for (int i = 0; i < clickIndex; i++)
            {
                spaceList.Add(i);
            }

            for (int i = clickIndex + 1; i < spaceCount; i++)
            {
                spaceList.Add(i);
            }

            for (int i = 0; i < mineCount; i++)
            {
                int randomIndex = randomGenerator.Next(0, spaceList.Count);
                int randomSpace = spaceList[randomIndex];
                spaceList.RemoveAt(randomIndex);
                double div = randomSpace / this.minesweeper.Tiles.Width;
                int y = (int)Math.Floor((double)randomSpace / this.minesweeper.Tiles.Width);
                int x = randomSpace % this.minesweeper.Tiles.Width;
                this.minesweeper.Tiles[x, y] = new Tile(TileType.Mine, false);
            }
        }
    }
}
