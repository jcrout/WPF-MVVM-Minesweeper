namespace WpfMinesweeper.Models
{
    using System;

    /// <summary>
    ///     Represents a collection of tiles, including the width and height of
    ///     the board.
    /// </summary>
    public interface ITileCollection : IEnumerable2D<Tile>, ICloneable
    {
        int Height { get; }

        int Width { get; }
    }
}
