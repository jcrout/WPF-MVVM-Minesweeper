namespace WpfMinesweeper.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a 2-dimensional array with a default property taking in two index values.
    /// </summary>
    public interface IEnumerable2D<T> : IEnumerable<T>
    {
        /// <summary>
        /// The default indexer property of the IEnumerable.
        /// </summary>
        /// <param name="x">The first dimensional index value.</param>
        /// <param name="y">The second dimensional index value.</param>
        /// <returns></returns>
        T this[int x, int y] { get; set; }
    }

    /// <summary>
    /// Represents a collection of tiles, including the width and height of the board.
    /// </summary>
    public interface ITileCollection : IEnumerable2D<Tile>, ICloneable
    {
        int Width { get; }

        int Height { get; }
    }

    /// <summary>
    /// The default implementation of ITileCollection.
    /// </summary>
    public class TileCollection : ITileCollection
    {
        private readonly Tile[][] tiles;
        private readonly int width;
        private readonly int height;

        /// <summary>
        /// Returns a new ITileCollection with the specified width and height.
        /// </summary>
        /// <param name="width">The number of tiles per row.</param>
        /// <param name="height">The number of tiles per column.</param>
        /// <returns></returns>
        public static ITileCollection Create(int width, int height)
        {
            return new TileCollection(width,
                height);
        }

        protected TileCollection(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.tiles = new Tile[height][];

            for (int c = 0; c < height; c++)
            {
                this.tiles[c] = new Tile[width];
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

        public int Width
        {
            get
            {
                return this.width;
            }
        }

        public int Height
        {
            get
            {
                return this.height;
            }
        }

        public IEnumerator<Tile> GetEnumerator()
        {
            for (int r = 0; r < this.Width; r++)
            {
                for (int c = 0; c < this.Height; c++)
                {
                    yield return this[r,
                        c];
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public object Clone()
        {
            var newCollection = new TileCollection(this.width,
                this.height);
            for (int r = 0; r < this.Width; r++)
            {
                for (int c = 0; c < this.Height; c++)
                {
                    newCollection.tiles[r][c] = this.tiles[r][c];
                }
            }

            return newCollection;
        }
    }
}