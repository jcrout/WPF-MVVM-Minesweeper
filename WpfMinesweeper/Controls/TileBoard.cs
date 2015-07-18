﻿namespace WpfMinesweeper.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;
    using WpfMinesweeper.Models;

    class TileBoard : FrameworkElement
    {
        #region Fields
        private static Dictionary<TileType, ImageSource> images;
        private static IMinesweeper defaultMinesweeper = MinesweeperFactory.Create(9, 9, 10);
        private static Brush defaultHoverBrush = new SolidColorBrush(Color.FromArgb(100, 255, 150, 150));
        private static Brush defaultSelectionBrush = new SolidColorBrush(Color.FromArgb(100, 100, 100, 255));
        private static List<Point> defaultAnimatedTiles = new List<Point>();
        private static double defaultTileWidth = 16d;
        private static double defaultTileHeight = 16d;

        private List<Visual> visuals = new List<Visual>();
        private DrawingVisual boardVisual;
        private DrawingVisual animationVisual;
        private DrawingVisual mouseHoverVisual;
        private ImageSource tileUnsetImage;
        private ImageSource tileSetImage;
        private List<Point> selectedTilePoints;
        private List<Visual> selectedTileVisuals;

        private int mouseHoverIndex = -1;
        private bool initialized;
        private double tileWidth = 16d;
        private double tileHeight = 16d;
        #endregion

        #region Dependency Properties
        public static readonly RoutedEvent MovePlayedEvent = EventManager.RegisterRoutedEvent(
            "TileHoverChanged",
            RoutingStrategy.Bubble,
            typeof(Action<TileEventArgs>),
            typeof(TileBoard));

        public static readonly DependencyProperty MinesweeperProperty =
         DependencyProperty.Register(
             "Minesweeper",
             typeof(IMinesweeper),
             typeof(TileBoard),
             new FrameworkPropertyMetadata(
                 defaultMinesweeper,
                 MinesweeperChanged,
                 CoerceMinesweeper));

        public static readonly DependencyProperty AnimatedTilesProperty =
            DependencyProperty.Register(
                "AnimatedTiles",
                typeof(List<Point>),
                typeof(TileBoard),
                new FrameworkPropertyMetadata(
                    defaultAnimatedTiles,
                    AnimatedTilesChanged));

        public static readonly DependencyProperty HoverTileProperty =
            DependencyProperty.Register(
                "HoverTile",
                typeof(Point),
                typeof(TileBoard),
                new FrameworkPropertyMetadata(
                    new Point(-1, -1),
                    HoverTileChanged));
        public static readonly DependencyProperty TileColorBrushProperty =
            DependencyProperty.Register(
                "TileColorBrush",
                typeof(Brush),
                typeof(TileBoard),
                new FrameworkPropertyMetadata(
                    null,
                    TileColorBrushChanged));

        public static readonly DependencyProperty HoverBrushProperty =
            DependencyProperty.Register(
                "HoverBrush",
                typeof(Brush),
                typeof(TileBoard),
                new FrameworkPropertyMetadata(
                    defaultHoverBrush,
                    HoverBrushChanged,
                    CoerceHoverBrush));

        public static readonly DependencyProperty SelectionBrushProperty =
            DependencyProperty.Register(
                "SelectionBrush",
                typeof(Brush),
                typeof(TileBoard),
                new FrameworkPropertyMetadata(
                    defaultSelectionBrush,
                    SelectionBrushChanged,
                    CoerceSelectionBrush));

        public static readonly DependencyProperty BoardInitializedCommandProperty =
            DependencyProperty.Register(
                "BoardInitializedCommand",
                typeof(ICommand),
                typeof(TileBoard)
                );

        public static readonly DependencyProperty TileHoverCommandProperty =
            DependencyProperty.Register(
                "TileHoverCommand",
                typeof(ICommand),
                typeof(TileBoard)
                );

        public static readonly DependencyProperty TileTapCommandProperty =
             DependencyProperty.Register(
                "TileTapCommand",
                typeof(ICommand),
                typeof(TileBoard)
                );

        public static readonly DependencyProperty TilesToUpdateProperty =
             DependencyProperty.Register(
                "TilesToUpdate",
                typeof(AnimatedTilesCollection),
                typeof(TileBoard),
                new PropertyMetadata(
                    null,
                    TilesToUpdateChanged,
                    CoerceTilesToUpdate)
                );

        public static readonly DependencyProperty SelectedTilesProperty =
             DependencyProperty.Register(
                "SelectedTiles",
                typeof(AnimatedTilesCollection),
                typeof(TileBoard),
                new PropertyMetadata(
                    null,
                    SelectedTilesChanged)
                );

        #endregion

        static TileBoard()
        {
            images = new Dictionary<TileType, ImageSource>();
            for (int i = TileType.MineCountMinimum; i < TileType.MineCountMaximum + 1; i++)
            {
                images.Add(TileType.Number(i), new BitmapImage(new Uri("pack://application:,,,/WpfMinesweeper;component/Resources/Images/" + i.ToString() + ".png", UriKind.Absolute)));
            }
            images.Add(TileType.Mine, new BitmapImage(new Uri("pack://application:,,,/WpfMinesweeper;component/Resources/Images/Mine.png", UriKind.Absolute)));
        }

        private void CreateTileImage() // 189
        {
            int horizontalThickness = (int)Math.Floor(tileWidth / defaultTileWidth) + 1;
            int verticalThickness = (int)Math.Floor(tileHeight / defaultTileHeight) + 1;
            double actualWidth = tileWidth - 1d;
            double actualHeight = tileHeight - 1d;

            var tileBrush = new SolidColorBrush(Color.FromArgb(255, 150, 150, 150));
            var lightPen = new Pen(new SolidColorBrush(Color.FromArgb(255, 200, 200, 200)), 1d);
            var darkPen = new Pen(new SolidColorBrush(Color.FromArgb(255, 100, 100, 100)), 1d);

            var tileUnsetTarget = new RenderTargetBitmap((int)tileWidth, (int)tileHeight, 96, 96, PixelFormats.Pbgra32);
            var tileUnsetVisual = new DrawingVisual();

            using (var drawingContext = tileUnsetVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(tileBrush, null, new Rect(0, 0, tileWidth, tileHeight));
                for (int i = 0; i < horizontalThickness; i++)
                {
                    drawingContext.DrawLine(lightPen, new Point(i + 0.5, i + 0.5), new Point(actualWidth - (i * 2) + 0.5, i + 0.5));
                    drawingContext.DrawLine(darkPen, new Point(i + 0.5, actualHeight - i + 0.5), new Point(actualWidth - i + 0.5, actualHeight - i + 0.5));
                }

                for (int i = 0; i < verticalThickness; i++)
                {
                    drawingContext.DrawLine(lightPen, new Point(i + 0.5, i + 0.5), new Point(i + 0.5, actualHeight - (i * 2) + 0.5));
                    drawingContext.DrawLine(darkPen, new Point(actualWidth - i + 0.5, i + 0.5), new Point(actualWidth - i + 0.5, actualHeight - i + 0.5));
                }
            }

            var tileSetTarget = new RenderTargetBitmap((int)tileWidth, (int)tileHeight, 96, 96, PixelFormats.Pbgra32);
            var tileSetVisual = new DrawingVisual();

            using (var drawingContext = tileSetVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(new SolidColorBrush(Color.FromArgb(255, 189, 189, 189)), null, new Rect(0, 0, tileWidth, tileHeight));
                drawingContext.DrawLine(lightPen, new Point(0.5, 0.5), new Point(actualWidth - 0.5, 0.5));
                drawingContext.DrawLine(lightPen, new Point(0.5, 0.5), new Point(0.5, actualHeight + 0.5));
            }

            tileUnsetTarget.Render(tileUnsetVisual);
            tileSetTarget.Render(tileSetVisual);

            this.tileUnsetImage = tileUnsetTarget;
            this.tileSetImage = tileSetTarget;
        }

        private static void MinesweeperChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var board = (TileBoard)d;
            board.Initialize();
        }

        private static void AnimatedTilesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private static void HoverTileChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var board = (TileBoard)d;
            board.DrawMouseHover();
        }

        private static void HoverBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var board = (TileBoard)d;
            board.DrawMouseHover();
        }

        private static void SelectionBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var board = (TileBoard)d;
            board.DrawSelectedTiles();
        }

        private static void TileColorBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var board = (TileBoard)d;
            board.Initialize();
        }

        private static void TilesToUpdateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
            {
                return;
            }

            var board = (TileBoard)d;
            board.AnimateTiles((AnimatedTilesCollection)e.NewValue, true);
        }

        private static void SelectedTilesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var board = (TileBoard)d;
            var tiles = (AnimatedTilesCollection)e.NewValue;

            if (tiles == null || tiles.Tiles == null)
            {
                board.selectedTilePoints = null;
            }
            else
            {
                board.selectedTilePoints = tiles.Tiles;
            }

            board.AnimateTiles(tiles, false);
        }

        private static object CoerceMinesweeper(DependencyObject d, object baseValue)
        {
            if (baseValue == null)
            {
                return defaultMinesweeper;
            }

            var minesweeper = (IMinesweeper)baseValue;
            if (minesweeper.Tiles == null || minesweeper.Tiles.Width < 1 || minesweeper.Tiles.Height < 1)
            {
                return defaultMinesweeper;
            }

            return baseValue;
        }

        private static object CoerceHoverBrush(DependencyObject d, object baseValue)
        {
            if (baseValue == null)
            {
                return defaultHoverBrush;
            }

            return baseValue;
        }

        private static object CoerceSelectionBrush(DependencyObject d, object baseValue)
        {
            if (baseValue == null)
            {
                return defaultSelectionBrush;
            }

            return baseValue;
        }

        private static object CoerceTilesToUpdate(DependencyObject d, object baseValue)
        {
            if (baseValue == null)
            {
                return null;
            }

            return baseValue;
        }

        public TileBoard()
        {
            this.CreateTileImage();
            this.Initialize();
        }

        public IMinesweeper Minesweeper
        {
            get
            {
                return (IMinesweeper)this.GetValue(MinesweeperProperty);
            }
            set
            {
                this.SetValue(MinesweeperProperty, value);
            }
        }

        public List<Point> AnimatedTiles
        {
            get
            {
                return (List<Point>)this.GetValue(AnimatedTilesProperty);
            }
            set
            {
                this.SetValue(AnimatedTilesProperty, value);
            }
        }

        public Point HoverTile
        {
            get
            {
                return (Point)this.GetValue(HoverTileProperty);
            }
            set
            {
                this.SetValue(HoverTileProperty, value);
            }
        }

        public Brush HoverBrush
        {
            get
            {
                return (Brush)this.GetValue(HoverBrushProperty);
            }
            set
            {
                this.SetValue(HoverBrushProperty, value);
            }
        }

        public Brush SelectionBrush
        {
            get
            {
                return (Brush)this.GetValue(SelectionBrushProperty);
            }
            set
            {
                this.SetValue(SelectionBrushProperty, value);
            }
        }

        public AnimatedTilesCollection SelectedTiles
        {
            get
            {
                return (AnimatedTilesCollection)this.GetValue(SelectedTilesProperty);
            }
            set
            {
                this.SetValue(SelectedTilesProperty, value);
            }
        }

        public AnimatedTilesCollection TilesToUpdate
        {
            get
            {
                return (AnimatedTilesCollection)this.GetValue(TilesToUpdateProperty);
            }
            set
            {
                this.SetValue(TilesToUpdateProperty, value);
            }
        }

        public ICommand BoardInitializedCommand
        {
            get
            {
                return (ICommand)this.GetValue(BoardInitializedCommandProperty);
            }
            set
            {
                this.SetValue(BoardInitializedCommandProperty, value);
            }
        }

        public ICommand TileHoverCommand
        {
            get
            {
                return (ICommand)this.GetValue(TileHoverCommandProperty);
            }
            set
            {
                this.SetValue(TileHoverCommandProperty, value);
            }
        }

        public ICommand TileTapCommand
        {
            get
            {
                return (ICommand)this.GetValue(TileTapCommandProperty);
            }
            set
            {
                this.SetValue(TileTapCommandProperty, value);
            }
        }

        private void Initialize()
        {
            if (this.visuals.Count > 0)
            {
                this.visuals.ForEach(v => this.RemoveVisualChild(v));
                this.visuals.Clear();
            }

            this.Width = this.Minesweeper.Tiles.Width * this.tileWidth;
            this.Height = this.Minesweeper.Tiles.Height * this.tileHeight;

            this.DrawBoard();
            this.DrawTileShader();
            this.DrawMouseHover();

            this.visuals.ForEach(delegate(Visual v) { AddVisualChild(v); });
            //this.initialized = true;

            this.BoardInitializedCommand.ExecuteIfAbleTo(this.Width);
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return visuals.Count;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= visuals.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            return visuals[index];
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            this.TileHoverCommand.ExecuteIfAbleTo(
                new TileEventArgs(
                    Tile.Empty,
                    -1,
                    -1));
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var minesweeper = this.Minesweeper;
            var mouseCoordinates = e.GetPosition(this);
            int tileX = Math.Max(0, Math.Min((int)Math.Floor(mouseCoordinates.X / tileWidth), minesweeper.Tiles.Width - 1));
            int tileY = Math.Max(0, Math.Min((int)Math.Floor(mouseCoordinates.Y / tileHeight), minesweeper.Tiles.Height - 1));

            var newCoords = new Point(tileX, tileY);
            if (this.HoverTile != newCoords)
            {
                this.TileHoverCommand.ExecuteIfAbleTo(
                    new TileEventArgs(
                        this.Minesweeper.Tiles[tileX, tileY],
                        tileX,
                        tileY));
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.RaiseTileTap(e.GetPosition(this), InputButtons.Left, true, false, e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            this.RaiseTileTap(e.GetPosition(this), InputButtons.Left, false, false, e);
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);
            this.RaiseTileTap(e.GetPosition(this), InputButtons.Right, true, false, e);
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);
            this.RaiseTileTap(e.GetPosition(this), InputButtons.Right, false, false, e);
        }

        private TileEventArgs GetTileEventArgsFromBoardPoint(Point boardPoint)
        {
            if (boardPoint.X == -1)
            {
                return null;
            }
            else
            {
                var tilePoint = this.GetTileCoordinatesFromBoardPoint(boardPoint);
                int x = (int)tilePoint.X;
                int y = (int)tilePoint.Y;
                return new TileEventArgs(
                    this.Minesweeper.Tiles[x, y],
                    x,
                    y);
            }
        }

        private InputButtons GetStateOfAllMouseButtons(MouseButtonEventArgs e)
        {
            InputButtons allButtons = InputButtons.None;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                allButtons = allButtons | InputButtons.Left;
            }

            if (e.RightButton == MouseButtonState.Pressed)
            {
                allButtons = allButtons | InputButtons.Right;
            }

            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                allButtons = allButtons | InputButtons.Middle;
            }

            if (e.XButton1 == MouseButtonState.Pressed)
            {
                allButtons = allButtons | InputButtons.XButton1;
            }

            if (e.XButton2 == MouseButtonState.Pressed)
            {
                allButtons = allButtons | InputButtons.XButton2;
            }

            return allButtons;
        }

        private void RaiseTileTap(Point tapPoint, InputButtons button, bool pressedDown, bool doubleClicked, MouseButtonEventArgs e)
        {
            InputButtons allButtons = e != null ? this.GetStateOfAllMouseButtons(e) : InputButtons.None;

            this.TileTapCommand.ExecuteIfAbleTo(
                new TileTapEventArgs(
                    this.GetTileEventArgsFromBoardPoint(tapPoint),
                    button,
                    doubleClicked,
                    pressedDown,
                    allButtons));
        }

        private async void AnimateTiles(AnimatedTilesCollection animatedTilesCollection, bool updateTiles)
        {
            if (animatedTilesCollection != null && animatedTilesCollection.Frames != null && animatedTilesCollection.Frames.Count() > 0)
            {
                foreach (var frame in animatedTilesCollection.Frames)
                {
                    var animationVisual = new DrawingVisual();
                    using (var drawingContext = animationVisual.RenderOpen())
                    {
                        var image = frame.Image;
                        double offsetX = Math.Max(0, (this.tileWidth - image.Width) / 2);
                        double offsetY = Math.Max(0, (this.tileHeight - image.Height) / 2);

                        foreach (var tilePoint in animatedTilesCollection.Tiles)
                        {
                            var boardPoint = this.GetBoardCoordinatesFromTilePoint(new Point(tilePoint.X, tilePoint.Y));
                            var drawRect = new Rect(boardPoint.X + offsetX, boardPoint.Y + offsetY, image.Width, image.Height);
                            drawingContext.DrawImage(image, drawRect);
                        }

                        this.AddVisualChild(animationVisual);
                        await Task.Delay((int)frame.Interval).ConfigureAwait(true);
                        this.RemoveVisualChild(animationVisual);
                    }
                }
            }

            if (updateTiles)
            {
                if (animatedTilesCollection != null)
                {
                    this.DrawTiles(animatedTilesCollection.Tiles);
                }
            }
            else
            {
                this.DrawSelectedTiles();
            }
        }

        private void DrawTiles(List<Point> tileList)
        {
            int startIndex = this.visuals.Count;
            foreach (var tilePoint in tileList)
            {
                var tile = this.Minesweeper.Tiles[(int)tilePoint.X, (int)tilePoint.Y];
                var visual = new DrawingVisual();

                using (var drawingContext = visual.RenderOpen())
                {
                    if (!tile.Shown)
                    {
                        continue;
                    }

                    var tileRect = GetTileRectangleFromTilePoint(new Point(tilePoint.X, tilePoint.Y));
                    if (tile.Type != TileType.Mine)
                    {
                        drawingContext.DrawImage(this.tileSetImage, tileRect);
                    }

                    if (tile.Type != TileType.EmptySpace)
                    {
                        var imageToDraw = images[tile.Type];
                        double offsetX = Math.Max(0, (this.tileWidth - imageToDraw.Width) / 2);
                        double offsetY = Math.Max(0, (this.tileHeight - imageToDraw.Height) / 2);
                        drawingContext.DrawImage(imageToDraw, new Rect(tileRect.X + offsetX, tileRect.Y + offsetY, imageToDraw.Width, imageToDraw.Height));
                    }
                }
                this.visuals.Add(visual);
            }

            for (int i = startIndex; i < this.visuals.Count; i++)
            {
                this.AddVisualChild(this.visuals[i]);
            }
        }

        private Point GetTileCoordinatesFromBoardPoint(Point boardCoordinates)
        {
            return new Point(
                (int)Math.Max(0, Math.Min(Math.Floor(boardCoordinates.X / tileWidth), this.Minesweeper.Tiles.Width - 1)),
                (int)Math.Max(0, Math.Min(Math.Floor(boardCoordinates.Y / tileHeight), this.Minesweeper.Tiles.Height - 1)));
        }

        private Point GetBoardCoordinatesFromTilePoint(Point tileCoordinates)
        {
            return new Point(
                tileCoordinates.X * this.tileWidth,
                tileCoordinates.Y * this.tileHeight);
        }

        private Rect GetTileRectangleFromTilePoint(Point tileCoordinates)
        {
            return new Rect(
                tileCoordinates.X * this.tileWidth,
                tileCoordinates.Y * this.tileHeight,
                this.tileWidth,
                this.tileHeight);
        }

        private void RemoveVisualAfterTiles(Visual visual)
        {
            if (visual == null)
            {
                return;
            }

            int index = this.Minesweeper.Tiles.Width * this.Minesweeper.Tiles.Height;
            for (int i = index; i < this.visuals.Count; i++)
            {
                if (this.visuals[i] == visual)
                {
                    this.visuals.RemoveAt(i);
                    this.RemoveVisualChild(visual);
                }
            }
        }

        private void DrawMouseHover()
        {
            if (this.mouseHoverVisual != null)
            {
                this.visuals.RemoveAt(this.mouseHoverIndex);
                this.RemoveVisualChild(this.mouseHoverVisual);
            }

            var mouseHoverTileCoords = this.HoverTile;
            if (mouseHoverTileCoords.X < 0 || mouseHoverTileCoords.Y < 0)
            {
                this.mouseHoverVisual = null;
            }
            else
            {
                this.mouseHoverVisual = new DrawingVisual();
                var tileRect = this.GetTileRectangleFromTilePoint(mouseHoverTileCoords);

                using (var drawingContext = this.mouseHoverVisual.RenderOpen())
                {
                    drawingContext.DrawRectangle(
                        this.SelectionBrush,
                        null,
                        tileRect);
                }

                this.mouseHoverIndex = this.visuals.Count;
                this.visuals.Add(this.mouseHoverVisual);
                this.AddVisualChild(this.mouseHoverVisual);
            }
        }

        private void DrawSelectedTiles()
        {
            if (this.selectedTileVisuals != null)
            {
                this.selectedTileVisuals.ForEach(v =>
                {
                    visuals.Remove(v);
                    this.RemoveVisualChild(v);
                });

            }

            if (this.selectedTilePoints == null || this.selectedTilePoints.Count == 0)
            {
                return;
            }

            this.selectedTileVisuals = new List<Visual>(selectedTilePoints.Count);
            foreach (var tilePoint in this.selectedTilePoints)
            {
                var tileRect = this.GetTileRectangleFromTilePoint(tilePoint);
                var selectionVisual = new DrawingVisual();
                using (var drawingContext = selectionVisual.RenderOpen())
                {
                    drawingContext.DrawRectangle(this.SelectionBrush, null, tileRect);
                }

                this.selectedTileVisuals.Add(selectionVisual);
                this.visuals.Add(selectionVisual);
                this.AddVisualChild(selectionVisual);
            }
        }

        private void DrawTile(int tileX, int tileY)
        {
            var tile = this.Minesweeper.Tiles[tileX, tileY];
            var tileVisual = new DrawingVisual();
            using (var drawingContext = tileVisual.RenderOpen())
            {
                drawingContext.DrawImage(
                    tile.Shown ? this.tileSetImage : this.tileUnsetImage,
                    new Rect(
                        tileX * this.tileWidth,
                        tileY * this.tileHeight,
                        this.tileWidth,
                        this.tileHeight));
            }

            this.visuals.Add(tileVisual);
        }

        private void DrawBoard()
        {
            var tiles = this.Minesweeper.Tiles;
            for (int r = 0; r < tiles.Width; r++)
            {
                for (int c = 0; c < tiles.Height; c++)
                {
                    this.DrawTile(r, c);
                }
            }
        }

        private void DrawTileShader()
        {
            this.boardVisual = new DrawingVisual();
            var brushz = new LinearGradientBrush(Color.FromArgb(100, 255, 0, 0), Color.FromArgb(100, 0, 0, 255), new Point(0, 0), new Point(.75, .75));
            using (var drawingContext = boardVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(brushz, null, new Rect(0, 0, this.Width, this.Height));
            }

            this.visuals.Add(boardVisual);
        }
    }

    public class TileEventArgs : EventArgs
    {
        /// <summary>
        /// Targeted Tile.
        /// </summary>
        private Tile tile;

        /// <summary>
        /// Tile X.
        /// </summary>
        private int x;

        /// <summary>
        /// Tile Y.
        /// </summary>
        private int y;

        public TileEventArgs(Tile tile, int x, int y)
        {
            this.tile = tile;
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Gets the Tile.
        /// </summary>
        public Tile Tile
        {
            get
            {
                return this.tile;
            }
            private set
            {
                this.tile = value;
            }
        }

        /// <summary>
        /// Gets the Tile's X coordinate.
        /// </summary>
        public int X
        {
            get
            {
                return this.x;
            }
            private set
            {
                this.x = value;
            }
        }

        /// <summary>
        /// Gets the Tile's Y coordinate.
        /// </summary>
        public int Y
        {
            get
            {
                return this.y;
            }
            private set
            {
                this.y = value;
            }
        }
    }

    public class TileTapEventArgs
    {
        /// <summary>
        /// Backing field for Button.
        /// </summary>
        private InputButtons button;

        /// <summary>
        /// Backing field for Button.
        /// </summary>
        private InputButtons allButtons;

        /// <summary>
        /// Backing field for DoubleClicked.
        /// </summary>
        private bool doubleClicked;

        /// <summary>
        /// Backing field for PressedDown.
        /// </summary>
        private bool pressedDown;

        /// <summary>
        /// Backing field for TileEventArgs.
        /// </summary>
        private TileEventArgs tileEventArgs;

        public TileTapEventArgs(TileEventArgs tileEventArgs, InputButtons button, bool doubleClicked, bool pressedDown, InputButtons allButtons = InputButtons.None)
        {
            this.tileEventArgs = tileEventArgs;
            this.button = button;
            this.doubleClicked = doubleClicked;
            this.pressedDown = pressedDown;
            this.allButtons = allButtons;
        }

        /// <summary>
        /// Gets the input tap button.
        /// </summary>
        public InputButtons Button
        {
            get
            {
                return this.button;
            }
            private set
            {
                this.button = value;
            }
        }

        /// <summary>
        /// Gets the state of all mouse buttons. Inclusion means that the button is currently pressed down.
        /// </summary>
        public InputButtons AllButtonStates
        {
            get
            {
                return this.allButtons;
            }
            private set
            {
                this.allButtons = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether input was double-tapped.
        /// </summary>
        public bool DoubleClicked
        {
            get
            {
                return this.doubleClicked;
            }
            private set
            {
                this.doubleClicked = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether input was pressed down or released.
        /// </summary>
        public bool PressedDown
        {
            get
            {
                return this.pressedDown;
            }
            private set
            {
                this.pressedDown = value;
            }
        }

        /// <summary>
        /// Gets the associated TileEventArgs.
        /// </summary>
        public TileEventArgs TileEventArgs
        {
            get
            {
                return this.tileEventArgs;
            }
            private set
            {
                this.tileEventArgs = value;
            }
        }
    }

    public class AnimatedTilesCollection
    {
        private List<Point> tilesToUpdate;
        private TileAnimation frames;

        private AnimatedTilesCollection(List<Point> tilesToUpdate, TileAnimation frames = null)
        {
            this.tilesToUpdate = tilesToUpdate;
            this.frames = frames;
        }

        public static AnimatedTilesCollection Create(List<Point> tilesToUpdate, TileAnimation frames = null)
        {
            return new AnimatedTilesCollection(tilesToUpdate, frames);
        }

        public List<Point> Tiles
        {
            get
            {
                return this.tilesToUpdate;
            }
        }

        public TileAnimation Frames
        {
            get
            {
                return this.frames;
            }
        }

        public static AnimatedTilesCollection GetXPattern(ITileCollection tiles, int thickness, TileAnimation animation = null)
        {
            double slope = tiles.Height / tiles.Width;

            for (int r = 0; r < tiles.Width; r++)
            {
                int c = (int)(slope * (double)r);

            }

            var result = AnimatedTilesCollection.Create(null, animation);
            return null;
        }
    }

    public class TileAnimation : IEnumerable<AnimationFrame>
    {
        private static TileAnimation fadeInRectanglesAnimation;
        private IEnumerable<AnimationFrame> frames;

        static TileAnimation()
        {
            int frameCount = 3;
            double tileWidth = 16;
            double tileHeight = 16;
            double tileWidthHalf = (tileWidth / 2);
            double tileHeightHalf = (tileHeight / 2);
            double tileWidthIncrement = tileWidthHalf / frameCount;
            double tileHeightIncrement = tileHeightHalf / frameCount;


            ImageSource[] images = new ImageSource[frameCount];
            AnimationFrame[] frames = new AnimationFrame[frameCount];
            for (int i = 0; i < frameCount; i++)
            {
                double rectWidth = tileWidthHalf + (tileWidthIncrement * i);
                double rectHeight = tileHeightHalf + (tileHeightIncrement * i);

                var rectTarget = new RenderTargetBitmap((int)rectWidth, (int)rectHeight, 96, 96, PixelFormats.Pbgra32);
                var rectVisual = new DrawingVisual();

                byte rgbValue = (byte)(125 + (i * 50));

                using (var drawingContext = rectVisual.RenderOpen())
                {
                    drawingContext.DrawRectangle(
                        new SolidColorBrush(
                            Colors.DarkGray),
                            null,
                            new Rect(0, 0, rectWidth, rectHeight));
                }

                rectTarget.Render(rectVisual);
                images[i] = rectTarget;
                frames[i] = new AnimationFrame(images[i], 50);
            }

            fadeInRectanglesAnimation = new TileAnimation(frames);
        }

        public static TileAnimation FadeInRectangles
        {
            get
            {
                return fadeInRectanglesAnimation;
            }
        }

        public TileAnimation(IEnumerable<AnimationFrame> frames)
        {
            this.frames = frames;
        }

        public IEnumerator<AnimationFrame> GetEnumerator()
        {
            return this.frames.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.frames.GetEnumerator();
        }
    }

    public class AnimationFrame
    {
        public long Interval { get; set; } // milliseconds

        public ImageSource Image { get; set; }

        public AnimationFrame(ImageSource image, long interval)
        {
            this.Interval = interval;
            this.Image = image;
        }
    }

    /// <summary>
    /// Defines flags that specify buttons on an input device.
    /// </summary>
    [Flags]
    public enum InputButtons
    {
        /// <summary>
        /// No input buttons.
        /// </summary>
        None = 0,

        /// <summary>
        /// The left mouse button.
        /// </summary>
        Left = 1,

        /// <summary>
        /// The right mouse button.
        /// </summary>
        Right = 2,

        /// <summary>
        /// The middle mouse button (wheel).
        /// </summary>
        Middle = 4,

        /// <summary>
        /// The first extended mouse button.
        /// </summary>
        XButton1 = 8,

        /// <summary>
        /// The second extended mouse button.
        /// </summary>
        XButton2 = 16
    }
}
