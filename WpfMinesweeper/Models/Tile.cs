namespace WpfMinesweeper.Models
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    ///     Specifies the extra data associated with a Tile, including whether
    ///     or not the <see cref="Tile" /> is flagged or contains a question
    ///     mark.
    /// </summary>
    public enum ExtraTileData
    {
        None,
        Flag,
        QuestionMark
    }

    /// <summary>
    ///     Represents a single tile on a <see cref="Minesweeper" /> tile board.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct Tile
    {
        private readonly ExtraTileData extraTileData;
        private readonly bool shown;
        private readonly TileType type;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Tile" /> struct.
        /// </summary>
        /// <param name="type">The type of the tile.</param>
        /// <param name="shown">Indicates whether the Tile is shown on the board or not..</param>
        /// <param name="extraTileData">The extra tile data enumeration value.</param>
        public Tile(TileType type, bool shown, ExtraTileData extraTileData = ExtraTileData.None)
        {
            this.type = type;
            this.shown = shown;
            this.extraTileData = extraTileData;
        }

        /// <summary>
        ///     Gets the extra tile data set on this tile.
        /// </summary>
        /// <value>The extra tile data enumeration value.</value>
        public ExtraTileData ExtraTileData
        {
            get
            {
                return this.extraTileData;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="Tile" /> is shown on the board.
        /// </summary>
        /// <value><c>true</c> if shown; otherwise, <c>false</c>.</value>
        public bool Shown
        {
            get
            {
                return this.shown;
            }
        }

        /// <summary>
        ///     Gets the type of the tile.
        /// </summary>
        /// <value>The TileType value.</value>
        public TileType Type
        {
            get
            {
                return this.type;
            }
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format(
                @"{0}; {1}{2}",
                this.type,
                this.shown ? "Shown" : "Not Shown",
                this.extraTileData != ExtraTileData.None ? "; " + this.extraTileData : string.Empty);
        }
    }

    /// <summary>
    ///     Represents the type of a tile, including EmptySpace, Mine, Number, or Unset
    /// </summary>
    public struct TileType
    {
        private const ushort TypeEmpty = 100;
        private const ushort TypeMine = 200;
        private static readonly ushort mineCountMaximum = 8;
        private readonly ushort value;

        private TileType(ushort value)
        {
            this.value = value;
        }

        public static int MineCountMaximum
        {
            get
            {
                return TileType.mineCountMaximum;
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
                return new TileType(TileType.TypeEmpty);
            }
        }

        public static TileType Mine
        {
            get
            {
                return new TileType(TileType.TypeMine);
            }
        }

        public int Value
        {
            get
            {
                return this.value;
            }
        }

        public static TileType Number(int mineCount)
        {
            if (mineCount == 0)
            {
                return TileType.EmptySpace;
            }

            if (mineCount > TileType.mineCountMaximum || mineCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(mineCount));
            }

            return new TileType((ushort)mineCount);
        }

        public static bool operator ==(TileType t1, TileType t2)
        {
            return t1.value == t2.value;
        }

        public static bool operator !=(TileType t1, TileType t2)
        {
            return t1.value != t2.value;
        }

        public override bool Equals(object obj)
        {
            var t2 = (TileType)obj;
            return this.value == t2.value;
        }

        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }

        public bool IsNumber()
        {
            return ((this.value > 0) && (this.value <= TileType.mineCountMaximum));
        }

        public bool IsUnset()
        {
            return this.value == 0;
        }

        public override string ToString()
        {
            if (this.IsNumber())
            {
                return this.value.ToString();
            }
            if (this.value == TileType.TypeMine)
            {
                return "MINE";
            }
            if (this.value == TileType.TypeEmpty)
            {
                return "EMPTY";
            }

            return "UNSET";
        }
    }
}
