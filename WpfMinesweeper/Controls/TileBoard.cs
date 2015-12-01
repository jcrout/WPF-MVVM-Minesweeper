namespace WpfMinesweeper.Controls
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using JonUtility;
    using JonUtility.WPF;
    using Models;

    public class TileBoard : TileBoardInputBase
    {
        private static PixelData defaultTileSetData;
        private static PixelData defaultTileUnsetData;
        private static PixelData flagImageData;
        private static PixelData questionMarkImageData;
        private static Dictionary<TileType, PixelData> tileTypeImageData;
        private readonly int tileHeight;
        private readonly int tileWidth;
        private WriteableBitmap boardBitmap;
        private ITileCollection boardTiles;
        private WriteableBitmap boardTop;
        private DrawingVisual boardTopVisual;
        private DrawingVisual boardVisual;
        private DrawingVisual mouseHoverVisual;
        private WriteableBitmapProxy proxy;
        private DrawingVisual selectionVisual;
        private ImageBrush tileSetBrush = new ImageBrush(TileBoardBase.DefaultTileSetImage);
        private PixelData tileSetData;
        private PixelData tileUnsetData;
        private bool useTileShader;

        static TileBoard()
        {
            TileBoard.defaultTileSetData = TileBoardBase.DefaultTileSetImage.GetPixelData();
            TileBoard.defaultTileUnsetData = TileBoardBase.DefaultTileUnsetImage.GetPixelData();

            TileBoard.tileTypeImageData = new Dictionary<TileType, PixelData>();
            var imageFormat = TileBoardBase.DefaultTileUnsetImage.Format;

            foreach (var keyPair in TileBoardBase.TileTypeImages)
            {
                TileBoard.tileTypeImageData.Add(
                    keyPair.Key,
                    keyPair.Value.ConvertFormat(PixelFormats.Bgra32).GetPixelData(TileBoard.defaultTileUnsetData.Stride));
            }

            TileBoard.flagImageData = TileBoardBase.FlagImage.GetPixelData(TileBoard.defaultTileUnsetData.Stride);
            TileBoard.questionMarkImageData = TileBoardBase.QuestionMarkImage.GetPixelData(TileBoard.defaultTileUnsetData.Stride);
        }

        protected override void OnTouchDown(TouchEventArgs e)
        {
            var point = e.GetTouchPoint(this);            

        }

        public TileBoard()
        {
            this.tileSetData = TileBoard.defaultTileSetData;
            this.tileUnsetData = TileBoard.defaultTileUnsetData;
            this.tileWidth = (int)this.TileSize.Width;
            this.tileHeight = (int)this.TileSize.Height;
        }

        protected override void OnDrawMouseHover()
        {
            var hoverPoint = this.HoverTile;

            if (this.mouseHoverVisual != null)
            {
                this.Visuals.Remove(this.mouseHoverVisual);
                this.RemoveVisualChild(this.mouseHoverVisual);
                this.mouseHoverVisual = null;
            }

            if (hoverPoint.X < 0 || hoverPoint.Y < 0 || hoverPoint.X >= this.boardTiles.Width || hoverPoint.Y >= this.boardTiles.Height)
            {
                return;
            }

            this.mouseHoverVisual = new DrawingVisual();
            using (var context = this.mouseHoverVisual.RenderOpen())
            {
                context.DrawRectangle(
                    this.DrawPressedTile ? this.tileSetBrush : this.HoverBrush,
                    null,
                    new Rect(
                        hoverPoint.X * this.tileWidth,
                        hoverPoint.Y * this.tileHeight,
                        this.tileWidth,
                        this.tileHeight));
            }

            this.Visuals.Add(this.mouseHoverVisual);
            this.AddVisualChild(this.mouseHoverVisual);
        }

        protected override void OnDrawSelectedTiles()
        {
            this.AnimateTiles(this.SelectedTiles, false);
        }

        protected override void OnDrawTilesToUpdate()
        {
            this.AnimateTiles(this.TilesToUpdate, true);
        }

        protected override void OnInitializeBoard()
        {
            this.InitBoard();
            base.OnInitializeBoard();
        }

        protected override void OnSelectionBrushChanged()
        {
            this.DrawSelectedTiles();
        }

        protected override void OnTileBrushChanged()
        {
            base.OnTileBrushChanged();

            var currentlyUsingShader = this.useTileShader;
            this.useTileShader = this.TileBrush is GradientBrush;

            if (!this.useTileShader)
            {
                var tileImages = TileBoardBase.CreateTileImage(this.TileBrush, this.TileSize);
                this.tileUnsetData = tileImages.Item1.GetPixelData();
                this.tileSetData = tileImages.Item2.GetPixelData();
                this.tileSetBrush = new ImageBrush(tileImages.Item2);
                if (this.boardTopVisual != null)
                {
                    this.proxy = new WriteableBitmapProxy(this.boardBitmap);
                    this.InitBoard();
                    return;
                }

                this.DrawBoard();
            }
            else
            {
                this.tileUnsetData = TileBoard.defaultTileUnsetData;
                this.tileSetData = TileBoard.defaultTileSetData;
                this.tileSetBrush = new ImageBrush(TileBoardBase.DefaultTileSetImage);

                this.InitBoard();
            }
        }

        private void AddVisuals()
        {
            this.boardVisual = new DrawingVisual();
            using (var drawingContext = this.boardVisual.RenderOpen())
            {
                drawingContext.DrawImage(this.boardBitmap, new Rect(0, 0, this.Width, this.Height));

                if (this.useTileShader)
                {
                    drawingContext.DrawRectangle(this.TileBrush, null, new Rect(0, 0, this.Width, this.Height));
                }
            }

            this.Visuals.Add(this.boardVisual);
            this.AddVisualChild(this.boardVisual);

            if (this.useTileShader)
            {
                this.boardTopVisual = new DrawingVisual();
                using (var drawingContext = this.boardTopVisual.RenderOpen())
                {
                    drawingContext.DrawImage(this.boardTop, new Rect(0, 0, this.Width, this.Height));
                }

                this.Visuals.Add(this.boardTopVisual);
                this.AddVisualChild(this.boardTopVisual);
            }
        }

        private async void AnimateTiles(AnimatedTilesCollection animatedTilesCollection, bool updateTiles)
        {
            bool hasTiles = animatedTilesCollection != null && animatedTilesCollection.Tiles != null &&
                            animatedTilesCollection.Tiles.Any();

            if (hasTiles && animatedTilesCollection.Frames != null && animatedTilesCollection.Frames.Any())
            {
                foreach (var frame in animatedTilesCollection.Frames)
                {
                    var animationVisual = new DrawingVisual();
                    using (var drawingContext = animationVisual.RenderOpen())
                    {
                        var image = frame.Image;
                        var offsetX = Math.Max(0, (this.TileSize.Width - image.Width) / 2);
                        var offsetY = Math.Max(0, (this.TileSize.Height - image.Height) / 2);

                        foreach (var tilePoint in animatedTilesCollection.Tiles)
                        {
                            var boardPoint = this.GetBoardCoordinatesFromTilePoint(tilePoint);
                            var drawRect = new Rect(
                                boardPoint.X + offsetX,
                                boardPoint.Y + offsetY,
                                image.Width,
                                image.Height);
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
                if (hasTiles)
                {
                    this.proxy.Lock();
                    foreach (var tilePoint in animatedTilesCollection.Tiles)
                    {
                        this.DrawTile(tilePoint.X, tilePoint.Y);
                    }

                    this.proxy.Unlock();
                }
            }
            else
            {
                this.DrawSelectedTiles();
            }
        }

        private void ApplyShaderAndUpdateRemainingTiles(ConcurrentStack<Point<int>> tilesToDrawAfterShading)
        {
            this.boardTop = new WriteableBitmap(this.boardBitmap.PixelWidth, this.boardBitmap.PixelHeight, this.boardBitmap.DpiX, this.boardBitmap.DpiY, this.boardBitmap.Format, this.boardBitmap.Palette);
            this.proxy = new WriteableBitmapProxy(this.boardTop);

            if (tilesToDrawAfterShading.Count > 0)
            {
                this.boardTop.Lock();
                foreach (var tilePoint in tilesToDrawAfterShading)
                {
                    this.DrawTile(tilePoint.X, tilePoint.Y, true);
                }

                this.boardTop.Unlock();
            }
        }

        private void DrawBoard()
        {
            if (this.boardBitmap == null)
            {
                return;
            }

            Action<int, int, bool, ImageDrawingMode> drawMethod;
            ConcurrentStack<Point<int>> tilesToDraw = null;
            this.boardBitmap.Lock();

            if (!this.useTileShader)
            {
                drawMethod = this.DrawTile;
            }
            else
            {
                tilesToDraw = new ConcurrentStack<Point<int>>();
                drawMethod = (int r, int c, bool b, ImageDrawingMode mode) =>
                {
                    var tile = this.boardTiles[r, c];
                    var tilePoint = this.GetBoardCoordinatesFromTilePoint(r, c);
                    this.proxy.DrawImagePixels(this.tileUnsetData, tilePoint.X, tilePoint.Y);

                    if (!tile.Shown)
                    {
                        if (tile.ExtraTileData != ExtraTileData.None)
                        {
                            tilesToDraw.Push(new Point<int>(r, c));
                        }
                    }
                    else
                    {
                        tilesToDraw.Push(new Point<int>(r, c));
                    }
                };
            }

            this.DrawBoardBase(drawMethod);
            this.boardBitmap.AddDirtyRect(new Int32Rect(0, 0, (int)this.Width, (int)this.Height));
            this.boardBitmap.Unlock();

            if (this.useTileShader)
            {
                this.ApplyShaderAndUpdateRemainingTiles(tilesToDraw);
            }
        }

        private void DrawBoardBase(Action<int, int, bool, ImageDrawingMode> drawMethod)
        {
            int totalTiles = this.boardTiles.Width * this.boardTiles.Height;
            int procCount = Environment.ProcessorCount;

            if (procCount == 1 || totalTiles < 150)
            {
                for (var r = 0; r < this.boardTiles.Width; r++)
                {
                    for (var c = 0; c < this.boardTiles.Height; c++)
                    {
                        drawMethod(r, c, false, ImageDrawingMode.ExactCopy);
                    }
                }
            }
            else
            {
                var methods = new Action[procCount];
                int interval = (int)Math.Ceiling((double)this.boardTiles.Height / procCount);

                for (int i = 0; i < procCount; i++)
                {
                    int startIndex = i * interval;
                    int endIndex = Math.Min(startIndex + interval, this.boardTiles.Height);

                    methods[i] = () =>
                    {
                        for (var c = startIndex; c < endIndex; c++)
                        {
                            for (var r = 0; r < this.boardTiles.Width; r++)
                            {
                                drawMethod(r, c, false, ImageDrawingMode.ExactCopy);
                            }
                        }
                    };
                }

                Parallel.Invoke(methods);
            }
        }

        private void DrawImageWithOffsets(PixelData pixelsToDraw, Point<int> tilePoint, bool roundUp = true)
        {
            if (roundUp)
            {
                var offsetX = tilePoint.X + (int)Math.Max(0, Math.Ceiling((double)(this.tileWidth - pixelsToDraw.PixelWidth) / 2));
                var offsetY = tilePoint.Y + (int)Math.Max(0, Math.Ceiling((double)(this.tileHeight - pixelsToDraw.PixelHeight) / 2));

                this.proxy.DrawImagePixels(pixelsToDraw, offsetX, offsetY);
            }
            else
            {
                var offsetX = tilePoint.X + (int)Math.Max(0, Math.Floor((double)(this.tileWidth - pixelsToDraw.PixelWidth) / 2));
                var offsetY = tilePoint.Y + (int)Math.Max(0, Math.Floor((double)(this.tileHeight - pixelsToDraw.PixelHeight) / 2));

                this.proxy.DrawImagePixels(pixelsToDraw, offsetX, offsetY);
            }
        }

        private void DrawSelectedTiles()
        {
            if (this.selectionVisual != null)
            {
                this.Visuals.Remove(this.selectionVisual);
                this.RemoveVisualChild(this.selectionVisual);
                this.selectionVisual = null;
            }

            if (this.proxy == null)
            {
                return;
            }

            if (this.SelectedTiles != null && this.SelectedTiles.Tiles != null && this.SelectedTiles.Tiles.Count > 0)
            {
                var selectedPoints = this.SelectedTiles.Tiles.ToArray();

                this.selectionVisual = new DrawingVisual();
                using (var context = this.selectionVisual.RenderOpen())
                {
                    foreach (var tilePoint in selectedPoints)
                    {
                        context.DrawRectangle(
                            this.SelectionBrush,
                            null,
                            new Rect(
                                tilePoint.X * this.TileSize.Width,
                                tilePoint.Y * this.TileSize.Height,
                                this.TileSize.Width,
                                this.TileSize.Height));
                    }
                }

                this.Visuals.Add(this.selectionVisual);
                this.AddVisualChild(this.selectionVisual);
            }
        }

        private void DrawTile(int x, int y, bool addDirtyRect = true, ImageDrawingMode mode = ImageDrawingMode.IgnoreAlpha)
        {
            var tile = this.boardTiles[x, y];
            var tilePoint = this.GetBoardCoordinatesFromTilePoint(x, y);
            var tileIsMine = tile.Type == TileType.Mine;

            if (!tile.Shown || tileIsMine)
            {
                if (this.useTileShader)
                {
                    this.boardTop.FillRectangle(tilePoint.X, tilePoint.Y, tilePoint.X + this.tileWidth, tilePoint.Y + this.tileHeight, Colors.Transparent);
                }
                else
                {
                    this.proxy.DrawImagePixels(this.tileUnsetData, tilePoint.X, tilePoint.Y, mode);
                }
            }
            else
            {
                if (!this.useTileShader)
                {
                    this.proxy.DrawImagePixels(this.tileUnsetData, tilePoint.X, tilePoint.Y, ImageDrawingMode.IgnoreAlpha);
                    this.proxy.DrawImagePixels(this.tileSetData, tilePoint.X, tilePoint.Y, ImageDrawingMode.IgnoreAlpha);
                }
                else
                {
                    this.proxy.DrawImagePixels(this.tileSetData, tilePoint.X, tilePoint.Y, mode);
                }
            }

            if (tile.Type != TileType.EmptySpace)
            {
                if (tile.Type != TileType.Unset && tile.Shown)
                {
                    this.DrawImageWithOffsets(TileBoard.tileTypeImageData[tile.Type], tilePoint, tile.Type == TileType.Mine ? false : true);
                }

                if (tile.ExtraTileData == ExtraTileData.Flag)
                {
                    this.DrawImageWithOffsets(TileBoard.flagImageData, tilePoint);
                }
                else if (tile.ExtraTileData == ExtraTileData.QuestionMark)
                {
                    this.DrawImageWithOffsets(TileBoard.questionMarkImageData, tilePoint);
                }
            }

            if (addDirtyRect)
            {
                this.proxy.AddDirtyRect(this.GetTileRectangleFromCoordinates(x, y));
            }
        }

        private Point GetBoardCoordinatesFromTilePoint(Point<int> tileCoordinates)
        {
            return new Point(tileCoordinates.X * this.TileSize.Width, tileCoordinates.Y * this.TileSize.Height);
        }

        private Point<int> GetBoardCoordinatesFromTilePoint(int x, int y)
        {
            return new Point<int>(x * this.tileWidth, y * this.tileHeight);
        }

        private Int32Rect GetTileRectangleFromCoordinates(int x, int y)
        {
            return new Int32Rect(
                x * (int)this.TileSize.Width,
                y * (int)this.TileSize.Height,
                (int)this.TileSize.Width,
                (int)this.TileSize.Height);
        }

        private void InitBoard()
        {
            var minesweeper = this.Minesweeper;
            if (minesweeper == null)
            {
                return;
            }

            double time1 = 0, time2 = 0;
            var sw = Stopwatch.StartNew();

            this.boardTiles = minesweeper.Tiles;
            double newWidth = this.boardTiles.Width * this.TileSize.Width;
            double newHeight = this.boardTiles.Height * this.TileSize.Height;

            bool resizeControl = this.Width != newWidth || this.Height != newHeight;
            if (resizeControl)
            {
                this.Width = this.boardTiles.Width * this.TileSize.Width;
                this.Height = this.boardTiles.Height * this.TileSize.Height;
            }

            if (this.boardBitmap == null || resizeControl)
            {
                this.boardBitmap = new WriteableBitmap((int)this.Width, (int)this.Height, 96d, 96d, PixelFormats.Bgra32, null);
                this.proxy = new WriteableBitmapProxy(this.boardBitmap);
            }

            this.Visuals.ForEach(this.RemoveVisualChild);
            this.Visuals.Clear();

            time1 = sw.Elapsed.TotalMilliseconds;
            this.DrawBoard();

            time2 = sw.Elapsed.TotalMilliseconds;
            this.AddVisuals();

            sw.Stop();

            Console.WriteLine("1) Resizing/Clearing Visuals: " + time1.ToString("0.0000"));
            Console.WriteLine("2) Drawing Board: " + (time2 - time1).ToString("0.0000"));
            Console.WriteLine("3) Adding Visuals: " + (sw.Elapsed.TotalMilliseconds - time2).ToString("0.0000"));
        }
    }
}
