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
    using JonUtility;
    using System.Windows.Media;

    public class TileBoardViewModel : MinesweeperComponentViewModel
    {
        private static Brush defaultTileBrush = new SolidColorBrush(Color.FromArgb(255, 153, 217, 234));
        private static Brush defaultSelectionBrush = new SolidColorBrush(Color.FromArgb(150, 150, 150, 150));
        private static Brush defaultHoverBrush = new SolidColorBrush(Color.FromArgb(110, 255,255,255));
        private static Brush mineClickBrush = new SolidColorBrush(Color.FromArgb(80, 255, 0, 0));
        private static Random randomGenerator = new Random();
        private static Point emptyPoint = new Point(-1, -1);

        private ICommand tileHoverCommand;
        private ICommand tileTapCommand;
        private ICommand boardInitializedCommand;
        private TileShadingMode tileShadingMode = TileShadingMode.SingleTile;
        private AnimatedTilesCollection tilesToUpdate;
        private AnimatedTilesCollection selectedTiles;
        private Brush selectionBrush;
        private Brush hoverBrush;
        private Brush tileBrush;
        private Point hoverTilePoint = emptyPoint;
        private Func<int, int, Task> revealSurroundingTiles;
        private bool boardInitialized;
        private bool isGameOver;
        private bool isVictory;
        private bool areQuestionMarksEnabled = true;
        private bool leftAndRightMouseDown = false;
        private int minSafeSpotsAroundFirstClick = 1;
        private int maxSafeSpotsAroundFirstClick = 5;
        private int revealedSpaces = 0;
        private int targetSpaceCount = 0;
        private int maxStackSize = 3000;

        public TileBoardViewModel()
        {
            this.TileBrush = defaultTileBrush;
            this.TileHoverCommand = new Command(o => this.OnTileHover((Controls.TileEventArgs)o), () => this.CanInteractWithBoard());
            this.TileTapCommand = new Command(o => this.OnTileTap((Controls.TileTapEventArgs)o), () => this.CanInteractWithBoard());
            this.boardInitializedCommand = new Command(o => OnTileBoardInitialized(o));
        }

        /// <summary>
        /// Sets the revealSurroundingTiles delegate, depending on the board size. If a stackoverflow exception is a possiblity
        /// due to a large number of tiles, then a non-recursive method is used. If it is not a possibility, then the faster
        /// recursive method is used.
        /// </summary>
        private void SetRevealTileMethod()
        {
            if (this.Minesweeper.Tiles.Width * this.Minesweeper.Tiles.Height < maxStackSize)
            {
                this.revealSurroundingTiles = this.RevealSurroundingTiles;
            }
            else
            {
                this.revealSurroundingTiles = this.RevealSurroundingTiles2;
            }
        }

        protected override void OnMinesweeperChanged()
        {
            this.targetSpaceCount = (this.Minesweeper.Tiles.Width * this.Minesweeper.Tiles.Height) - this.Minesweeper.MineCount;
            this.SetRevealTileMethod();
            this.boardInitialized = false;
            this.IsGameOver = false;
            this.IsVictory = false;
            this.leftAndRightMouseDown = false;
            this.revealedSpaces = 0;
            this.SelectionBrush = defaultSelectionBrush;
            this.HoverBrush = defaultHoverBrush;
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

        public TileShadingMode TileShadingMode
        {
            get
            {
                return this.tileShadingMode;
            }

            set
            {
                if (this.tileShadingMode != value)
                {
                    this.tileShadingMode = value;
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
                    this.hoverBrush = value;
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
                    this.selectionBrush = value;
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

        private void OnTileBoardInitialized(object parameter)
        {
            Mediator.Instance.Notify(ViewModelMessages.TileBoardInitialized, parameter);
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
                else
                {
                    this.HoverTile = emptyPoint;
                }
            }
            else
            {
                this.HoverTile = emptyPoint;
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
                    this.TileTapLeftUp(e.TileEventArgs);
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
                    this.TileTapRightUp(e.TileEventArgs);
                    this.leftAndRightMouseDown = false;
                }
            }
        }

        private void LeftDoubleClicked(Controls.TileEventArgs e)
        {
            if (!e.Tile.Shown || !e.Tile.Type.IsNumber())
            {
                return;
            }

            this.CheckFlagCountAndSurroundingTiles(e);
        }

        private void CheckFlagCountAndSurroundingTiles(Controls.TileEventArgs e)
        {
            var surroundingTiles = this.GetSurroundingTiles(e.X, e.Y);
            var flaggedTiles = (from tilePoint in surroundingTiles
                                let tile = this.Minesweeper.Tiles[(int)tilePoint.X, (int)tilePoint.Y]
                                where (!tile.Shown) && (tile.ExtraTileData == ExtraTileData.Flag)
                                select tilePoint).ToList();

            int mineCount = e.Tile.Type.Value;

            if (flaggedTiles.Count == mineCount)
            {
                foreach (var tile in flaggedTiles)
                {
                    if (this.Minesweeper.Tiles[(int)tile.X, (int)tile.Y].Type != TileType.Mine)
                    {
                        this.GameOver(e.X, e.Y);
                        return;
                    }
                }

                foreach (var tilePoint in surroundingTiles)
                {
                    var tile = this.Minesweeper.Tiles[(int)tilePoint.X, (int)tilePoint.Y];
                    if (!tile.Shown && tile.ExtraTileData != ExtraTileData.Flag)
                    {
                        this.revealSurroundingTiles((int)tilePoint.X, (int)tilePoint.Y);
                    }
                }
            }
        }

        private void UpdateTileListAndCheckForVictory(List<Point> updateTileList)
        {
            this.revealedSpaces += updateTileList.Count;
            this.TilesToUpdate = AnimatedTilesCollection.Create(updateTileList);

            if (revealedSpaces == this.targetSpaceCount)
            {
                Victory();
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

        private async void TileTapLeftUp(Controls.TileEventArgs e)
        {
            if (!this.boardInitialized)
            {
                await Task.Run(() => this.GenerateMinefield(e.X, e.Y));
            }

            if (this.leftAndRightMouseDown)
            {
                this.CheckFlagCountAndSurroundingTiles(e);
                return;
            }

            if (e.Tile.Shown || e.Tile.ExtraTileData != ExtraTileData.None)
            {
                return;
            }

            if (e.Tile.Type == TileType.Mine)
            {
                this.GameOver(e.X, e.Y);
            }
            else
            {
                await this.revealSurroundingTiles(e.X, e.Y);
            }

            if (!this.boardInitialized && this.CanInteractWithBoard())
            {
                this.boardInitialized = true;
                Mediator.Instance.Notify(ViewModelMessages.GameStarted);
            }
        }

        private async Task RevealSurroundingTiles(int x, int y)
        {
            var updateTileList = new List<Point>();
            await Task.Run(() => this.CheckSurroundingTiles(updateTileList, x, y)).ConfigureAwait(true);
            this.UpdateTileListAndCheckForVictory(updateTileList);
        }

        private async Task RevealSurroundingTiles2(int x, int y)
        {
            long time1 = System.Diagnostics.Stopwatch.GetTimestamp();
            var updateTileList = new List<Point>();
            await Task.Run(() =>
            {
                List<Point> tilesToCheck = new List<Point> { new Point(x, y) };
                do
                {
                    var newList = new List<Point>();
                    foreach (var point in tilesToCheck)
                    {
                        newList.AddRange(this.CheckSurroundingTiles_NonRecursive(updateTileList, (int)point.X, (int)point.Y));
                    }

                    tilesToCheck = newList;
                }
                while (tilesToCheck.Count > 0);
            }).ConfigureAwait(true);

            if (!this.boardInitialized)
            {
                App.Tracer.TraceMethodTime(time1);
            }

            this.UpdateTileListAndCheckForVictory(updateTileList);
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
                    this.Minesweeper.Tiles[e.X, e.Y] = new Tile(
                        e.Tile.Type,
                        e.Tile.Shown,
                        ExtraTileData.Flag);
                    this.Minesweeper.MinesRemaining--;
                }
                else if (e.Tile.ExtraTileData == ExtraTileData.Flag)
                {
                    this.Minesweeper.Tiles[e.X, e.Y] = new Tile(
                        e.Tile.Type,
                        e.Tile.Shown,
                        (this.areQuestionMarksEnabled) ? ExtraTileData.QuestionMark : ExtraTileData.None);
                    this.Minesweeper.MinesRemaining++;
                }
                else // ExtraTileData.QuestionMark
                {
                    this.Minesweeper.Tiles[e.X, e.Y] = new Tile(
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

        private void TileTapRightUp(Controls.TileEventArgs e)
        {
            if (this.leftAndRightMouseDown)
            {
                this.CheckFlagCountAndSurroundingTiles(e);
                return;
            }
        }

        private bool CanInteractWithBoard()
        {
            return !this.isGameOver && !this.isVictory;
        }

        private void Victory()
        {
            var list = new List<Point>();
            for (int r = 0; r < this.Minesweeper.Tiles.Width; r++)
            {
                for (int c = 0; c < this.Minesweeper.Tiles.Height; c++)
                {
                    var tile = this.Minesweeper.Tiles[r, c];
                    if (tile.Type == TileType.Mine && tile.ExtraTileData != ExtraTileData.Flag)
                    {
                        this.Minesweeper.Tiles[r, c] = new Tile(TileType.Mine, false, ExtraTileData.Flag);
                        list.Add(new Point(r, c));
                    }
                }
            }

            this.Minesweeper.MinesRemaining = 0;
            this.IsVictory = true;

            if (list.Count > 0)
            {
                this.TilesToUpdate = AnimatedTilesCollection.Create(list);
            }
            Mediator.Instance.Notify(ViewModelMessages.Victory);
        }

        /// <summary>
        /// This method is called on GameOver, which occurs when a mine tile is left-clicked.
        /// </summary>
        private void GameOver(int clickX, int clickY)
        {
            var list = new List<Point>();

            for (int r = 0; r < this.Minesweeper.Tiles.Width; r++)
            {
                for (int c = 0; c < this.Minesweeper.Tiles.Height; c++)
                {
                    var tile = this.Minesweeper.Tiles[r, c];
                    if (tile.Type == TileType.Mine)
                    {
                        this.Minesweeper.Tiles[r, c] = new Tile(tile.Type, true);
                        list.Add(new Point(r, c));
                    }
                }
            }

            this.IsGameOver = true;
            this.HoverTile = new Point(-1, -1);

            this.TilesToUpdate = AnimatedTilesCollection.Create(list);
            if (clickX > -1 && clickY > 0)
            {
                this.SelectionBrush = mineClickBrush;
                var tile = this.Minesweeper.Tiles[clickX, clickY];

                // mine was clicked, causing game over
                if (tile.Type == TileType.Mine)
                {
                    this.SelectedTiles = AnimatedTilesCollection.Create(new List<Point>(1) { new Point(clickX, clickY) });
                }
                else
                {
                    var mineTiles = (from tilePoint in this.GetSurroundingTiles(clickX, clickY)
                                     let surroundingTile = this.Minesweeper.Tiles[(int)tilePoint.X, (int)tilePoint.Y]
                                     where (surroundingTile.Type == TileType.Mine)
                                     select tilePoint).ToList();
                    this.SelectedTiles = AnimatedTilesCollection.Create(mineTiles);
                }
            }

            Mediator.Instance.Notify(ViewModelMessages.GameOver);
        }

        /// <summary>
        /// Sets the current tile equal to the mine count, appends the list of tiles to update, and recursively checks the
        /// surrounding tiles if the current tile's mine count is zero.
        /// </summary>
        /// <remarks>
        /// This method checks each of the un-flipped surrounding tiles centered on the x and y coordinate parameters.
        /// Each call adds the tile point to the list. If there are zero mines surrounding this coordinate, an empty
        /// tile is set and this method is called recursively on each of the valid surrounding tiles.
        /// </remarks>
        /// <param name="tiles">The cumulative list of tile points to populate.</param>
        /// <param name="x">The X coordinate of the tile.</param>
        /// <param name="y">The Y coordinate of the tile.</param>
        private void CheckSurroundingTiles(List<Point> tiles, int x, int y)
        {
            int count = 0;
            int left = x > 0 ? x - 1 : x;
            int right = x < this.Minesweeper.Tiles.Width - 1 ? x + 1 : x;
            int top = y > 0 ? y - 1 : y;
            int bottom = y < this.Minesweeper.Tiles.Height - 1 ? y + 1 : y;

            for (int r = left; r <= right; r++)
            {
                for (int c = top; c <= bottom; c++)
                {
                    if ((r != x || c != y) && this.Minesweeper.Tiles[r, c].Type == TileType.Mine)
                    {
                        count++;
                    }
                }
            }

            tiles.Add(new Point(x, y));
            if (count > 0)
            {
                this.Minesweeper.Tiles[x, y] = new Tile(TileType.Number(count), true);
            }
            else
            {
                this.Minesweeper.Tiles[x, y] = new Tile(TileType.EmptySpace, true);
                for (int r = left; r <= right; r++)
                {
                    for (int c = top; c <= bottom; c++)
                    {
                        if (r != x || c != y)
                        {
                            var tile = this.Minesweeper.Tiles[r, c];
                            if (tile.Type == TileType.Unset)
                            {
                                this.CheckSurroundingTiles(tiles, r, c);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns a list of surrounding tiles needing to be checked while adding to the list of tiles needing updated.
        /// </summary>
        /// <returns>
        /// This method then returns the list of tiles that need checked. An empty list is returned if the current
        /// tile is a number. If the tile is an empty space, then it returns a list of surrounding tiles that
        /// are not yet defined that haven't already been added to the cumulative list.
        /// </returns>
        /// <remarks>
        /// This method checks each of the un-flipped surrounding tiles centered on the x and y coordinate parameters.
        /// Each call adds the tile point to the list. The list of surrounding tiles is then returned.
        /// </remarks>
        /// <param name="tiles">The cumulative list of tile points to populate.</param>
        /// <param name="x">The X coordinate of the tile.</param>
        /// <param name="y">The Y coordinate of the tile.</param>
        private List<Point> CheckSurroundingTiles_NonRecursive(List<Point> tiles, int x, int y)
        {
            var tilesToCheck = new List<Point>();
            tiles.Add(new Point(x, y));

            int count = 0;
            int left = x > 0 ? x - 1 : x;
            int right = x < this.Minesweeper.Tiles.Width - 1 ? x + 1 : x;
            int top = y > 0 ? y - 1 : y;
            int bottom = y < this.Minesweeper.Tiles.Height - 1 ? y + 1 : y;

            for (int r = left; r <= right; r++)
            {
                for (int c = top; c <= bottom; c++)
                {
                    if ((r != x || c != y) && this.Minesweeper.Tiles[r, c].Type == TileType.Mine)
                    {
                        count++;
                    }
                }
            }

            if (count > 0)
            {
                this.Minesweeper.Tiles[x, y] = new Tile(TileType.Number(count), true);
            }
            else
            {
                this.Minesweeper.Tiles[x, y] = new Tile(TileType.EmptySpace, true);
                for (int r = left; r <= right; r++)
                {
                    for (int c = top; c <= bottom; c++)
                    {
                        if (r != x || c != y)
                        {
                            var tile = this.Minesweeper.Tiles[r, c];
                            if (tile.Type == TileType.Unset && !tile.Shown)
                            {
                                this.Minesweeper.Tiles[r, c] = new Tile(TileType.Unset, true);
                                tilesToCheck.Add(new Point(r, c));
                            }
                        }
                    }
                }
            }

            return tilesToCheck;
        }

        private List<Point> GetSurroundingTiles(int x, int y, int distance = 1)
        {
            var list = new List<Point>();
            int left = x >= distance ? x - distance : x;
            int right = x < this.Minesweeper.Tiles.Width - distance ? x + distance : x;
            int top = y >= distance ? y - distance : y;
            int bottom = y < this.Minesweeper.Tiles.Height - distance ? y + distance : y;

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
                         let tile = this.Minesweeper.Tiles[(int)tilePoint.X, (int)tilePoint.Y]
                         where (!tile.Shown) && (tile.ExtraTileData == ExtraTileData.None)
                         select tilePoint).ToList();

            this.SelectedTiles = tiles.Count > 0 ? AnimatedTilesCollection.Create(tiles) : null;
        }

        /// <summary>
        /// This method calculates and returns a list of safe tiles around the first tile clicked. Safe tiles are those that
        /// are not mines and are adjacent to or within a specified radial range of the first tile clicked. This is done to
        /// increase the odds of the first click containing more than just a single number and decrease the odds of the first
        /// click being a high number like 8.
        /// </summary>
        /// <param name="clickX">The X coordinate of the first tile clicked.</param>
        /// <param name="clickY">The Y coordinate of the first tile clicked.</param>
        /// <returns></returns>
        private List<int> GetSafeTileIndexes(int clickX, int clickY)
        {
            int mineCount = this.Minesweeper.MineCount;
            int nonMineCount = this.Minesweeper.Tiles.Width * this.Minesweeper.Tiles.Height - mineCount;
            int safeSpotCount = this.minSafeSpotsAroundFirstClick;

            if (this.minSafeSpotsAroundFirstClick != this.maxSafeSpotsAroundFirstClick)
            {
                safeSpotCount = randomGenerator.Next(this.minSafeSpotsAroundFirstClick, this.maxSafeSpotsAroundFirstClick + 1);
            }

            safeSpotCount = Math.Min(safeSpotCount, nonMineCount - 1);
            if (safeSpotCount == 0)
            {
                return new List<int>(1) { (clickY * this.Minesweeper.Tiles.Width) + clickX };
            }

            var safeList = new List<int>(safeSpotCount) { (clickY * this.Minesweeper.Tiles.Width) + clickX };
            var surroundingTiles = this.GetSurroundingTiles(clickX, clickY, 1);
            int surroundingTileCount = surroundingTiles.Count - 1;
            for (int i = 0; i < safeSpotCount; i++)
            {
                int randomIndex = randomGenerator.Next(0, surroundingTiles.Count);
                var tilePoint = surroundingTiles[randomIndex];
                surroundingTiles.RemoveAt(randomIndex);

                safeList.Add((int)((tilePoint.Y * this.Minesweeper.Tiles.Width) + tilePoint.X));

                if (i == surroundingTileCount)
                {
                    surroundingTiles = this.GetSurroundingTiles(clickX, clickY, 2);
                }
            }

            safeList.Sort();
            return safeList;
        }

        /// <summary>
        /// Generates the minefield centered around the first tile clicked.
        /// </summary>
        /// <remarks>
        /// This method randomly determines the location of each mine on the board. The first tile clicked is excluded. An
        /// additional set of tiles may be added to the exclusion list depending on the minimum/maximum number of safe spots
        /// around the clicked tile and also on the number of non-mine spaces available by calling the GetSafeTileIndexes method.
        /// </remarks>
        /// <param name="clickX"></param>
        /// <param name="clickY"></param>
        private void GenerateMinefield(int clickX, int clickY)
        {
            long initialTime = System.Diagnostics.Stopwatch.GetTimestamp();
            var safeList = this.GetSafeTileIndexes(clickX, clickY);
            int spaceCount = this.Minesweeper.Tiles.Width * this.Minesweeper.Tiles.Height;
            int mineCount = this.Minesweeper.MineCount;
            int clickIndex = clickX * clickY;
            var spaceList = new List<int>(spaceCount);

            int currentIndex = 0;
            safeList.Add(this.Minesweeper.Tiles.Width * this.Minesweeper.Tiles.Height);
            for (int i = 0; i < safeList.Count; i++)
            {
                int targetIndex = safeList[i];
                for (int i2 = currentIndex; i2 < targetIndex; i2++)
                {
                    spaceList.Add(i2);
                }

                currentIndex = targetIndex + 1;
            }

            for (int i = 0; i < mineCount; i++)
            {
                int randomIndex = randomGenerator.Next(0, spaceList.Count);
                int randomSpace = spaceList[randomIndex];
                spaceList.RemoveAt(randomIndex);
                int y = (int)Math.Floor((double)randomSpace / this.Minesweeper.Tiles.Width);
                int x = randomSpace % this.Minesweeper.Tiles.Width;
                this.Minesweeper.Tiles[x, y] = new Tile(TileType.Mine, false);
            }

            App.Tracer.TraceMethodTime(initialTime);
        }
    }
}
