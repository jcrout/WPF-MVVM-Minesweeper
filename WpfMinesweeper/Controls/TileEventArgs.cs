namespace WpfMinesweeper.Controls
{
    using System;
    using Models;

    /// <summary>
    ///     Provides data associated with a <see cref="Tile" />.
    /// </summary>
    public class TileEventArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TileEventArgs" /> class.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public TileEventArgs(Tile tile, int x, int y)
        {
            this.Tile = tile;
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        ///     Gets the
        ///     <see cref="Tile" />
        ///     associated with the event.
        /// </summary>
        public Tile Tile { get; private set; }

        /// <summary>
        ///     Gets the Tile's X coordinate.
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        ///     Gets the Tile's Y coordinate.
        /// </summary>
        public int Y { get; private set; }
    }
}
