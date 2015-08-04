namespace WpfMinesweeper.Models
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    ///     Specifies the extra data associated with a Tile, including whether or not the Tile is flagged or contains a
    ///     question mark.
    /// </summary>
    public enum ExtraTileData
    {
        None,
        Flag,
        QuestionMark
    }

    /// <summary>
    ///     Represents a single tile on a Minesweeper tile board.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct Tile
    {
        private readonly ExtraTileData extraTileData;
        private readonly bool shown;
        private readonly TileType type;

        public Tile(TileType type, bool shown, ExtraTileData extraTileData = ExtraTileData.None)
        {
            this.type = type;
            this.shown = shown;
            this.extraTileData = extraTileData;
        }

        public ExtraTileData ExtraTileData
        {
            get
            {
                return this.extraTileData;
            }
        }

        public bool Shown
        {
            get
            {
                return this.shown;
            }
        }

        public TileType Type
        {
            get
            {
                return this.type;
            }
        }
    }

    /// <summary>
    ///     Represents the type of a tile, including EmptySpace, Mine, Number, or Unset
    /// </summary>
    public struct TileType
    {
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
                return new TileType(TileType.TYPE_EMPTY);
            }
        }

        public static TileType Mine
        {
            get
            {
                return new TileType(TileType.TYPE_MINE);
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

            var usmineCount = (ushort)mineCount;
            if (usmineCount > TileType.mineCountMaximum || usmineCount < 0)
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
            if (this.value == TileType.TYPE_MINE)
            {
                return "MINE";
            }
            if (this.value == TileType.TYPE_EMPTY)
            {
                return "EMPTY";
            }

            return "UNSET";
        }

        private const ushort TYPE_EMPTY = 100;
        private const ushort TYPE_MINE = 200;
    }
}
