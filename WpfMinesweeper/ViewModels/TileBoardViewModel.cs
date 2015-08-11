namespace WpfMinesweeper.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using Controls;
    using Models;
    using JonUtility;

    public class TileBoardViewModel : TileBoardViewModelBase
    {
        private static readonly Brush mineClickBrush = new SolidColorBrush(Color.FromArgb(80, 255, 0, 0));
        private static readonly Random randomGenerator = new Random();

        private readonly bool areQuestionMarksEnabled = true;
        private readonly int maxSafeSpotsAroundFirstClick = 5;
        private readonly int maxStackSize = 2000;
        private readonly int minSafeSpotsAroundFirstClick = 1;
        private bool boardInitialized;
        private int boardHeight;
        private int boardWidth;
        private int maxYCoordinate;
        private int maxXCoordinate;
        private bool leftAndRightMouseDown;
        private bool leftMouseDown;
        private int revealedSpaces;
        private Func<List<Point<int>>, Task> revealSurroundingTiles;
        private ITileCollection boardTiles;
        private int targetSpaceCount;
        private object syncLock = new object();

        public TileBoardViewModel()
        {
            this.TileHoverCommand = new Command(
                o => this.OnTileHover((TileEventArgs)o),
                this.CanInteractWithBoard);
            this.TileTapCommand = new Command(
                o => this.OnTileTap((TileTapEventArgs)o),
                this.CanInteractWithBoard);
            this.BoardInitializedCommand = new Command(this.OnTileBoardInitialized);

            var testGradientBrush = new LinearGradientBrush();
            testGradientBrush.GradientStops = new GradientStopCollection();
            testGradientBrush.GradientStops.Add(new GradientStop(Color.FromArgb(150, 135, 50, 160), 0d));
            testGradientBrush.GradientStops.Add(new GradientStop(Color.FromArgb(150, 170, 120, 20), .25d));
            testGradientBrush.GradientStops.Add(new GradientStop(Color.FromArgb(120, 0, 230, 23), .5d));
            testGradientBrush.GradientStops.Add(new GradientStop(Color.FromArgb(150, 120, 170, 250), .75d));
            testGradientBrush.GradientStops.Add(new GradientStop(Color.FromArgb(150, 20, 20, 70), 1d));

            this.TileBrush = testGradientBrush;

            Mediator.Notify(ViewModelMessages.TileColorsChanged, this.TileBrush);
            Mediator.Register(ViewModelMessages.TileColorsChanged, o => this.UpdateTileBrush((Brush)o));
        }

        protected override void OnMinesweeperChanged()
        {
            this.targetSpaceCount = (this.Minesweeper.Tiles.Width * this.Minesweeper.Tiles.Height) -
                                     this.Minesweeper.MineCount;
            this.boardTiles = this.Minesweeper.Tiles;
            this.boardInitialized = false;
            this.revealedSpaces = 0;
            this.leftAndRightMouseDown = false;

            this.SetRevealTileMethod();
            this.IsGameOver = false;
            this.IsVictory = false;
            this.SelectionBrush = TileBoardViewModelBase.DefaultSelectionBrush;
            this.HoverBrush = TileBoardViewModelBase.DefaultHoverBrush;
            this.IsTilePressed = false;
        }

        private bool CanInteractWithBoard()
        {
            return !this.IsGameOver && !this.IsVictory;
        }

        private void CheckFlagCountAndSurroundingTiles(TileEventArgs e)
        {
            var surroundingTiles = this.GetSurroundingTiles(e.X, e.Y);
            var flaggedTiles = (from tilePoint in surroundingTiles
                                let tile = this.Minesweeper.Tiles[(int)tilePoint.X, (int)tilePoint.Y]
                                where (!tile.Shown) && (tile.ExtraTileData == ExtraTileData.Flag)
                                select tilePoint).ToList();

            var mineCount = e.Tile.Type.Value;

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

                var tilesToCheck = new List<Point<int>>();
                foreach (var tilePoint in surroundingTiles)
                {
                    var tile = this.Minesweeper.Tiles[(int)tilePoint.X, (int)tilePoint.Y];

                    if (tile.Type == TileType.Unset && !tile.Shown && tile.ExtraTileData != ExtraTileData.Flag)
                    {
                        tilesToCheck.Add(tilePoint);
                    }
                }

                if (tilesToCheck.Count > 0)
                {
                    this.revealSurroundingTiles(tilesToCheck);
                }
            }
        }

        /// <summary>
        ///     Sets the current tile equal to the mine count, appends the list
        ///     of <paramref name="tiles" /> to update, and recursively checks
        ///     the surrounding <paramref name="tiles" /> if the current tile's
        ///     mine count is zero.
        /// </summary>
        /// <remarks>
        ///     This method checks each of the un-flipped surrounding
        ///     <paramref name="tiles" /> centered on the <paramref name="x" />
        ///     and
        ///     <paramref name="y" /> coordinate parameters. Each call adds the
        ///     tile point to the list. If there are zero mines surrounding this
        ///     coordinate, an empty tile is set and this method is called
        ///     recursively on each of the valid surrounding tilesToUpdate.
        /// </remarks>
        /// <param name="tiles">
        ///     The cumulative list of tile points to populate.
        /// </param>
        /// <param name="x">The X coordinate of the tile.</param>
        /// <param name="y">The Y coordinate of the tile.</param>
        private void CheckSurroundingTiles(List<Point<int>> tiles, int x, int y)
        {
            var count = 0;
            var left = x > 0 ? x - 1 : x;
            var right = x < this.Minesweeper.Tiles.Width - 1 ? x + 1 : x;
            var top = y > 0 ? y - 1 : y;
            var bottom = y < this.Minesweeper.Tiles.Height - 1 ? y + 1 : y;

            for (var r = left; r <= right; r++)
            {
                for (var c = top; c <= bottom; c++)
                {
                    if ((r != x || c != y) && this.Minesweeper.Tiles[r, c].Type == TileType.Mine)
                    {
                        count++;
                    }
                }
            }

            tiles.Add(new Point<int>(x, y));
            if (count > 0)
            {
                this.SetTile(x, y, new Tile(TileType.Number(count), true));
            }
            else
            {
                this.SetTile(x, y, new Tile(TileType.EmptySpace, true));
                for (var r = left; r <= right; r++)
                {
                    for (var c = top; c <= bottom; c++)
                    {
                        if (r != x || c != y)
                        {
                            var tile = this.Minesweeper.Tiles[r, c];
                            if (tile.Type == TileType.Unset && tile.ExtraTileData != ExtraTileData.Flag)
                            {
                                if (tile.Shown)
                                {
                                    Console.WriteLine("ee");
                                }
                                this.CheckSurroundingTiles(tiles, r, c);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Returns a list of surrounding <paramref name="tilesToUpdate" /> needing to be <see langword="checked" /> while adding to
        ///     the list of <paramref name="tilesToUpdate" /> needing updated.
        /// </summary>
        /// <remarks>
        ///     This method checks each of the un-flipped surrounding <paramref name="tilesToUpdate" /> centered on the
        ///     <paramref name="x" /> and <paramref name="y" /> coordinate parameters. Each call adds the tile point to the list.
        ///     The list of surrounding <paramref name="tilesToUpdate" /> is then returned.
        /// </remarks>
        /// <param name="tilesToUpdate">The cumulative list of tile points to populate.</param>
        /// <param name="x">The X coordinate of the tile.</param>
        /// <param name="y">The Y coordinate of the tile.</param>
        /// <returns>
        ///     This method then returns the list of <paramref name="tilesToUpdate" /> that need checked. An empty list is returned if
        ///     the current tile is a number. If the tile is an empty space, then it returns a list of surrounding
        ///     <paramref name="tilesToUpdate" /> that are not yet defined that haven't already been added to the cumulative list.
        /// </returns>
        private IEnumerable<Point<int>> CheckSurroundingTiles_NonRecursive(Point<int> tilePoint)
        {
            var surroundingTilePoints = this.GetSurroundingTilesPoints(tilePoint).ToArray();
            var count = surroundingTilePoints.Where(p => this.GetTile(p).Type == TileType.Mine).Count();

            if (count > 0)
            {
                this.SetTile(tilePoint, new Tile(TileType.Number(count), true));
                yield break;
            }
            else
            {
                this.SetTile(tilePoint, new Tile(TileType.EmptySpace, true));
                foreach (var pt in surroundingTilePoints)
                {
                    var tile = this.GetTile(pt);
                    if (tile.Type == TileType.Unset && !tile.Shown && tile.ExtraTileData != ExtraTileData.Flag)
                    {
                        this.SetTile(pt, new Tile(TileType.Unset, true));
                        yield return pt;
                    }
                }
            }
        }

        /// <summary>
        ///     This method is called on GameOver to end the game and reveal the mines.
        /// </summary>
        private void GameOver(int clickX, int clickY)
        {
            var list = new List<Point<int>>();

            for (var r = 0; r < this.Minesweeper.Tiles.Width; r++)
            {
                for (var c = 0; c < this.Minesweeper.Tiles.Height; c++)
                {
                    var tile = this.Minesweeper.Tiles[r, c];
                    if (tile.Type == TileType.Mine)
                    {
                        this.SetTile(r, c, new Tile(tile.Type, true));
                        list.Add(new Point<int>(r, c));
                    }
                }
            }

            this.IsGameOver = true;
            this.HoverTile = new Point<int>(-1, -1);

            this.TilesToUpdate = AnimatedTilesCollection.Create(list);
            if (clickX > -1 && clickY > 0)
            {
                this.SelectionBrush = TileBoardViewModel.mineClickBrush;
                var tile = this.Minesweeper.Tiles[clickX, clickY];

                // mine was clicked, causing game over
                if (tile.Type == TileType.Mine)
                {
                    this.SelectedTiles = AnimatedTilesCollection.Create(new List<Point<int>>(1) { new Point<int>(clickX, clickY) });
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

            Mediator.Notify(ViewModelMessages.GameOver);
        }

        /// <summary>
        ///     Generates the minefield centered around the first tile clicked.
        /// </summary>
        /// <remarks>
        ///     This method randomly determines the location of each mine on the board. The first tile clicked is excluded. An
        ///     additional set of tilesToUpdate may be added to the exclusion list depending on the minimum/maximum number of safe spots
        ///     around the clicked tile and also on the number of non-mine spaces available by calling the
        ///     <see cref="TileBoardViewModel.GetSafeTileIndexes" /> method.
        /// </remarks>
        /// <param name="clickX"></param>
        /// <param name="clickY"></param>
        private void GenerateMinefield(int clickX, int clickY)
        {
            var safeList = this.GetSafeTileIndexes(clickX, clickY);
            var spaceCount = this.Minesweeper.Tiles.Width * this.Minesweeper.Tiles.Height;
            var mineCount = this.Minesweeper.MineCount;
            var spaceList = new List<int>(spaceCount);

            var currentIndex = 0;
            safeList.Add(this.Minesweeper.Tiles.Width * this.Minesweeper.Tiles.Height);
            foreach (var targetIndex in safeList)
            {
                for (var i = currentIndex; i < targetIndex; i++)
                {
                    spaceList.Add(i);
                }

                currentIndex = targetIndex + 1;
            }

            for (var i = 0; i < mineCount; i++)
            {
                var randomIndex = TileBoardViewModel.randomGenerator.Next(0, spaceList.Count);
                var randomSpace = spaceList[randomIndex];
                spaceList.RemoveAt(randomIndex);
                var y = (int)Math.Floor((double)randomSpace / this.Minesweeper.Tiles.Width);
                var x = randomSpace % this.Minesweeper.Tiles.Width;
                this.boardTiles[x, y] = new Tile(TileType.Mine, false);
            }
        }

        /// <summary>
        ///     This method calculates and returns a list of safe tilesToUpdate around the first tile clicked. Safe tilesToUpdate are those that
        ///     are not mines and are adjacent to or within a specified radial range of the first tile clicked. This is done to
        ///     increase the odds of the first click containing more than just a single number and decrease the odds of the first
        ///     click being a high number like 8.
        /// </summary>
        /// <param name="clickX">The X coordinate of the first tile clicked.</param>
        /// <param name="clickY">The Y coordinate of the first tile clicked.</param>
        /// <returns></returns>
        private List<int> GetSafeTileIndexes(int clickX, int clickY)
        {
            var mineCount = this.Minesweeper.MineCount;
            var nonMineCount = this.Minesweeper.Tiles.Width * this.Minesweeper.Tiles.Height - mineCount;
            var safeSpotCount = this.minSafeSpotsAroundFirstClick;

            if (this.minSafeSpotsAroundFirstClick != this.maxSafeSpotsAroundFirstClick)
            {
                safeSpotCount = TileBoardViewModel.randomGenerator.Next(
                    this.minSafeSpotsAroundFirstClick,
                    this.maxSafeSpotsAroundFirstClick + 1);
            }

            safeSpotCount = Math.Min(safeSpotCount, nonMineCount - 1);
            if (safeSpotCount == 0)
            {
                return new List<int>(1) { (clickY * this.Minesweeper.Tiles.Width) + clickX };
            }

            var safeList = new List<int>(safeSpotCount) { (clickY * this.Minesweeper.Tiles.Width) + clickX };
            var surroundingTiles = this.GetSurroundingTiles(clickX, clickY, 1);
            var surroundingTileCount = surroundingTiles.Count - 1;
            for (var i = 0; i < safeSpotCount; i++)
            {
                var randomIndex = TileBoardViewModel.randomGenerator.Next(0, surroundingTiles.Count);
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

        private IEnumerable<Point<int>> GetSurroundingTilesPoints(Point<int> tilePoint)
        {
            int x = tilePoint.X;
            int y = tilePoint.Y;

            if (x > 0)
            {
                yield return new Point<int>(x - 1, y);

                if (y > 0)
                {
                    yield return new Point<int>(x - 1, y - 1);
                }

                if (y < this.maxYCoordinate)
                {
                    yield return new Point<int>(x - 1, y + 1);
                }
            }

            if (x < this.maxXCoordinate)
            {
                yield return new Point<int>(x + 1, y);
                if (y > 0)
                {
                    yield return new Point<int>(x + 1, y - 1);
                }

                if (y < this.maxYCoordinate)
                {
                    yield return new Point<int>(x + 1, y + 1);
                }
            }

            if (y > 0)
            {
                yield return new Point<int>(x, y - 1);
            }

            if (y < this.maxYCoordinate)
            {
                yield return new Point<int>(x, y + 1);
            }
        }

        private List<Point<int>> GetSurroundingTiles(int x, int y, int distance = 1)
        {
            var list = new List<Point<int>>();
            var left = x >= distance ? x - distance : x;
            var right = x < this.Minesweeper.Tiles.Width - distance ? x + distance : x;
            var top = y >= distance ? y - distance : y;
            var bottom = y < this.Minesweeper.Tiles.Height - distance ? y + distance : y;

            for (var r = left; r <= right; r++)
            {
                for (var c = top; c <= bottom; c++)
                {
                    if (r != x || c != y)
                    {
                        list.Add(new Point<int>(r, c));
                    }
                }
            }

            return list;
        }

        private Tile GetTile(Point<int> tilePoint)
        {
            return this.boardTiles[tilePoint.X, tilePoint.Y];
        }

        private void LeftDoubleClicked(TileEventArgs e)
        {
            if (!e.Tile.Shown || !e.Tile.Type.IsNumber())
            {
                return;
            }

            this.CheckFlagCountAndSurroundingTiles(e);
        }

        private void LeftRightDown(TileEventArgs e)
        {
            this.leftAndRightMouseDown = true;
            if (e.Tile.Type == TileType.EmptySpace)
            {
                return;
            }

            this.SelectSurroundingTiles(e);
        }

        private void OnTileBoardInitialized(object parameter)
        {
            if (this.Minesweeper.Tiles.Width != this.boardWidth ||
                this.Minesweeper.Tiles.Height != this.boardHeight)
            {
                Mediator.Notify(ViewModelMessages.TileBoardSizeChanged, parameter);

                this.boardWidth = this.boardTiles.Width;
                this.boardHeight = this.boardTiles.Height;
                this.maxXCoordinate = boardWidth - 1;
                this.maxYCoordinate = boardHeight - 1;
            }
        }

        private void OnTileHover(TileEventArgs e)
        {
            if (this.leftMouseDown && Mouse.LeftButton != MouseButtonState.Pressed)
            {
                this.IsTilePressed = false;
            }

            if (e.X < 0 || e.Y < 0)
            {
                this.HoverTile = TileBoardViewModelBase.EmptyPoint;
                return;
            }

            if (!e.Tile.Shown)
            {
                this.SelectedTiles = null;
                if (this.leftMouseDown && e.Tile.ExtraTileData != ExtraTileData.None)
                {
                    this.HoverTile = TileBoardViewModelBase.EmptyPoint;
                }
                else
                {
                    this.HoverTile = new Point<int>(e.X, e.Y);
                }
            }
            else
            {
                this.HoverTile = TileBoardViewModelBase.EmptyPoint;
                if (this.SelectedTiles != null)
                {
                    this.SelectSurroundingTiles(e);
                }
            }
        }

        private void OnTileTap(TileTapEventArgs e)
        {
            if (e.Button == InputButtons.Left && e.PressedDown)
            {
                this.leftMouseDown = true;
            }

            if (e.AllButtonStates.HasFlag(InputButtons.Left) && e.AllButtonStates.HasFlag(InputButtons.Right))
            {
                if (e.TileEventArgs.Tile.Shown)
                {
                    if (e.PressedDown)
                    {
                        this.LeftRightDown(e.TileEventArgs);
                    }
                }

                return;
            }

            if (this.SelectedTiles != null)
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
                    this.TileTapLeftDown(e.TileEventArgs);
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

        private async Task RevealSurroundingTiles(List<Point<int>> tilePoints)
        {
            var updateTileList = new List<Point<int>>();
            await Task.Run(() =>
            {
                // this.CheckSurroundingTiles(updateTileList, x, y)).

            }).ConfigureAwait(true);


            this.UpdateTileListAndCheckForVictory(updateTileList);
        }

        private async Task RevealSurroundingTiles2(List<Point<int>> tilePoints)
        {
            var updateTileList = new List<Point<int>>();
            await Task.Run(() =>
                {
                    var tilesToCheck = tilePoints;
                    do
                    {
                        var newList = new List<Point<int>>();
                        foreach (var point in tilesToCheck)
                        {
                            updateTileList.Add(new Point<int>(point.X, point.Y));
                            var newTilesToCheck = this.CheckSurroundingTiles_NonRecursive(point);
                            newList.AddRange(newTilesToCheck);
                        }

                        tilesToCheck = newList;
                    } while (tilesToCheck.Count > 0);
                }).ConfigureAwait(true);

            this.UpdateTileListAndCheckForVictory(updateTileList);
        }

        private void SelectSurroundingTiles(TileEventArgs e)
        {
            var tiles = (from tilePoint in this.GetSurroundingTiles(e.X, e.Y)
                         let tile = this.Minesweeper.Tiles[(int)tilePoint.X, (int)tilePoint.Y]
                         where (!tile.Shown) && (tile.ExtraTileData == ExtraTileData.None)
                         select tilePoint).ToList();

            this.SelectedTiles = tiles.Count > 0 ? AnimatedTilesCollection.Create(tiles) : null;
        }

        /// <summary>
        ///     Sets the <see cref="WpfMinesweeper.ViewModels.TileBoardViewModel.revealSurroundingTiles" /> delegate, depending on
        ///     the board size. If a stackoverflow exception is a possiblity due to a large number of tilesToUpdate, then a non-recursive
        ///     method is used. If it is not a possibility, then the faster recursive method is used.
        /// </summary>
        private void SetRevealTileMethod()
        {
            if (this.Minesweeper.Tiles.Width * this.Minesweeper.Tiles.Height < this.maxStackSize)
            {
                this.revealSurroundingTiles = this.RevealSurroundingTiles2;
            }
            else
            {
                this.revealSurroundingTiles = this.RevealSurroundingTiles2;
            }
        }

        private void SetTile(Point<int> tilePoint, Tile value)
        {
            this.SetTile(tilePoint.X, tilePoint.Y, value);
        }

        private void SetTile(int tileX, int tileY, Tile value)
        {
            lock (this.syncLock)
            {
                if (this.boardTiles[tileX, tileY].Type == TileType.Unset &&
                    (value.Type != TileType.Unset))
                {
                    this.revealedSpaces++;
                }
            }

            this.Minesweeper.Tiles[tileX, tileY] = value;
        }

        private void TileTapLeftDown(TileEventArgs e)
        {
            this.IsTilePressed = true;
        }

        private async void TileTapLeftUp(TileEventArgs e)
        {
            this.IsTilePressed = false;

            if (!this.leftMouseDown)
            {
                return;
            }
            this.leftMouseDown = false;

            if (!this.boardInitialized)
            {
                await Task.Run(() => this.GenerateMinefield(e.X, e.Y));
                this.GameStatistics[Statistic.GameStartTime] = DateTime.Now;
            }

            if (this.leftAndRightMouseDown)
            {
                this.CheckFlagCountAndSurroundingTiles(e);
                this.HoverTile = TileBoardViewModelBase.EmptyPoint;
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

                this.revealSurroundingTiles(new List<Point<int>>(1) { new Point<int>(e.X, e.Y) });
                this.HoverTile = TileBoardViewModelBase.EmptyPoint;
            }

            if (!this.boardInitialized && this.CanInteractWithBoard())
            {
                this.boardInitialized = true;
                Mediator.Notify(ViewModelMessages.GameStarted);
            }
        }

        private void TileTapRightDown(TileEventArgs e)
        {
            if (!this.boardInitialized)
            {
                return;
            }

            if (!e.Tile.Shown)
            {
                if (e.Tile.ExtraTileData == ExtraTileData.None)
                {
                    this.SetTile(e.X, e.Y, new Tile(e.Tile.Type, e.Tile.Shown, ExtraTileData.Flag));
                    this.Minesweeper.MinesRemaining--;
                    this.GameStatistics[Statistic.FlagsPlaced] = (int)this.GameStatistics[Statistic.FlagsPlaced] + 1;
                }
                else if (e.Tile.ExtraTileData == ExtraTileData.Flag)
                {
                    this.SetTile(
                        e.X,
                        e.Y,
                        new Tile(
                            e.Tile.Type,
                            e.Tile.Shown,
                            (this.areQuestionMarksEnabled) ? ExtraTileData.QuestionMark : ExtraTileData.None));
                    this.Minesweeper.MinesRemaining++;
                    this.GameStatistics[Statistic.FlagsPlaced] = (int)this.GameStatistics[Statistic.FlagsPlaced] - 1;
                }
                else // ExtraTileData.QuestionMark
                {
                    this.SetTile(e.X, e.Y, new Tile(e.Tile.Type, e.Tile.Shown, ExtraTileData.None));
                }

                this.TilesToUpdate = AnimatedTilesCollection.Create(new List<Point<int>>(1) { new Point<int>(e.X, e.Y) });
            }
        }

        private void TileTapRightUp(TileEventArgs e)
        {
            if (this.leftAndRightMouseDown)
            {
                this.CheckFlagCountAndSurroundingTiles(e);
            }
        }

        private void UpdateTileBrush(Brush brush)
        {
            this.TileBrush = brush;
        }

        private void UpdateTileListAndCheckForVictory(List<Point<int>> updateTileList)
        {
            this.GameStatistics[Statistic.Moves] = (int)this.GameStatistics[Statistic.Moves] + 1;
            this.TilesToUpdate = AnimatedTilesCollection.Create(updateTileList);

            if (this.revealedSpaces == this.targetSpaceCount)
            {
                this.Victory();
            }
        }

        private void Victory()
        {
            var list = new List<Point<int>>();
            for (var r = 0; r < this.Minesweeper.Tiles.Width; r++)
            {
                for (var c = 0; c < this.Minesweeper.Tiles.Height; c++)
                {
                    var tile = this.Minesweeper.Tiles[r, c];

                    if (tile.Type == TileType.Mine && tile.ExtraTileData != ExtraTileData.Flag)
                    {
                        this.SetTile(r, c, new Tile(TileType.Mine, false, ExtraTileData.Flag));
                        list.Add(new Point<int>(r, c));
                    }
                }
            }

            this.Minesweeper.MinesRemaining = 0;

            if (list.Count > 0)
            {
                this.TilesToUpdate = AnimatedTilesCollection.Create(list);
            }

            this.IsVictory = true;
            Mediator.Notify(ViewModelMessages.Victory);
        }
    }
}
