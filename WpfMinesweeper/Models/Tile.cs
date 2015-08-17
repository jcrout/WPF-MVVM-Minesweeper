namespace WpfMinesweeper.Models
{
    using System.Runtime.InteropServices;

    /// <summary>
    ///     Represents a single tile on a <see cref="Minesweeper" /> tile board.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct Tile
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Tile" /> struct.
        /// </summary>
        /// <param name="type">The type of the tile.</param>
        /// <param name="shown">Indicates whether the Tile is shown on the board or not..</param>
        /// <param name="extraTileData">The extra tile data enumeration value.</param>
        public Tile(TileType type, bool shown, ExtraTileData extraTileData = ExtraTileData.None)
        {
            this.Type = type;
            this.Shown = shown;
            this.ExtraTileData = extraTileData;
        }

        /// <summary>
        ///     Gets the extra tile data set on this tile.
        /// </summary>
        /// <value>The extra tile data enumeration value.</value>
        public ExtraTileData ExtraTileData { get; }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="Tile" /> is shown on the board.
        /// </summary>
        /// <value><c>true</c> if shown; otherwise, <c>false</c>.</value>
        public bool Shown { get; }

        /// <summary>
        ///     Gets the type of the tile.
        /// </summary>
        /// <value>The TileType value.</value>
        public TileType Type { get; }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format(
                @"{0}; {1}{2}",
                this.Type,
                this.Shown ? "Shown" : "Not Shown",
                this.ExtraTileData != ExtraTileData.None ? "; " + this.ExtraTileData : string.Empty);
        }
    }
}
