namespace WpfMinesweeper.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Input;
    using System.Windows.Media;
    using JonUtility;
    using Models;
    using MouseEventArgs = System.Windows.Input.MouseEventArgs;

    public class TileBoardOld : TileBoardInputBase
    {
        private readonly bool applyShadingToRevealedTiles = false;

        private int boardHeight;
        private int boardWidth;
        private DrawingVisual mouseHoverVisual;
        private List<Visual> selectedTileVisuals;
        private DrawingVisual shaderVisual;
        private ImageSource tileSetImage = TileBoardBase.DefaultTileSetImage;
        private ImageSource tileUnsetImage = TileBoardBase.DefaultTileUnsetImage;
        private bool useTilerShader;

        protected override void OnDrawMouseHover()
        {
            if (this.mouseHoverVisual != null)
            {
                this.Visuals.Remove(this.mouseHoverVisual);
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
                    if (this.DrawPressedTile)
                    {
                        drawingContext.DrawImage(this.tileSetImage, tileRect);
                    }
                    else
                    {
                        drawingContext.DrawRectangle(this.HoverBrush, null, tileRect);
                    }
                }

                this.Visuals.Add(this.mouseHoverVisual);
                this.AddVisualChild(this.mouseHoverVisual);
            }
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
            var skipRedraw = this.boardWidth == this.Minesweeper.Tiles.Width &&
                             this.boardHeight == this.Minesweeper.Tiles.Height;
            if (skipRedraw)
            {
                var lastTileIndex = this.boardWidth * this.boardHeight;
                var removeEnd = (this.shaderVisual == null) ? lastTileIndex - 1 : lastTileIndex;
                for (var i = this.Visuals.Count - 1; i > removeEnd; i--)
                {
                    this.RemoveVisualChild(this.Visuals[i]);
                    this.Visuals.RemoveAt(i);
                }
                for (var i = 0; i < lastTileIndex; i++)
                {
                    ((DrawingVisual)this.Visuals[i]).Children.Clear();
                }
            }
            else
            {
                this.Visuals.ForEach(v => this.RemoveVisualChild(v));
                this.Visuals.Clear();
                this.boardWidth = this.Minesweeper.Tiles.Width;
                this.boardHeight = this.Minesweeper.Tiles.Height;
                this.Width = this.boardWidth * this.TileSize.Width;
                this.Height = this.boardHeight * this.TileSize.Height;

                this.DrawBoard();
                this.DrawTileShader();
            }

            this.HoverTile = new Point<int>(-1, -1);
            base.OnInitializeBoard();
        }
        
        protected override void OnTileBrushChanged()
        {
            if (this.TileBrush is GradientBrush)
            {
                this.useTilerShader = true;
                this.tileUnsetImage = DefaultTileUnsetImage;
                this.tileSetImage = DefaultTileSetImage;
            }
            else
            {
                var tileImages = TileBoardBase.CreateTileImage(this.TileBrush, this.TileSize);
                this.tileUnsetImage = tileImages.Item1;
                this.tileSetImage = tileImages.Item2;
            }

            this.RedrawBoard();
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
                    this.DrawTiles(animatedTilesCollection.Tiles);
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
                    this.DrawTile(r, c);
                }
            }
        }

        private void DrawImageWithOffsets(DrawingContext drawingContext, ImageSource imageToDraw, Rect tileRect)
        {
            var offsetX = Math.Max(0, (this.TileSize.Width - imageToDraw.Width) / 2);
            var offsetY = Math.Max(0, (this.TileSize.Height - imageToDraw.Height) / 2);
            drawingContext.DrawImage(
                imageToDraw,
                new Rect(tileRect.X + offsetX, tileRect.Y + offsetY, imageToDraw.Width, imageToDraw.Height));
        }

        private void DrawSelectedTiles()
        {
            if (this.selectedTileVisuals != null)
            {
                this.selectedTileVisuals.ForEach(
                    v =>
                    {
                        this.Visuals.Remove(v);
                        this.RemoveVisualChild(v);
                    });
            }

            var selectedTiles = this.SelectedTiles;
            if (selectedTiles == null || selectedTiles.Tiles == null || !selectedTiles.Tiles.Any())
            {
                return;
            }

            var selectedTilePoints = selectedTiles.Tiles;
            this.selectedTileVisuals = new List<Visual>(selectedTilePoints.Count);

            foreach (var tilePoint in selectedTilePoints)
            {
                var tileRect = this.GetTileRectangleFromTilePoint(tilePoint);
                var selectionVisual = new DrawingVisual();
                using (var drawingContext = selectionVisual.RenderOpen())
                {
                    drawingContext.DrawRectangle(this.SelectionBrush, null, tileRect);
                }

                this.selectedTileVisuals.Add(selectionVisual);
                this.Visuals.Add(selectionVisual);
                this.AddVisualChild(selectionVisual);
            }
        }

        private void DrawTile(int tileX, int tileY)
        {
            var tileVisual = new DrawingVisual();

            using (var drawingContext = tileVisual.RenderOpen())
            {
                var tileRect = this.GetTileRectangleFromTilePoint(new Point<int>(tileX, tileY));
                drawingContext.DrawImage(this.tileUnsetImage, tileRect);
            }

            this.Visuals.Add(tileVisual);
            this.AddVisualChild(tileVisual);
        }

        private void DrawTiles(List<Point<int>> tileList)
        {
            foreach (var tilePoint in tileList)
            {
                var index = (int)(this.Minesweeper.Tiles.Height * tilePoint.X + tilePoint.Y);
                var tile = this.Minesweeper.Tiles[tilePoint.X, tilePoint.Y];
                var tileVisual = (DrawingVisual)this.Visuals[index];
                var tileRect = this.GetTileRectangleFromTilePoint(tilePoint);

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
                        this.DrawImageWithOffsets(
                            drawingContext,
                            tile.ExtraTileData == ExtraTileData.Flag
                                ? TileBoardBase.FlagImage
                                : TileBoardBase.QuestionMarkImage,
                            tileRect);
                    }

                    tileVisual.Children.Add(visual);
                }
                else
                {
                    var visual = new DrawingVisual();
                    using (var drawingContext = visual.RenderOpen())
                    {
                        if (tile.Type != TileType.Mine)
                        {
                            drawingContext.DrawImage(this.tileSetImage, tileRect);
                        }

                        if (tile.Type != TileType.EmptySpace)
                        {
                            this.DrawImageWithOffsets(drawingContext, TileBoardBase.TileTypeImages[tile.Type], tileRect);
                        }
                    }

                    if (this.applyShadingToRevealedTiles)
                    {
                        tileVisual.Children.Add(visual);
                    }
                    else
                    {
                        this.Visuals.Add(visual);
                        this.AddVisualChild(visual);
                    }
                }
            }
        }

        private void DrawTileShader()
        {
            if (this.useTilerShader)
            {
                return;
            }

            if (this.shaderVisual != null)
            {
                this.Visuals.Remove(this.shaderVisual);
                this.RemoveVisualChild(this.shaderVisual);
            }

            this.shaderVisual = new DrawingVisual();
            using (var drawingContext = this.shaderVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(this.TileBrush, null, new Rect(0, 0, this.Width, this.Height));
            }

            this.Visuals.Add(this.shaderVisual);
            this.AddVisualChild(this.shaderVisual);
        }

        private Point GetBoardCoordinatesFromTilePoint(Point<int> tileCoordinates)
        {
            return new Point(tileCoordinates.X * this.TileSize.Width, tileCoordinates.Y * this.TileSize.Height);
        }

        private Rect GetTileRectangleFromTilePoint(Point<int> tileCoordinates)
        {
            var tileSize = this.TileSize;
            return new Rect(
                tileCoordinates.X * tileSize.Width,
                tileCoordinates.Y * tileSize.Height,
                tileSize.Width,
                tileSize.Height);
        }

        private void RedrawBoard()
        {
            if (this.Visuals.Count == 0)
            {
                return;
            }

            this.Visuals.ForEach(v => this.RemoveVisualChild(v));
            this.Visuals.Clear();
            this.DrawBoard();

            var allTilePoints = new List<Point<int>>(this.boardWidth * this.boardHeight);
            for (int r = 0; r < this.boardWidth; r++)
            {
                for (int c = 0; c < this.boardHeight; c++)
                {
                    allTilePoints.Add(new Point<int>(r, c));
                }
            }

            this.DrawTiles(allTilePoints);
        }
    }
}
