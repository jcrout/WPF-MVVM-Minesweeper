namespace WpfMinesweeper.Models
{
    using System;

    /// <summary>
    ///     Represents the type of a tile, including EmptySpace, Mine, Number,
    ///     or Unset
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
