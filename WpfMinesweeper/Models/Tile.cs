namespace WpfMinesweeper.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Media;

    public interface IEnumerable2D<T> : IEnumerable<T>
    {
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

    public class TileCollection : ITileCollection
    {
        private Tile[][] tiles;
        private int width;
        private int height;

        public static ITileCollection Create(int width, int height)
        {
            return new TileCollection(width, height);
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
            get { return this.width; }
        }

        public int Height
        {
            get { return this.height; }
        }

        public IEnumerator<Tile> GetEnumerator()
        {
            for (int r = 0; r < this.Width; r++)
            {
                for (int c = 0; c < this.Height; c++)
                {
                    yield return this[r, c];
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public object Clone()
        {
            var newCollection = new TileCollection(this.width, this.height);
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

    /// <summary>
    /// Represents a single tile on the board. This struct contains a TileType property that
    /// indicates the type of the tile including number, mine, empty space. This struct also
    /// contains a boolean indicating if the tile has been shown.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct Tile
    {
        private static Tile empty = new Tile();

        private TileType type;
        private ExtraTileData extraTileData;
        private bool shown;

        public Tile(TileType type, bool shown, ExtraTileData extraTileData = ExtraTileData.None)
        {
            this.type = type;
            this.shown = shown;
            this.extraTileData = extraTileData;
        }

        public static Tile Empty
        {
            get
            {
                return empty;
            }
        }

        public bool Shown
        {
            get
            {
                return this.shown;
            }
            set
            {
                this.shown = value;
            }
        }

        public TileType Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }

        public ExtraTileData ExtraTileData
        {
            get
            {
                return this.extraTileData;
            }
            set
            {
                this.extraTileData = value;
            }
        }
    }

    /// <summary>
    /// This struct represents a type of Tile, such as a Mine or a Flag or a Number.
    /// </summary>
    public struct TileType
    {
        private static ushort mineCountMinimum = 1;
        private static ushort mineCountMaximum = 8;

        private const ushort TYPE_EMPTY = 100;
        private const ushort TYPE_MINE = 200;

        private ushort value;

        public static int MineCountMaximum
        {
            get
            {
                return (int)mineCountMaximum;
            }
        }

        public static int MineCountMinimum
        {
            get
            {
                return (int)mineCountMinimum;
            }
        }

        public static TileType Unset
        {
            get
            {
                return new TileType();
            }
        }

        public static TileType EmptySpace
        {
            get
            {
                return new TileType(TYPE_EMPTY);
            }
        }

        public static TileType Mine
        {
            get
            {
                return new TileType(TYPE_MINE);
            }
        }

        public static TileType Number(int mineCount)
        {
            ushort usmineCount = (ushort)mineCount;
            if (usmineCount < mineCountMinimum || usmineCount > mineCountMaximum)
            {
                throw new ArgumentOutOfRangeException("mineCount");
            }

            return new TileType(usmineCount);
        }

        public static bool operator ==(TileType t1, TileType t2)
        {
            return t1.value == t2.value;
        }

        public static bool operator !=(TileType t1, TileType t2)
        {
            return t1.value != t2.value;
        }

        private TileType(ushort value)
        {
            this.value = value;
        }

        public int  Value
        {
            get
            {
                return (int)this.value;
            }
        }

        public bool IsNumber()
        {
            return ((this.value >= mineCountMinimum) && (this.value <= mineCountMaximum));
        }

        public bool IsUnset()
        {
            return this.value == 0;
        }

        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var t2 = (TileType)obj;
            return this.value == t2.value;
        }

        public override string ToString()
        {
            if (this.IsNumber())
            {
                return this.value.ToString();
            }
            else if (this.value == TYPE_MINE)
            {
                return "MINE";
            }
            else if (this.value == TYPE_EMPTY)
            {
                return "EMPTY";
            }

            return "UNSET";
        }
    }

    public enum ExtraTileData
    {
        None,
        Flag,
        QuestionMark
    }

    public enum SmileyState
    {
        Default,
        TapDown,
        Victory,
        GameOver
    }
}
