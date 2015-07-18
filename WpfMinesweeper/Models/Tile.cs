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

    /// <summary>
    /// This interface implements the core aspects of the game Minesweeper.
    /// </summary>
    public interface IMinesweeper : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the collection of tiles that make up the board.
        /// </summary>
        ITileCollection Tiles { get; }

        /// <summary>
        /// Gets the total amount of time that has elapsed since the game began.
        /// </summary>
        int TimeElapsed { get; set; }

        /// <summary>
        /// Gets the total number of mines on the board.
        /// </summary>
        int MineCount { get; }

        /// <summary>
        /// Gets the total number of mines remaining (unflagged) on the board.
        /// </summary>
        int MinesRemaining { get; set; }
    }

    public class MinesweeperFactory
    {
        public static Minesweeper Create(int width, int height, int mineCount)
        {
            return Minesweeper.Create(width, height, mineCount);
        }
    }

    /// <summary>
    /// THe default implementation of the IMinesweeper interface.
    /// </summary>
    public class Minesweeper : IMinesweeper
    {
        /// <summary>
        /// Total number of mines on the board.
        /// </summary>
        private int mineCount;

        /// <summary>
        /// Total number of mines remaining (unflagged) on the board.
        /// </summary>
        private int minesLeft;

        /// <summary>
        /// The collection of tiles that make up the board.
        /// </summary>
        private ITileCollection tiles;

        //private IEnumerable<>

        /// <summary>
        /// Total amount of time that has elapsed since the game began.
        /// </summary>
        private int timeElapsed;

        public event PropertyChangedEventHandler PropertyChanged;

        public static Minesweeper Create(int width, int height, int mineCount)
        {
            if (width < 1 || height < 1)
            {
                throw new ArgumentException("Board width and height must be greater than 0.");
            }

            if (mineCount >= (width * height))
            {
                throw new ArgumentException("mineCount must be less than the total number of spaces.");
            }

            var minesweeper = new Minesweeper();
            minesweeper.tiles = TileCollection.Create(width, height);
            minesweeper.mineCount = mineCount;
            minesweeper.minesLeft = mineCount;
            minesweeper.timeElapsed = 0;

            return minesweeper;
        }

        private Minesweeper()
        {
        }

        public int MineCount
        {
            get
            {
                return this.mineCount;
            }
            private set
            {
                if (this.mineCount != value)
                {
                    this.mineCount = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public int MinesRemaining
        {
            get
            {
                return this.minesLeft;
            }
            set
            {
                if (this.minesLeft != value)
                {
                    this.minesLeft = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public ITileCollection Tiles
        {
            get
            {
                return this.tiles;
            }
            private set
            {
                if (this.tiles != value)
                {
                    this.tiles = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public int TimeElapsed
        {
            get
            {
                return this.timeElapsed;
            }
            set
            {
                if (this.timeElapsed != value)
                {
                    this.timeElapsed = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        protected void RaisePropertyChanged([CallerMemberName]string prop = "")
        {
            var propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }

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
        private bool shown;

        public Tile(TileType type, bool shown)
        {
            this.type = type;
            this.shown = shown;
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

    public interface ITileImageFactory
    {
        Brush TileColor { get; set; }

        Brush BorderLightColor { get; set; }

        Brush BorderDarkColor { get; set; }

        ImageSource GetImage(Tile tile);
    }

    public class TileImageFactory : ITileImageFactory
    {
        private ImageSource tileImage;
        private ImageSource mineImage;

        static TileImageFactory()
        {
            //CreateTileImage();
        }


        public static ITileImageFactory Create()
        {
            return new TileImageFactory();
        }

        ImageSource ITileImageFactory.GetImage(Tile tile)
        {
            throw new NotImplementedException();
        }

        protected TileImageFactory()
        {
        }

        Brush ITileImageFactory.TileColor
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        Brush ITileImageFactory.BorderLightColor
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        Brush ITileImageFactory.BorderDarkColor
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }

    public class TileStateFlags
    {
        private static List<TileStateFlags> allStates;
        private int value;
        private string name;

        public static TileStateFlags Unset = new TileStateFlags(0, "Unset");
        public static TileStateFlags Shown = new TileStateFlags(1, "Shown");
        public static TileStateFlags MouseHover = new TileStateFlags(2, "MouseHover");
        public static TileStateFlags MouseDown = new TileStateFlags(4, "MouseDown");

        static TileStateFlags()
        {
            allStates = new List<TileStateFlags>()
                {                
                    Shown,
                    MouseHover,
                    MouseDown
                };
        }

        public static TileStateFlags operator |(TileStateFlags enum1, TileStateFlags enum2)
        {
            if (enum1 == null && enum2 == null)
            {
                return null;
            }

            if (enum1 == null && enum2 != null)
            {
                return enum2;
            }

            if (enum2 == null && enum1 != null)
            {
                return enum1;
            }

            int newValue = enum1.value | enum2.value;
            return new TileStateFlags(newValue, GetNewName(newValue));
        }

        private static string GetNewName(int newValue)
        {
            if (newValue == 0)
            {
                return Unset.name;
            }
            else
            {
                var builder = new StringBuilder();
                foreach (var tileState in allStates)
                {
                    if ((newValue & tileState.value) != 0)
                    {
                        builder.Append(tileState.name);
                        builder.Append(", ");
                    }
                }

                builder.Remove(builder.Length - 2, 2);
                return builder.ToString();
            }
        }

        public static bool operator &(TileStateFlags enum1, TileStateFlags enum2)
        {
            if (enum1 == null || enum2 == null)
            {
                return false;
            }

            bool hasEnum = (enum1.value & enum2.value) != 0;
            return hasEnum;
        }

        public int Value
        {
            get { return value; }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public override string ToString()
        {
            return this.name;
        }

        protected TileStateFlags(int value, string name)
        {
            this.value = value;
            this.name = name;
        }
    }

    public static class MinesweeperExtensionMethods
    {
        public static bool HasFlag(this TileStateFlags @this, TileStateFlags flagToCheck)
        {
            if (@this == null || flagToCheck == null)
            {
                return false;
            }

            return ((@this.Value & flagToCheck.Value) != 0);
        }
    }

    public enum SmileyState
    {
        Default,
        TapDown,
        Victory,
        GameOver
    }
}
