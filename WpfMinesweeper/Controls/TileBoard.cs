namespace WpfMinesweeper.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using JonUtility;
    using Models;
    using MouseEventArgs = System.Windows.Input.MouseEventArgs;

    /// <summary>
    ///     Defines flags that specify buttons on an input device.
    /// </summary>
    [Flags]
    public enum InputButtons
    {
        /// <summary>
        ///     No input buttons.
        /// </summary>
        None = 0,

        /// <summary>
        ///     The left mouse button.
        /// </summary>
        Left = 1,

        /// <summary>
        ///     The right mouse button.
        /// </summary>
        Right = 2,

        /// <summary>
        ///     The middle mouse button (wheel).
        /// </summary>
        Middle = 4,

        /// <summary>
        ///     The first extended mouse button.
        /// </summary>
        XButton1 = 8,

        /// <summary>
        ///     The second extended mouse button.
        /// </summary>
        XButton2 = 16
    }

    public enum TileShadingMode
    {
        SingleTile,
        AllTiles
    }

    public class AnimatedTilesCollection
    {
        private readonly TileAnimation frames;
        private readonly List<Point> tilesToUpdate;

        private AnimatedTilesCollection(List<Point> tilesToUpdate, TileAnimation frames = null)
        {
            this.tilesToUpdate = tilesToUpdate;
            this.frames = frames;
        }

        public TileAnimation Frames
        {
            get
            {
                return this.frames;
            }
        }

        public List<Point> Tiles
        {
            get
            {
                return this.tilesToUpdate;
            }
        }

        public static AnimatedTilesCollection Create(List<Point> tilesToUpdate, TileAnimation frames = null)
        {
            return new AnimatedTilesCollection(tilesToUpdate,
                frames);
        }
    }

    public class AnimationFrame
    {
        public AnimationFrame(ImageSource image, long interval)
        {
            this.Interval = interval;
            this.Image = image;
        }

        public ImageSource Image { get; set; }

        public long Interval { get; set; } // milliseconds
    }

    public class TileAnimation : IEnumerable<AnimationFrame>
    {
        private static readonly TileAnimation fadeInRectanglesAnimation;
        private readonly IEnumerable<AnimationFrame> frames;

        static TileAnimation()
        {
            var frameCount = 3;
            double tileWidth = 16;
            double tileHeight = 16;
            var tileWidthHalf = (tileWidth / 2);
            var tileHeightHalf = (tileHeight / 2);
            var tileWidthIncrement = tileWidthHalf / frameCount;
            var tileHeightIncrement = tileHeightHalf / frameCount;

            var images = new ImageSource[frameCount];
            var frames = new AnimationFrame[frameCount];
            for (var i = 0; i < frameCount; i++)
            {
                var rectWidth = tileWidthHalf + (tileWidthIncrement * i);
                var rectHeight = tileHeightHalf + (tileHeightIncrement * i);

                var rectTarget = new RenderTargetBitmap((int)rectWidth,
                    (int)rectHeight,
                    96,
                    96,
                    PixelFormats.Pbgra32);
                var rectVisual = new DrawingVisual();

                var rgbValue = (byte)(125 + (i * 50));

                using (var drawingContext = rectVisual.RenderOpen())
                {
                    drawingContext.DrawRectangle(
                        new SolidColorBrush(
                            Colors.DarkGray),
                        null,
                        new Rect(0,
                            0,
                            rectWidth,
                            rectHeight));
                }

                rectTarget.Render(
                    rectVisual);
                images[i] = rectTarget;
                frames[i] = new AnimationFrame(images[i],
                    50);
            }

            TileAnimation.fadeInRectanglesAnimation = new TileAnimation(frames);
        }

        public TileAnimation(IEnumerable<AnimationFrame> frames)
        {
            this.frames = frames;
        }

        public static TileAnimation FadeInRectangles
        {
            get
            {
                return TileAnimation.fadeInRectanglesAnimation;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.frames.GetEnumerator();
        }

        public IEnumerator<AnimationFrame> GetEnumerator()
        {
            return this.frames.GetEnumerator();
        }
    }

    [ContentProperty("Content")]
    public class TileBoard : FrameworkElement
    {
        private int moveCounter;

        static TileBoard()
        {
            var tileImages = TileBoard.CreateTileImage(
                TileBoard.defaultTileBrush,
                TileBoard.defaultTileSize);
            TileBoard.defaultTileUnsetImage = tileImages.Item1;
            TileBoard.defaultTileSetImage = tileImages.Item2;

            TileBoard.images = new Dictionary<TileType, ImageSource>();
            for (var i = 1; i < TileType.MineCountMaximum + 1; i++)
            {
                TileBoard.images.Add(
                    TileType.Number(
                        i),
                    new BitmapImage(new Uri("pack://application:,,,/WpfMinesweeper;component/Resources/Images/" + i + ".png",
                        UriKind.Absolute)));
            }
            TileBoard.images.Add(
                TileType.Mine,
                new BitmapImage(new Uri("pack://application:,,,/WpfMinesweeper;component/Resources/Images/Mine.png",
                    UriKind.Absolute)));
            TileBoard.flagImage = new BitmapImage(new Uri("pack://application:,,,/WpfMinesweeper;component/Resources/Images/Flag.png",
                UriKind.Absolute));
            TileBoard.questionMarkImage = new BitmapImage(new Uri("pack://application:,,,/WpfMinesweeper;component/Resources/Images/QuestionMark.png",
                UriKind.Absolute));
        }

        public List<Point> AnimatedTiles
        {
            get
            {
                return (List<Point>)this.GetValue(
                    TileBoard.AnimatedTilesProperty);
            }
            set
            {
                this.SetValue(
                    TileBoard.AnimatedTilesProperty,
                    value);
            }
        }

        public ICommand BoardInitializedCommand
        {
            get
            {
                return (ICommand)this.GetValue(
                    TileBoard.BoardInitializedCommandProperty);
            }
            set
            {
                this.SetValue(
                    TileBoard.BoardInitializedCommandProperty,
                    value);
            }
        }

        public object Content
        {
            get
            {
                return this.GetValue(
                    TileBoard.ContentProperty);
            }
            set
            {
                this.SetValue(
                    TileBoard.ContentProperty,
                    value);
            }
        }

        public bool DrawPressedTile
        {
            get
            {
                return (bool)this.GetValue(
                    TileBoard.DrawPressedTileProperty);
            }
            set
            {
                this.SetValue(
                    TileBoard.DrawPressedTileProperty,
                    value);
            }
        }

        public Brush HoverBrush
        {
            get
            {
                return (Brush)this.GetValue(
                    TileBoard.HoverBrushProperty);
            }
            set
            {
                this.SetValue(
                    TileBoard.HoverBrushProperty,
                    value);
            }
        }

        public Point HoverTile
        {
            get
            {
                return (Point)this.GetValue(
                    TileBoard.HoverTileProperty);
            }
            set
            {
                this.SetValue(
                    TileBoard.HoverTileProperty,
                    value);
            }
        }

        public IMinesweeper Minesweeper
        {
            get
            {
                return (IMinesweeper)this.GetValue(
                    TileBoard.MinesweeperProperty);
            }
            set
            {
                this.SetValue(
                    TileBoard.MinesweeperProperty,
                    value);
            }
        }

        public AnimatedTilesCollection SelectedTiles
        {
            get
            {
                return (AnimatedTilesCollection)this.GetValue(
                    TileBoard.SelectedTilesProperty);
            }
            set
            {
                this.SetValue(
                    TileBoard.SelectedTilesProperty,
                    value);
            }
        }

        public Brush SelectionBrush
        {
            get
            {
                return (Brush)this.GetValue(
                    TileBoard.SelectionBrushProperty);
            }
            set
            {
                this.SetValue(
                    TileBoard.SelectionBrushProperty,
                    value);
            }
        }

        public Brush TileBrush
        {
            get
            {
                return (Brush)this.GetValue(
                    TileBoard.TileBrushProperty);
            }
            set
            {
                this.SetValue(
                    TileBoard.TileBrushProperty,
                    value);
            }
        }

        public ICommand TileHoverCommand
        {
            get
            {
                return (ICommand)this.GetValue(
                    TileBoard.TileHoverCommandProperty);
            }
            set
            {
                this.SetValue(
                    TileBoard.TileHoverCommandProperty,
                    value);
            }
        }

        public TileShadingMode TileShadingMode
        {
            get
            {
                return (TileShadingMode)this.GetValue(
                    TileBoard.TileShadingModeProperty);
            }
            set
            {
                this.SetValue(
                    TileBoard.TileShadingModeProperty,
                    value);
            }
        }

        public Size TileSize
        {
            get
            {
                return (Size)this.GetValue(
                    TileBoard.TileSizeProperty);
            }
            set
            {
                this.SetValue(
                    TileBoard.TileSizeProperty,
                    value);
            }
        }

        public AnimatedTilesCollection TilesToUpdate
        {
            get
            {
                return (AnimatedTilesCollection)this.GetValue(
                    TileBoard.TilesToUpdateProperty);
            }
            set
            {
                this.SetValue(
                    TileBoard.TilesToUpdateProperty,
                    value);
            }
        }

        public ICommand TileTapCommand
        {
            get
            {
                return (ICommand)this.GetValue(
                    TileBoard.TileTapCommandProperty);
            }
            set
            {
                this.SetValue(
                    TileBoard.TileTapCommandProperty,
                    value);
            }
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return this.visuals.Count;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= this.visuals.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            return this.visuals[index];
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(
                e);
            this.TileHoverCommand.ExecuteIfAbleTo(
                new TileEventArgs(
                    new Tile(),
                    -1,
                    -1));
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(
                e);

            var doubleClicked = false;
            if (this.lastClickTime != -1)
            {
                var currentClickTime = Stopwatch.GetTimestamp();
                if (currentClickTime - this.lastClickTime <= TileBoard.doubleClickInterval)
                {
                    doubleClicked = true;
                }
                this.lastClickTime = -1;
            }
            else
            {
                this.lastClickTime = Stopwatch.GetTimestamp();
            }

            this.RaiseTileTap(
                e.GetPosition(
                    this),
                InputButtons.Left,
                true,
                doubleClicked,
                e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(
                e);
            this.RaiseTileTap(
                e.GetPosition(
                    this),
                InputButtons.Left,
                false,
                false,
                e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(
                e);

            var minesweeper = this.Minesweeper;
            var mouseCoordinates = e.GetPosition(
                this);
            var tileX = Math.Max(
                0,
                Math.Min(
                    (int)Math.Floor(
                        mouseCoordinates.X / this.TileSize.Width),
                    minesweeper.Tiles.Width - 1));
            var tileY = Math.Max(
                0,
                Math.Min(
                    (int)Math.Floor(
                        mouseCoordinates.Y / this.TileSize.Height),
                    minesweeper.Tiles.Height - 1));

            var newCoords = new Point(tileX,
                tileY);
            if (this.HoverTile != newCoords)
            {
                this.TileHoverCommand.ExecuteIfAbleTo(
                    new TileEventArgs(
                        this.Minesweeper.Tiles[tileX,
                            tileY],
                        tileX,
                        tileY));
            }
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(
                e);
            this.RaiseTileTap(
                e.GetPosition(
                    this),
                InputButtons.Right,
                true,
                false,
                e);
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(
                e);
            this.RaiseTileTap(
                e.GetPosition(
                    this),
                InputButtons.Right,
                false,
                false,
                e);
        }

        private static void AnimatedTilesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private static object CoerceHoverBrush(DependencyObject d, object baseValue)
        {
            if (baseValue == null)
            {
                return TileBoard.defaultHoverBrush;
            }

            return baseValue;
        }

        private static object CoerceMinesweeper(DependencyObject d, object baseValue)
        {
            if (baseValue == null)
            {
                return TileBoard.defaultMinesweeper;
            }

            var minesweeper = (IMinesweeper)baseValue;
            if (minesweeper.Tiles == null || minesweeper.Tiles.Width < 1 || minesweeper.Tiles.Height < 1)
            {
                return TileBoard.defaultMinesweeper;
            }

            return baseValue;
        }

        private static object CoerceSelectionBrush(DependencyObject d, object baseValue)
        {
            if (baseValue == null)
            {
                return TileBoard.defaultSelectionBrush;
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

        private static void ContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var board = (TileBoard)d;
        }

        private static Tuple<ImageSource, ImageSource> CreateTileImage(Brush tileBrush, Size tileSize) // 189
        {
            if (tileBrush == null)
            {
                throw new ArgumentNullException("tileBrush");
            }

            var tileWidth = tileSize.Width;
            var tileHeight = tileSize.Height;
            var actualWidth = tileWidth - 1d;
            var actualHeight = tileHeight - 1d;
            var horizontalThickness = (int)Math.Floor(
                tileWidth / tileWidth) + 1;
            var verticalThickness = (int)Math.Floor(
                tileHeight / tileHeight) + 1;

            Pen lightPen = null;
            Pen darkPen = null;

            if (tileBrush.GetType().IsAssignableFrom(
                typeof(SolidColorBrush)))
            {
                var solidColorBrush = tileBrush as SolidColorBrush;
                var tileColor = solidColorBrush.Color;
                lightPen = new Pen(new SolidColorBrush(tileColor.GetOffsetColor(
                    50)),
                    1d);
                darkPen = new Pen(new SolidColorBrush(tileColor.GetOffsetColor(
                    -50)),
                    1d);
            }
            else
            {
                lightPen = new Pen(new SolidColorBrush(Color.FromArgb(
                    255,
                    200,
                    200,
                    200)),
                    1d);
                darkPen = new Pen(new SolidColorBrush(Color.FromArgb(
                    255,
                    100,
                    100,
                    100)),
                    1d);
            }

            var tileUnsetTarget = new RenderTargetBitmap((int)tileWidth,
                (int)tileHeight,
                96,
                96,
                PixelFormats.Pbgra32);
            var tileUnsetVisual = new DrawingVisual();

            using (var drawingContext = tileUnsetVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(
                    tileBrush,
                    null,
                    new Rect(0,
                        0,
                        tileWidth,
                        tileHeight));
                for (var i = 0; i < horizontalThickness; i++)
                {
                    drawingContext.DrawLine(
                        lightPen,
                        new Point(i + 0.5,
                            i + 0.5),
                        new Point(actualWidth - (i * 2) + 0.5,
                            i + 0.5));
                    drawingContext.DrawLine(
                        darkPen,
                        new Point(i + 0.5,
                            actualHeight - i + 0.5),
                        new Point(actualWidth - i + 0.5,
                            actualHeight - i + 0.5));
                }

                for (var i = 0; i < verticalThickness; i++)
                {
                    drawingContext.DrawLine(
                        lightPen,
                        new Point(i + 0.5,
                            i + 0.5),
                        new Point(i + 0.5,
                            actualHeight - (i * 2) + 0.5));
                    drawingContext.DrawLine(
                        darkPen,
                        new Point(actualWidth - i + 0.5,
                            i + 0.5),
                        new Point(actualWidth - i + 0.5,
                            actualHeight - i + 0.5));
                }
            }

            var tileSetTarget = new RenderTargetBitmap((int)tileWidth,
                (int)tileHeight,
                96,
                96,
                PixelFormats.Pbgra32);
            var tileSetVisual = new DrawingVisual();
            //lightPen = new Pen(new SolidColorBrush(Color.FromArgb(255, 35, 35, 35)), 1.0);

            using (var drawingContext = tileSetVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(
                    new SolidColorBrush(Color.FromArgb(
                        255,
                        200,
                        213,
                        232)),
                    null,
                    new Rect(0,
                        0,
                        tileWidth,
                        tileHeight));
                drawingContext.DrawRectangle(
                    null,
                    new Pen(new SolidColorBrush(Color.FromArgb(
                        255,
                        223,
                        230,
                        235)),
                        1d),
                    new Rect(1.5,
                        1.5,
                        tileWidth - 2,
                        tileHeight - 2));
                drawingContext.DrawLine(
                    darkPen,
                    new Point(0.5,
                        0.5),
                    new Point(actualWidth + 1.5,
                        0.5));
                drawingContext.DrawLine(
                    darkPen,
                    new Point(0.5,
                        0.5),
                    new Point(0.5,
                        actualHeight + 1.5));
            }

            tileUnsetTarget.Render(
                tileUnsetVisual);
            tileSetTarget.Render(
                tileSetVisual);

            return new Tuple<ImageSource, ImageSource>(tileUnsetTarget,
                tileSetTarget);
        }

        private static void DrawPressedTileChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var board = (TileBoard)d;
            board.DrawMouseHover();
        }

        private static void HoverBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var board = (TileBoard)d;
            board.DrawMouseHover();
        }

        private static void HoverTileChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var board = (TileBoard)d;
            board.DrawMouseHover();
        }

        private static void MinesweeperChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var board = (TileBoard)d;
            board.InitializeBoard();
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

            board.AnimateTiles(
                tiles,
                false);
        }

        private static void SelectionBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var board = (TileBoard)d;
            board.DrawSelectedTiles();
        }

        private static void TileBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var board = (TileBoard)d;

            if (board.TileShadingMode == TileShadingMode.AllTiles)
            {
                board.DrawTileShader();
            }
            else
            {
                var tileImages = TileBoard.CreateTileImage(
                    board.TileBrush,
                    board.TileSize);
                board.tileUnsetImage = tileImages.Item1;
                board.tileSetImage = tileImages.Item2;
                board.RedrawBoard();
            }
        }

        private static void TileSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var board = (TileBoard)d;
            board.RedrawBoard();
        }

        private static void TilesToUpdateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
            {
                return;
            }

            var board = (TileBoard)d;
            board.AnimateTiles(
                (AnimatedTilesCollection)e.NewValue,
                true);
        }

        private static bool ValidateTileBrush(object value)
        {
            if (value == null)
            {
                return false;
            }

            return true;
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
                        var offsetX = Math.Max(
                            0,
                            (this.TileSize.Width - image.Width) / 2);
                        var offsetY = Math.Max(
                            0,
                            (this.TileSize.Height - image.Height) / 2);

                        foreach (var tilePoint in animatedTilesCollection.Tiles)
                        {
                            var boardPoint = this.GetBoardCoordinatesFromTilePoint(
                                new Point(tilePoint.X,
                                    tilePoint.Y));
                            var drawRect = new Rect(boardPoint.X + offsetX,
                                boardPoint.Y + offsetY,
                                image.Width,
                                image.Height);
                            drawingContext.DrawImage(
                                image,
                                drawRect);
                        }

                        this.AddVisualChild(
                            animationVisual);
                        await Task.Delay(
                            (int)frame.Interval).ConfigureAwait(
                                true);
                        this.RemoveVisualChild(
                            animationVisual);
                    }
                }
            }

            if (updateTiles)
            {
                if (animatedTilesCollection != null)
                {
                    this.DrawTiles(
                        animatedTilesCollection.Tiles);
                }
            }
            else
            {
                this.DrawSelectedTiles();
            }
        }

        private void DrawBoard()
        {
            var tiles = this.Minesweeper.Tiles;
            for (var r = 0; r < tiles.Width; r++)
            {
                for (var c = 0; c < tiles.Height; c++)
                {
                    this.DrawTile(
                        r,
                        c);
                }
            }
        }

        private void DrawImageWithOffsets(DrawingContext drawingContext, ImageSource imageToDraw, Rect tileRect)
        {
            var offsetX = Math.Max(
                0,
                (this.TileSize.Width - imageToDraw.Width) / 2);
            var offsetY = Math.Max(
                0,
                (this.TileSize.Height - imageToDraw.Height) / 2);
            drawingContext.DrawImage(
                imageToDraw,
                new Rect(tileRect.X + offsetX,
                    tileRect.Y + offsetY,
                    imageToDraw.Width,
                    imageToDraw.Height));
        }

        private void DrawMouseHover()
        {
            if (this.mouseHoverVisual != null)
            {
                this.visuals.Remove(
                    this.mouseHoverVisual);
                this.RemoveVisualChild(
                    this.mouseHoverVisual);
            }

            var mouseHoverTileCoords = this.HoverTile;
            if (mouseHoverTileCoords.X < 0 || mouseHoverTileCoords.Y < 0)
            {
                this.mouseHoverVisual = null;
            }
            else
            {
                this.mouseHoverVisual = new DrawingVisual();
                var tileRect = this.GetTileRectangleFromTilePoint(
                    mouseHoverTileCoords);

                using (var drawingContext = this.mouseHoverVisual.RenderOpen())
                {
                    if (this.DrawPressedTile)
                    {
                        drawingContext.DrawImage(
                            this.tileSetImage,
                            tileRect);
                    }
                    else
                    {
                        drawingContext.DrawRectangle(
                            this.HoverBrush,
                            null,
                            tileRect);
                    }
                }

                this.visuals.Add(
                    this.mouseHoverVisual);
                this.AddVisualChild(
                    this.mouseHoverVisual);
            }
        }

        private void DrawSelectedTiles()
        {
            if (this.selectedTileVisuals != null)
            {
                this.selectedTileVisuals.ForEach(
                    v =>
                    {
                        this.visuals.Remove(
                            v);
                        this.RemoveVisualChild(
                            v);
                    });
            }

            if (this.selectedTilePoints == null || this.selectedTilePoints.Count == 0)
            {
                return;
            }

            this.selectedTileVisuals = new List<Visual>(this.selectedTilePoints.Count);
            foreach (var tilePoint in this.selectedTilePoints)
            {
                var tileRect = this.GetTileRectangleFromTilePoint(
                    tilePoint);
                var selectionVisual = new DrawingVisual();
                using (var drawingContext = selectionVisual.RenderOpen())
                {
                    drawingContext.DrawRectangle(
                        this.SelectionBrush,
                        null,
                        tileRect);
                }

                this.selectedTileVisuals.Add(
                    selectionVisual);
                this.visuals.Add(
                    selectionVisual);
                this.AddVisualChild(
                    selectionVisual);
            }
        }

        private void DrawTile(int tileX, int tileY)
        {
            var tile = this.Minesweeper.Tiles[tileX,
                tileY];
            var tileVisual = new DrawingVisual();

            using (var drawingContext = tileVisual.RenderOpen())
            {
                var tileRect = this.GetTileRectangleFromTilePoint(
                    new Point(tileX,
                        tileY));
                drawingContext.DrawImage(
                    this.tileUnsetImage,
                    tileRect);
            }

            this.visuals.Add(
                tileVisual);
            this.AddVisualChild(
                tileVisual);
        }

        private void DrawTiles(List<Point> tileList)
        {
            var startIndex = this.visuals.Count;
            foreach (var tilePoint in tileList)
            {
                var index = (int)(this.Minesweeper.Tiles.Height * tilePoint.X + tilePoint.Y);
                var tile = this.Minesweeper.Tiles[(int)tilePoint.X,
                    (int)tilePoint.Y];
                var tileVisual = (DrawingVisual)this.visuals[index];
                var tileRect = this.GetTileRectangleFromTilePoint(
                    new Point(tilePoint.X,
                        tilePoint.Y));

                if (!tile.Shown)
                {
                    tileVisual.Children.Clear();
                    if (tile.ExtraTileData == ExtraTileData.None)
                    {
                        continue;
                    }

                    var visual = new DrawingVisual();
                    using (var drawingContext = visual.RenderOpen())
                    {
                        if (tile.ExtraTileData == ExtraTileData.Flag)
                        {
                            this.DrawImageWithOffsets(
                                drawingContext,
                                TileBoard.flagImage,
                                tileRect);
                        }
                        else
                        {
                            this.DrawImageWithOffsets(
                                drawingContext,
                                TileBoard.questionMarkImage,
                                tileRect);
                        }
                    }

                    tileVisual.Children.Add(
                        visual);
                }
                else
                {
                    var visual = new DrawingVisual();
                    using (var drawingContext = visual.RenderOpen())
                    {
                        if (tile.Type != TileType.Mine)
                        {
                            drawingContext.DrawImage(
                                this.tileSetImage,
                                tileRect);
                        }

                        if (tile.Type != TileType.EmptySpace)
                        {
                            this.DrawImageWithOffsets(
                                drawingContext,
                                TileBoard.images[tile.Type],
                                tileRect);
                        }
                    }

                    if (this.applyShadingToRevealedTiles)
                    {
                        tileVisual.Children.Add(
                            visual);
                    }
                    else
                    {
                        this.visuals.Add(
                            visual);
                        this.AddVisualChild(
                            visual);
                    }
                }
            }

            this.moveCounter++;
            WpfExtensionMethods.ConvertToJpeg(
                this,
                @"Y:\Downloads\Minesweeper_" + this.moveCounter + ".jpeg",
                96d);
        }

        private void DrawTileShader()
        {
            if (this.TileShadingMode != TileShadingMode.AllTiles)
            {
                return;
            }

            if (this.shaderVisual != null)
            {
                this.visuals.Remove(
                    this.shaderVisual);
                this.RemoveVisualChild(
                    this.shaderVisual);
            }

            this.shaderVisual = new DrawingVisual();
            //var brushz = new LinearGradientBrush(Color.FromArgb(100, 255, 0, 0), Color.FromArgb(100, 0, 0, 255), new Point(0, 0), new Point(.75, .75));
            using (var drawingContext = this.shaderVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(
                    this.TileBrush,
                    null,
                    new Rect(0,
                        0,
                        this.Width,
                        this.Height));
            }

            this.visuals.Add(
                this.shaderVisual);
            this.AddVisualChild(
                this.shaderVisual);
        }

        private Point GetBoardCoordinatesFromTilePoint(Point tileCoordinates)
        {
            return new Point(
                tileCoordinates.X * this.TileSize.Width,
                tileCoordinates.Y * this.TileSize.Height);
        }

        private InputButtons GetStateOfAllMouseButtons(MouseButtonEventArgs e)
        {
            var allButtons = InputButtons.None;

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

        private Point GetTileCoordinatesFromBoardPoint(Point boardCoordinates)
        {
            return new Point(
                (int)Math.Max(
                    0,
                    Math.Min(
                        Math.Floor(
                            boardCoordinates.X / this.TileSize.Width),
                        this.Minesweeper.Tiles.Width - 1)),
                (int)Math.Max(
                    0,
                    Math.Min(
                        Math.Floor(
                            boardCoordinates.Y / this.TileSize.Height),
                        this.Minesweeper.Tiles.Height - 1)));
        }

        private TileEventArgs GetTileEventArgsFromBoardPoint(Point boardPoint)
        {
            if (boardPoint.X == -1)
            {
                return null;
            }
            var tilePoint = this.GetTileCoordinatesFromBoardPoint(
                boardPoint);
            var x = (int)tilePoint.X;
            var y = (int)tilePoint.Y;
            return new TileEventArgs(
                this.Minesweeper.Tiles[x,
                    y],
                x,
                y);
        }

        private Rect GetTileRectangleFromTilePoint(Point tileCoordinates)
        {
            var tileSize = this.TileSize;
            return new Rect(
                tileCoordinates.X * tileSize.Width,
                tileCoordinates.Y * tileSize.Height,
                tileSize.Width,
                tileSize.Height);
        }

        private void InitializeBoard()
        {
            var skipRedraw = this.boardWidth == this.Minesweeper.Tiles.Width && this.boardHeight == this.Minesweeper.Tiles.Height;
            if (skipRedraw)
            {
                var lastTileIndex = this.boardWidth * this.boardHeight;
                var removeEnd = (this.shaderVisual == null) ? lastTileIndex - 1 : lastTileIndex;
                for (var i = this.visuals.Count - 1; i > removeEnd; i--)
                {
                    this.RemoveVisualChild(
                        this.visuals[i]);
                    this.visuals.RemoveAt(
                        i);
                }
                for (var i = 0; i < lastTileIndex; i++)
                {
                    ((DrawingVisual)this.visuals[i]).Children.Clear();
                }
            }
            else
            {
                this.visuals.ForEach(
                    v => this.RemoveVisualChild(
                        v));
                this.visuals.Clear();
                this.boardWidth = this.Minesweeper.Tiles.Width;
                this.boardHeight = this.Minesweeper.Tiles.Height;
                this.Width = this.boardWidth * this.TileSize.Width;
                this.Height = this.boardHeight * this.TileSize.Height;
                //this.RenderSize = new Size(this.Width, this.Height);

                this.DrawBoard();
                this.DrawTileShader();
                this.HoverTile = new Point(-1,
                    -1);
            }

            this.moveCounter = 0;
            this.BoardInitializedCommand.ExecuteIfAbleTo(
                new Size(this.Width,
                    this.Height));
        }

        private void RaiseTileTap(Point tapPoint, InputButtons button, bool pressedDown, bool doubleClicked, MouseButtonEventArgs e)
        {
            var allButtons = e != null ? this.GetStateOfAllMouseButtons(
                e) : InputButtons.None;

            this.TileTapCommand.ExecuteIfAbleTo(
                new TileTapEventArgs(
                    this.GetTileEventArgsFromBoardPoint(
                        tapPoint),
                    button,
                    doubleClicked,
                    pressedDown,
                    allButtons));
        }

        private void RedrawBoard()
        {
            if (this.visuals.Count == 0)
            {
                return;
            }

            this.visuals.ForEach(
                v => this.RemoveVisualChild(
                    v));
            this.visuals.Clear();
            this.DrawBoard();

            var allTilePoints = new List<Point>(this.boardWidth * this.boardHeight);
            for (double r = 0; r < this.boardWidth; r++)
            {
                for (double c = 0; c < this.boardHeight; c++)
                {
                    allTilePoints.Add(
                        new Point(r,
                            c));
                }
            }

            this.DrawTiles(
                allTilePoints);
        }

        private void RemoveVisualAfterTiles(Visual visual)
        {
            if (visual == null)
            {
                return;
            }

            var index = this.Minesweeper.Tiles.Width * this.Minesweeper.Tiles.Height;
            for (var i = index; i < this.visuals.Count; i++)
            {
                if (object.ReferenceEquals(
                    this.visuals[i],
                    visual))
                {
                    this.visuals.RemoveAt(
                        i);
                    this.RemoveVisualChild(
                        visual);
                }
            }
        }

        #region Fields
        private static readonly IMinesweeper defaultMinesweeper = MinesweeperFactory.Create(
            9,
            9,
            10);

        private static readonly Dictionary<TileType, ImageSource> images;
        private static readonly ImageSource defaultTileUnsetImage;
        private static readonly ImageSource defaultTileSetImage;
        private static readonly ImageSource flagImage;
        private static readonly ImageSource questionMarkImage;

        private static readonly Brush defaultHoverBrush = new SolidColorBrush(Color.FromArgb(
            100,
            255,
            150,
            150));

        private static readonly Brush defaultSelectionBrush = new SolidColorBrush(Color.FromArgb(
            100,
            50,
            50,
            255));

        private static readonly Brush defaultTileBrush = new SolidColorBrush(Color.FromArgb(
            255,
            153,
            217,
            234));

        private static readonly Size defaultTileSize = new Size(16d,
            16d);

        private static readonly List<Point> defaultAnimatedTiles = new List<Point>();
        private static readonly long doubleClickInterval = SystemInformation.DoubleClickTime * Stopwatch.Frequency / 1000;

        private readonly List<Visual> visuals = new List<Visual>();
        private DrawingVisual shaderVisual;
        private DrawingVisual mouseHoverVisual;
        private List<Point> selectedTilePoints;
        private List<Visual> selectedTileVisuals;
        private ImageSource tileUnsetImage = TileBoard.defaultTileUnsetImage;
        private ImageSource tileSetImage = TileBoard.defaultTileSetImage;

        private int boardWidth;
        private int boardHeight;

        private long lastClickTime = -1;
        private readonly bool applyShadingToRevealedTiles = false;
        #endregion

        #region Dependency Properties
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
                "Content",
                typeof(object),
                typeof(TileBoard),
                new FrameworkPropertyMetadata(
                    null,
                    TileBoard.ContentChanged));

        public static readonly DependencyProperty MinesweeperProperty =
            DependencyProperty.Register(
                "Minesweeper",
                typeof(IMinesweeper),
                typeof(TileBoard),
                new FrameworkPropertyMetadata(
                    TileBoard.defaultMinesweeper,
                    TileBoard.MinesweeperChanged,
                    TileBoard.CoerceMinesweeper));

        public static readonly DependencyProperty TileSizeProperty =
            DependencyProperty.Register(
                "TileSize",
                typeof(Size),
                typeof(TileBoard),
                new FrameworkPropertyMetadata(
                    TileBoard.defaultTileSize,
                    TileBoard.TileSizeChanged));

        public static readonly DependencyProperty AnimatedTilesProperty =
            DependencyProperty.Register(
                "AnimatedTiles",
                typeof(List<Point>),
                typeof(TileBoard),
                new FrameworkPropertyMetadata(
                    TileBoard.defaultAnimatedTiles,
                    TileBoard.AnimatedTilesChanged));

        public static readonly DependencyProperty HoverTileProperty =
            DependencyProperty.Register(
                "HoverTile",
                typeof(Point),
                typeof(TileBoard),
                new FrameworkPropertyMetadata(
                    new Point(-1,
                        -1),
                    TileBoard.HoverTileChanged));

        public static readonly DependencyProperty DrawPressedTileProperty =
            DependencyProperty.Register(
                "DrawPressedTile",
                typeof(bool),
                typeof(TileBoard),
                new FrameworkPropertyMetadata(
                    false,
                    TileBoard.DrawPressedTileChanged));

        public static readonly DependencyProperty TileBrushProperty =
            DependencyProperty.Register(
                "TileBrush",
                typeof(Brush),
                typeof(TileBoard),
                new FrameworkPropertyMetadata(
                    TileBoard.defaultTileBrush,
                    TileBoard.TileBrushChanged),
                TileBoard.ValidateTileBrush);

        public static readonly DependencyProperty HoverBrushProperty =
            DependencyProperty.Register(
                "HoverBrush",
                typeof(Brush),
                typeof(TileBoard),
                new FrameworkPropertyMetadata(
                    TileBoard.defaultHoverBrush,
                    TileBoard.HoverBrushChanged,
                    TileBoard.CoerceHoverBrush));

        public static readonly DependencyProperty SelectionBrushProperty =
            DependencyProperty.Register(
                "SelectionBrush",
                typeof(Brush),
                typeof(TileBoard),
                new FrameworkPropertyMetadata(
                    TileBoard.defaultSelectionBrush,
                    TileBoard.SelectionBrushChanged,
                    TileBoard.CoerceSelectionBrush));

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

        public static readonly DependencyProperty TileShadingModeProperty =
            DependencyProperty.Register(
                "TileShadingMode",
                typeof(TileShadingMode),
                typeof(TileBoard),
                new PropertyMetadata(TileShadingMode.SingleTile)
                );

        public static readonly DependencyProperty TilesToUpdateProperty =
            DependencyProperty.Register(
                "TilesToUpdate",
                typeof(AnimatedTilesCollection),
                typeof(TileBoard),
                new PropertyMetadata(
                    null,
                    TileBoard.TilesToUpdateChanged,
                    TileBoard.CoerceTilesToUpdate)
                );

        public static readonly DependencyProperty SelectedTilesProperty =
            DependencyProperty.Register(
                "SelectedTiles",
                typeof(AnimatedTilesCollection),
                typeof(TileBoard),
                new PropertyMetadata(
                    null,
                    TileBoard.SelectedTilesChanged)
                );
        #endregion
    }

    public class TileEventArgs : EventArgs
    {
        public TileEventArgs(Tile tile, int x, int y)
        {
            this.Tile = tile;
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        ///     Gets the <see cref="Tile" /> associated with the event.
        /// </summary>
        public Tile Tile { get; private set; }

        /// <summary>
        ///     Gets the Tile's <see cref="X" /> coordinate.
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        ///     Gets the Tile's <see cref="Y" /> coordinate.
        /// </summary>
        public int Y { get; private set; }
    }

    public class TileTapEventArgs
    {
        public TileTapEventArgs(TileEventArgs tileEventArgs, InputButtons button, bool doubleClicked, bool pressedDown, InputButtons allButtons = InputButtons.None)
        {
            this.TileEventArgs = tileEventArgs;
            this.Button = button;
            this.DoubleClicked = doubleClicked;
            this.PressedDown = pressedDown;
            this.AllButtonStates = allButtons;
        }

        /// <summary>
        ///     Gets the state of all mouse buttons. Inclusion means that the
        ///     <see cref="WpfMinesweeper.Controls.TileTapEventArgs.Button" /> is currently pressed down.
        /// </summary>
        public InputButtons AllButtonStates { get; private set; }

        /// <summary>
        ///     Gets the input tap button.
        /// </summary>
        public InputButtons Button { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether input was double-tapped.
        /// </summary>
        public bool DoubleClicked { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether input was pressed down or released.
        /// </summary>
        public bool PressedDown { get; private set; }

        /// <summary>
        ///     Gets the associated TileEventArgs.
        /// </summary>
        public TileEventArgs TileEventArgs { get; private set; }
    }
}
