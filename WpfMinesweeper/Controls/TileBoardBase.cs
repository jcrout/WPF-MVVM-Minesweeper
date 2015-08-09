namespace WpfMinesweeper.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using JonUtility;
    using Models;

    [ContentProperty("Content"), DebuggerStepThrough]
    public class TileBoardBase : FrameworkElement
    {
        private static readonly Brush defaultHoverBrush = new SolidColorBrush(Color.FromArgb(100, 255, 150, 150));
        private static readonly IMinesweeper defaultMinesweeper = MinesweeperFactory.Create(9, 9, 10);
        private static readonly Brush defaultSelectionBrush = new SolidColorBrush(Color.FromArgb(100, 50, 50, 255));
        private static readonly Brush defaultTileBrush = new SolidColorBrush(Color.FromArgb(255, 153, 217, 234));
        private static readonly BitmapSource defaultTileSetImage;
        private static readonly Size defaultTileSize = new Size(16d, 16d);
        private static readonly BitmapSource defaultTileUnsetImage;
        private static readonly BitmapSource flagImage;
        private static readonly Dictionary<TileType, BitmapSource> tileTypeImages;
        private static readonly BitmapSource questionMarkImage;

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content",
            typeof(object),
            typeof(TileBoardBase),
            new FrameworkPropertyMetadata(null, TileBoardBase.ContentChanged));

        public static readonly DependencyProperty MinesweeperProperty = DependencyProperty.Register(
            "Minesweeper",
            typeof(IMinesweeper),
            typeof(TileBoardBase),
            new FrameworkPropertyMetadata(
                TileBoardBase.defaultMinesweeper,
                TileBoardBase.MinesweeperChanged,
                TileBoardBase.CoerceMinesweeper));

        public static readonly DependencyProperty TileSizeProperty = DependencyProperty.Register(
            "TileSize",
            typeof(Size),
            typeof(TileBoardBase),
            new FrameworkPropertyMetadata(TileBoardBase.defaultTileSize, TileBoardBase.TileSizeChanged));

        public static readonly DependencyProperty HoverTileProperty = DependencyProperty.Register(
            "HoverTile",
            typeof(Point<int>),
            typeof(TileBoardBase),
            new FrameworkPropertyMetadata(new Point<int>(-1, -1), TileBoardBase.HoverTileChanged));

        public static readonly DependencyProperty DrawPressedTileProperty =
            DependencyProperty.Register(
                "DrawPressedTile",
                typeof(bool),
                typeof(TileBoardBase),
                new FrameworkPropertyMetadata(false, TileBoardBase.DrawPressedTileChanged));

        public static readonly DependencyProperty TileBrushProperty = DependencyProperty.Register(
            "TileBrush",
            typeof(Brush),
            typeof(TileBoardBase),
            new FrameworkPropertyMetadata(
                TileBoardBase.defaultTileBrush,
                TileBoardBase.TileBrushChanged,
                TileBoardBase.CoerceTileBrush));

        public static readonly DependencyProperty HoverBrushProperty = DependencyProperty.Register(
            "HoverBrush",
            typeof(Brush),
            typeof(TileBoardBase),
            new FrameworkPropertyMetadata(
                TileBoardBase.defaultHoverBrush,
                TileBoardBase.HoverBrushChanged,
                TileBoardBase.CoerceHoverBrush));

        public static readonly DependencyProperty SelectionBrushProperty = DependencyProperty.Register(
            "SelectionBrush",
            typeof(Brush),
            typeof(TileBoardBase),
            new FrameworkPropertyMetadata(
                TileBoardBase.defaultSelectionBrush,
                TileBoardBase.SelectionBrushChanged,
                TileBoardBase.CoerceSelectionBrush));

        public static readonly DependencyProperty BoardInitializedCommandProperty =
            DependencyProperty.Register("BoardInitializedCommand", typeof(ICommand), typeof(TileBoardBase));

        public static readonly DependencyProperty TileHoverCommandProperty =
            DependencyProperty.Register("TileHoverCommand", typeof(ICommand), typeof(TileBoardBase));

        public static readonly DependencyProperty TileTapCommandProperty = DependencyProperty.Register(
            "TileTapCommand",
            typeof(ICommand),
            typeof(TileBoardBase));

        public static readonly DependencyProperty TilesToUpdateProperty = DependencyProperty.Register(
            "TilesToUpdate",
            typeof(AnimatedTilesCollection),
            typeof(TileBoardBase),
            new PropertyMetadata(null, TileBoardBase.TilesToUpdateChanged));

        public static readonly DependencyProperty SelectedTilesProperty = DependencyProperty.Register(
            "SelectedTiles",
            typeof(AnimatedTilesCollection),
            typeof(TileBoardBase),
            new PropertyMetadata(null, TileBoardBase.SelectedTilesChanged));

        static TileBoardBase()
        {
            var tileImages = TileBoardBase.CreateTileImage(
                TileBoardBase.defaultTileBrush,
                TileBoardBase.defaultTileSize);
            TileBoardBase.defaultTileUnsetImage = tileImages.Item1;
            TileBoardBase.defaultTileSetImage = tileImages.Item2;

            TileBoardBase.tileTypeImages = new Dictionary<TileType, BitmapSource>();
            for (var i = 1; i < TileType.MineCountMaximum + 1; i++)
            {
                TileBoardBase.tileTypeImages.Add(
                    TileType.Number(i),
                    new BitmapImage(
                        new Uri(
                            "pack://application:,,,/WpfMinesweeper;component/Resources/Images/" + i + ".png",
                            UriKind.Absolute)));
            }

            TileBoardBase.tileTypeImages.Add(
                TileType.Mine,
                new BitmapImage(
                    new Uri(
                        "pack://application:,,,/WpfMinesweeper;component/Resources/Images/Mine.png",
                        UriKind.Absolute)));
            TileBoardBase.flagImage =
                new BitmapImage(
                    new Uri(
                        "pack://application:,,,/WpfMinesweeper;component/Resources/Images/Flag.png",
                        UriKind.Absolute));
            TileBoardBase.questionMarkImage =
                new BitmapImage(
                    new Uri(
                        "pack://application:,,,/WpfMinesweeper;component/Resources/Images/QuestionMark.png",
                        UriKind.Absolute));
        }

        public ICommand BoardInitializedCommand
        {
            get
            {
                return (ICommand)this.GetValue(TileBoardBase.BoardInitializedCommandProperty);
            }
            set
            {
                this.SetValue(TileBoardBase.BoardInitializedCommandProperty, value);
            }
        }

        public object Content
        {
            get
            {
                return this.GetValue(TileBoardBase.ContentProperty);
            }
            set
            {
                this.SetValue(TileBoardBase.ContentProperty, value);
            }
        }

        public bool DrawPressedTile
        {
            get
            {
                return (bool)this.GetValue(TileBoardBase.DrawPressedTileProperty);
            }
            set
            {
                this.SetValue(TileBoardBase.DrawPressedTileProperty, value);
            }
        }

        public Brush HoverBrush
        {
            get
            {
                return (Brush)this.GetValue(TileBoardBase.HoverBrushProperty);
            }
            set
            {
                this.SetValue(TileBoardBase.HoverBrushProperty, value);
            }
        }

        public Point<int> HoverTile
        {
            get
            {
                return (Point<int>)this.GetValue(TileBoardBase.HoverTileProperty);
            }
            set
            {
                this.SetValue(TileBoardBase.HoverTileProperty, value);
            }
        }

        public IMinesweeper Minesweeper
        {
            get
            {
                return (IMinesweeper)this.GetValue(TileBoardBase.MinesweeperProperty);
            }
            set
            {
                this.SetValue(TileBoardBase.MinesweeperProperty, value);
            }
        }

        public AnimatedTilesCollection SelectedTiles
        {
            get
            {
                return (AnimatedTilesCollection)this.GetValue(TileBoardBase.SelectedTilesProperty);
            }
            set
            {
                this.SetValue(TileBoardBase.SelectedTilesProperty, value);
            }
        }

        public Brush SelectionBrush
        {
            get
            {
                return (Brush)this.GetValue(TileBoardBase.SelectionBrushProperty);
            }
            set
            {
                this.SetValue(TileBoardBase.SelectionBrushProperty, value);
            }
        }

        public Brush TileBrush
        {
            get
            {
                return (Brush)this.GetValue(TileBoardBase.TileBrushProperty);
            }
            set
            {
                this.SetValue(TileBoardBase.TileBrushProperty, value);
            }
        }

        public ICommand TileHoverCommand
        {
            get
            {
                return (ICommand)this.GetValue(TileBoardBase.TileHoverCommandProperty);
            }
            set
            {
                this.SetValue(TileBoardBase.TileHoverCommandProperty, value);
            }
        }

        public Size TileSize
        {
            get
            {
                return (Size)this.GetValue(TileBoardBase.TileSizeProperty);
            }
            set
            {
                this.SetValue(TileBoardBase.TileSizeProperty, value);
            }
        }

        public AnimatedTilesCollection TilesToUpdate
        {
            get
            {
                return (AnimatedTilesCollection)this.GetValue(TileBoardBase.TilesToUpdateProperty);
            }
            set
            {
                this.SetValue(TileBoardBase.TilesToUpdateProperty, value);
            }
        }

        public ICommand TileTapCommand
        {
            get
            {
                return (ICommand)this.GetValue(TileBoardBase.TileTapCommandProperty);
            }
            set
            {
                this.SetValue(TileBoardBase.TileTapCommandProperty, value);
            }
        }

        protected static Brush DefaultHoverBrush
        {
            get
            {
                return TileBoardBase.defaultHoverBrush;
            }
        }

        protected static Brush DefaultSelectionBrush
        {
            get
            {
                return TileBoardBase.defaultSelectionBrush;
            }
        }

        protected static Brush DefaultTileBrush
        {
            get
            {
                return TileBoardBase.defaultTileBrush;
            }
        }

        protected static BitmapSource DefaultTileSetImage
        {
            get
            {
                return TileBoardBase.defaultTileSetImage;
            }
        }

        protected static BitmapSource DefaultTileUnsetImage
        {
            get
            {
                return TileBoardBase.defaultTileUnsetImage;
            }
        }

        protected static BitmapSource FlagImage
        {
            get
            {
                return TileBoardBase.flagImage;
            }
        }

        protected static Dictionary<TileType, BitmapSource> TileTypeImages
        {
            get
            {
                return TileBoardBase.tileTypeImages;
            }
        }

        protected static BitmapSource QuestionMarkImage
        {
            get
            {
                return TileBoardBase.questionMarkImage;
            }
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return this.Visuals.Count;
            }
        }

        protected List<Visual> Visuals { get; } = new List<Visual>();

        protected static Tuple<RenderTargetBitmap, RenderTargetBitmap> CreateTileImage(Brush tileBrush, Size tileSize)
        {
            if (tileBrush == null)
            {
                throw new ArgumentNullException("tileBrush");
            }

            var tileWidth = tileSize.Width;
            var tileHeight = tileSize.Height;
            var actualWidth = tileWidth - 1d;
            var actualHeight = tileHeight - 1d;
            var horizontalThickness = (int)Math.Floor(tileWidth / tileWidth) + 1;
            var verticalThickness = (int)Math.Floor(tileHeight / tileHeight) + 1;

            Pen lightPen;
            Pen darkPen;

            if (tileBrush is SolidColorBrush)
            {
                var solidColorBrush = (SolidColorBrush)tileBrush;
                var tileColor = solidColorBrush.Color;
                lightPen = new Pen(new SolidColorBrush(tileColor.GetOffsetColor(50)), 1d);
                darkPen = new Pen(new SolidColorBrush(tileColor.GetOffsetColor(-50)), 1d);
            }
            else
            {
                lightPen = new Pen(new SolidColorBrush(Color.FromArgb(255, 200, 200, 200)), 1d);
                darkPen = new Pen(new SolidColorBrush(Color.FromArgb(255, 100, 100, 100)), 1d);
            }

            var tileUnsetTarget = new RenderTargetBitmap((int)tileWidth, (int)tileHeight, 96, 96, PixelFormats.Pbgra32);
            var tileUnsetVisual = new DrawingVisual();

            using (var drawingContext = tileUnsetVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(tileBrush, null, new Rect(0, 0, tileWidth, tileHeight));
                for (var i = 0; i < horizontalThickness; i++)
                {
                    drawingContext.DrawLine(
                        lightPen,
                        new Point(i + 0.5, i + 0.5),
                        new Point(actualWidth - (i * 2) + 0.5, i + 0.5));
                    drawingContext.DrawLine(
                        darkPen,
                        new Point(i + 0.5, actualHeight - i + 0.5),
                        new Point(actualWidth - i + 0.5, actualHeight - i + 0.5));
                }

                for (var i = 0; i < verticalThickness; i++)
                {
                    drawingContext.DrawLine(
                        lightPen,
                        new Point(i + 0.5, i + 0.5),
                        new Point(i + 0.5, actualHeight - (i * 2) + 0.5));
                    drawingContext.DrawLine(
                        darkPen,
                        new Point(actualWidth - i + 0.5, i + 0.5),
                        new Point(actualWidth - i + 0.5, actualHeight - i + 0.5));
                }
            }

            var tileSetTarget = new RenderTargetBitmap((int)tileWidth, (int)tileHeight, 96, 96, PixelFormats.Pbgra32);
            var tileSetVisual = new DrawingVisual();
            //lightPen = new Pen(new SolidColorBrush(Color.FromArgb(255, 35, 35, 35)), 1.0);

            using (var drawingContext = tileSetVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(
                    new SolidColorBrush(Color.FromArgb(255, 200, 213, 232)),
                    null,
                    new Rect(0, 0, tileWidth, tileHeight));
                drawingContext.DrawRectangle(
                    null,
                    new Pen(new SolidColorBrush(Color.FromArgb(255, 223, 230, 235)), 1d),
                    new Rect(1.5, 1.5, tileWidth - 2, tileHeight - 2));
                drawingContext.DrawLine(darkPen, new Point(0.5, 0.5), new Point(actualWidth + 1.5, 0.5));
                drawingContext.DrawLine(darkPen, new Point(0.5, 0.5), new Point(0.5, actualHeight + 1.5));
            }

            tileUnsetTarget.Render(tileUnsetVisual);
            tileSetTarget.Render(tileSetVisual);

            return new Tuple<RenderTargetBitmap, RenderTargetBitmap>(tileUnsetTarget, tileSetTarget);
        }

        protected virtual void OnDrawMouseHover()
        {
        }

        protected virtual void OnDrawSelectedTiles()
        {
        }

        protected virtual void OnDrawTilesToUpdate()
        {
        }

        protected virtual void OnInitializeBoard()
        {
            this.BoardInitializedCommand.ExecuteIfAbleTo(new Size(this.Width, this.Height));
        }

        protected virtual void OnTileBrushChanged()
        {
        }

        protected virtual void OnTileShadingModeChanged()
        {
        }

        protected virtual void OnTileSizeChanged()
        {
        }

        protected virtual void OnSelectionBrushChanged()
        {
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= this.Visuals.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            return this.Visuals[index];
        }

        private static void AnimatedTilesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var board = (TileBoardBase)d;
            board.OnDrawMouseHover();
        }

        private static object CoerceHoverBrush(DependencyObject d, object baseValue)
        {
            if (baseValue == null)
            {
                return TileBoardBase.defaultHoverBrush;
            }

            return baseValue;
        }

        private static object CoerceMinesweeper(DependencyObject d, object baseValue)
        {
            if (baseValue == null)
            {
                return TileBoardBase.defaultMinesweeper;
            }

            var minesweeper = (IMinesweeper)baseValue;
            if (minesweeper.Tiles == null || minesweeper.Tiles.Width < 1 || minesweeper.Tiles.Height < 1)
            {
                return TileBoardBase.defaultMinesweeper;
            }

            return baseValue;
        }

        private static object CoerceSelectionBrush(DependencyObject d, object baseValue)
        {
            if (baseValue == null)
            {
                return TileBoardBase.defaultSelectionBrush;
            }

            return baseValue;
        }

        private static object CoerceTileBrush(DependencyObject d, object baseValue)
        {
            if (baseValue == null)
            {
                return TileBoardBase.defaultTileBrush;
            }

            return baseValue;
        }

        private static void ContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var board = (TileBoardBase)d;

            if (e.OldValue != null)
            {
                board.RemoveLogicalChild(e.OldValue);
            }

            if (e.NewValue != null)
            {
                board.AddLogicalChild(e.NewValue);
            }
        }

        private static void DrawPressedTileChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var board = (TileBoardBase)d;
            board.OnDrawMouseHover();
        }

        private static void HoverBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var board = (TileBoardBase)d;
            board.OnDrawMouseHover();
        }

        private static void HoverTileChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var board = (TileBoardBase)d;
            board.OnDrawMouseHover();
        }

        private static void MinesweeperChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var board = (TileBoardBase)d;
            board.OnInitializeBoard();
        }

        private static void SelectedTilesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var board = (TileBoardBase)d;
            board.OnDrawSelectedTiles();
        }

        private static void SelectionBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var board = (TileBoardBase)d;
            board.OnSelectionBrushChanged();
        }

        private static void TileBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var board = (TileBoardBase)d;
            board.OnTileBrushChanged();
        }

        private static void TileSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var board = (TileBoardBase)d;
            board.OnTileSizeChanged();
        }

        private static void TilesToUpdateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var board = (TileBoardBase)d;
            board.OnDrawTilesToUpdate();
        }
    }
}
