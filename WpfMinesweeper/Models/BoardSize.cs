namespace WpfMinesweeper.Models
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Runtime.Serialization;

    [Serializable, DataContract(Name = "BoardSize", IsReference = false), TypeConverter(typeof (BoardSizeConverter))]
    public struct BoardSize : IComparable<BoardSize>
    {
        private static BoardSize beginner = new BoardSize(9,
            9,
            10);

        private static BoardSize intermediate = new BoardSize(16,
            16,
            40);

        private static BoardSize expert = new BoardSize(30,
            16,
            99);

        private readonly int height;
        private readonly int mineCount;
        private readonly int width;

        public BoardSize(int width, int height, int mineCount)
        {
            this.width = width;
            this.height = height;
            this.mineCount = mineCount;
        }

        /// <summary>
        ///     Gets Height.
        /// </summary>
        public int Height
        {
            get
            {
                return this.height;
            }
        }

        /// <summary>
        ///     Gets MineCount.
        /// </summary>
        public int MineCount
        {
            get
            {
                return this.mineCount;
            }
        }

        /// <summary>
        ///     Gets Width.
        /// </summary>
        public int Width
        {
            get
            {
                return this.width;
            }
        }

        [IgnoreDataMember]
        public string Description
        {
            get
            {
                return string.Format(
                    @"{0}x{1}, {2} mines",
                    this.width,
                    this.height,
                    this.mineCount);
            }
        }

        public static BoardSize Beginner
        {
            get
            {
                return BoardSize.beginner;
            }
        }

        public static BoardSize Intermediate
        {
            get
            {
                return BoardSize.intermediate;
            }
        }

        public static BoardSize Expert
        {
            get
            {
                return BoardSize.expert;
            }
        }

        public int CompareTo(BoardSize other)
        {
            var thisTotal = this.width*this.height;
            var otherTotal = other.width*other.height;

            if (thisTotal == otherTotal)
            {
                return this.mineCount > other.mineCount ? 1 : this.mineCount < other.mineCount ? -1 : 0;
            }
            return thisTotal > otherTotal ? 1 : -1;
        }

        public override string ToString()
        {
            if (this == BoardSize.beginner)
            {
                return "Beginner";
            }
            if (this == BoardSize.intermediate)
            {
                return "Intermediate";
            }
            if (this == BoardSize.expert)
            {
                return "Expert";
            }
            return this.Description;
        }

        public static BoardSize Parse(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (string.Equals(
                text,
                BoardSize.beginner.ToString()))
            {
                return BoardSize.beginner;
            }
            if (string.Equals(
                text,
                BoardSize.intermediate.ToString()))
            {
                return BoardSize.intermediate;
            }
            if (string.Equals(
                text,
                BoardSize.expert.ToString()))
            {
                return BoardSize.expert;
            }
            var parts = text.Split(
                new[] {','},
                StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 3)
            {
                throw new FormatException("text must either be equal to the value of one of the static BoardSize properties or include width, height, and mine count delineated by commas between the numbers.");
            }
            return new BoardSize(int.Parse(
                parts[0]),
                int.Parse(
                    parts[1]),
                int.Parse(
                    parts[2]));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is BoardSize))
            {
                return false;
            }

            var board = (BoardSize)obj;
            return this == board;
        }

        public static bool operator ==(BoardSize bs1, BoardSize bs2)
        {
            return (bs1.width == bs2.width &&
                    bs1.height == bs2.height &&
                    bs1.mineCount == bs2.mineCount);
        }

        public static bool operator !=(BoardSize bs1, BoardSize bs2)
        {
            return (bs1.width != bs2.width ||
                    bs1.height != bs2.height ||
                    bs1.mineCount != bs2.mineCount);
        }
    }

    public class BoardSizeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof (string))
            {
                return true;
            }

            return base.CanConvertFrom(
                context,
                sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (value.GetType() == typeof (string))
            {
                return BoardSize.Parse(
                    value.ToString());
            }
            return base.ConvertFrom(
                context,
                culture,
                value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof (string))
            {
                return true;
            }

            return base.CanConvertTo(
                context,
                destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var boardSize = (BoardSize)value;
            if (destinationType == typeof (string))
            {
                return string.Format(
                    "{0},{1},{2}",
                    boardSize.Width,
                    boardSize.Height,
                    boardSize.MineCount);
            }

            return base.ConvertTo(
                context,
                culture,
                value,
                destinationType);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return false;
        }
    }
}
