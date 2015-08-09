namespace WpfMinesweeper.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    ///     Represents a 2-dimensional array with a default property taking in
    ///     two index values.
    /// </summary>
    public interface IEnumerable2D<T> : IEnumerable<T>
    {
        /// <summary>
        ///     The default indexer property of the IEnumerable.
        /// </summary>
        /// <param name="x">The first dimensional index value.</param>
        /// <param name="y">The second dimensional index value.</param>
        /// <returns></returns>
        T this[int x, int y] { get; set; }
    }

    /// <summary>
    ///     Represents a collection of tiles, including the width and height of the board.
    /// </summary>
    public interface ITileCollection : IEnumerable2D<Tile>, ICloneable
    {
        int Height { get; }

        int Width { get; }
    }

    /// <summary>
    ///     The default implementation of ITileCollection.
    /// </summary>
    public class TileCollection : ITileCollection
    {
        private readonly int height;
        private readonly Tile[][] tiles;
        private readonly int width;

        protected TileCollection(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.tiles = new Tile[height][];

            for (var c = 0; c < height; c++)
            {
                this.tiles[c] = new Tile[width];
            }
        }

        public int Height
        {
            get
            {
                return this.height;
            }
        }

        public int Width
        {
            get
            {
                return this.width;
            }
        }

        public Tile this[int x, int y]
        {
            get
            {
                return this.tiles[y][x];
            }
            set
            {
                this.tiles[y][x] = value;
            }
        }

        /// <summary>
        ///     Returns a new <see cref="ITileCollection" /> with the specified width and height.
        /// </summary>
        /// <param name="width">The number of <see cref="WpfMinesweeper.Models.TileCollection.tiles" /> per row.</param>
        /// <param name="height">The number of <see cref="WpfMinesweeper.Models.TileCollection.tiles" /> per column.</param>
        /// <returns></returns>
        public static ITileCollection Create(int width, int height)
        {
            return new TileCollection(width, height);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public object Clone()
        {
            var newCollection = new TileCollection(this.width, this.height);
            for (var r = 0; r < this.Width; r++)
            {
                for (var c = 0; c < this.Height; c++)
                {
                    newCollection.tiles[r][c] = this.tiles[r][c];
                }
            }

            return newCollection;
        }

        public IEnumerator<Tile> GetEnumerator()
        {
            for (var r = 0; r < this.Width; r++)
            {
                for (var c = 0; c < this.Height; c++)
                {
                    yield return this[r, c];
                }
            }
        }
    }
}
