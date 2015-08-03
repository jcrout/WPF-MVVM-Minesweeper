﻿namespace WpfMinesweeper.Models
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents a single tile on a Minesweeper tile board.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct Tile
    {  
        private TileType type;
        private ExtraTileData extraTileData;
        private bool shown;

        public Tile(TileType type, bool shown, ExtraTileData extraTileData = ExtraTileData.None)
        {
            this.type = type;
            this.shown = shown;
            this.extraTileData = extraTileData;
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

        public ExtraTileData ExtraTileData
        {
            get
            {
                return this.extraTileData;
            }
        }
    }

    /// <summary>
    /// Represents the type of a tile, including EmptySpace, Mine, Number, or Unset
    /// </summary>
    public struct TileType
    {
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
            if (mineCount == 0)
            {
                return TileType.EmptySpace;
            }

            ushort usmineCount = (ushort)mineCount;
            if (usmineCount > mineCountMaximum || usmineCount < 0)
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
            return ((this.value > 0) && (this.value <= mineCountMaximum));
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
    
    /// <summary>
    /// Specifies the extra data associated with a Tile, including whether or not the Tile is flagged or contains a question mark.
    /// </summary>
    public enum ExtraTileData
    {
        None,
        Flag,
        QuestionMark
    }
}
