namespace WpfMinesweeper.Models
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    ///     The default implementation of ITileCollection.
    /// </summary>
    public class TileCollection : ITileCollection
    {
        private readonly Tile[][] tiles;

        protected TileCollection(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.tiles = new Tile[height][];

            for (var c = 0; c < height; c++)
            {
                this.tiles[c] = new Tile[width];
            }
        }

        public int Height { get; }

        public int Width { get; }

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
            var newCollection = new TileCollection(this.Width, this.Height);
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
