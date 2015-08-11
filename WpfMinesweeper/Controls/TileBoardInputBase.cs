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

    [DebuggerStepThrough]
    public abstract class TileBoardInputBase : TileBoardBase
    {
        private static readonly long doubleClickInterval = SystemInformation.DoubleClickTime *
                                                   Stopwatch.Frequency / 1000;
        private long lastClickTime = -1;

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            this.TileHoverCommand.ExecuteIfAbleTo(new TileEventArgs(new Tile(), -1, -1));
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            var doubleClicked = false;
            if (this.lastClickTime != -1)
            {
                var currentClickTime = Stopwatch.GetTimestamp();
                if (currentClickTime - this.lastClickTime <= TileBoardInputBase.doubleClickInterval)
                {
                    doubleClicked = true;
                }
                this.lastClickTime = -1;
            }
            else
            {
                this.lastClickTime = Stopwatch.GetTimestamp();
            }

            this.RaiseTileTap(e.GetPosition(this), InputButtons.Left, true, doubleClicked, e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            this.RaiseTileTap(e.GetPosition(this), InputButtons.Left, false, false, e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var minesweeper = this.Minesweeper;
            var mouseCoordinates = e.GetPosition(this);
            var tileX = Math.Max(
                0,
                Math.Min((int)Math.Floor(mouseCoordinates.X / this.TileSize.Width), minesweeper.Tiles.Width - 1));
            var tileY = Math.Max(
                0,
                Math.Min((int)Math.Floor(mouseCoordinates.Y / this.TileSize.Height), minesweeper.Tiles.Height - 1));

            var newCoords = new Point<int>(tileX, tileY);
            if (this.HoverTile != newCoords)
            {
                this.TileHoverCommand.ExecuteIfAbleTo(
                    new TileEventArgs(this.Minesweeper.Tiles[tileX, tileY], tileX, tileY));
            }
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

        private void RaiseTileTap(Point tapPoint, InputButtons button, bool pressedDown, bool doubleClicked,
            MouseButtonEventArgs e)
        {
            var allButtons = e != null ? this.GetStateOfAllMouseButtons(e) : InputButtons.None;

            this.TilePressCommand.ExecuteIfAbleTo(
                new TileTapEventArgs(
                    this.GetTileEventArgsFromBoardPoint(tapPoint),
                    button,
                    doubleClicked,
                    pressedDown,
                    allButtons));
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
                (int)Math.Max(0,
                    Math.Min(Math.Floor(boardCoordinates.X / this.TileSize.Width),
                        this.Minesweeper.Tiles.Width - 1)),
                (int)Math.Max(0,
                    Math.Min(Math.Floor(boardCoordinates.Y / this.TileSize.Height),
                        this.Minesweeper.Tiles.Height - 1)));
        }

        private TileEventArgs GetTileEventArgsFromBoardPoint(Point boardPoint)
        {
            if (boardPoint.X == -1)
            {
                return null;
            }

            var tilePoint = this.GetTileCoordinatesFromBoardPoint(boardPoint);
            var x = (int)tilePoint.X;
            var y = (int)tilePoint.Y;

            return new TileEventArgs(this.Minesweeper.Tiles[x, y], x, y);
        }
    }
}
