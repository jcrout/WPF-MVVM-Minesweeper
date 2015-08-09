namespace WpfMinesweeper.Models
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Runtime.Serialization;

    /// <summary>
    ///     Represents the number of tiles and the mine count of a Minesweeper
    ///     board.
    /// </summary>
    [Serializable, DataContract(Name = "BoardSize", IsReference = false), TypeConverter(typeof(BoardSizeConverter))]
    public struct BoardSize : IComparable<BoardSize>
    {
        private static BoardSize beginner = new BoardSize(9, 9, 10);
        private static BoardSize expert = new BoardSize(30, 16, 99);
        private static BoardSize intermediate = new BoardSize(16, 16, 40);
        private readonly int height;
        private readonly int mineCount;
        private readonly int width;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BoardSize" /> struct.
        /// </summary>
        /// <param name="width">The number of tiles per row.</param>
        /// <param name="height">The number of tiles per column.</param>
        /// <param name="mineCount">The total number of mines on the board.</param>
        public BoardSize(int width, int height, int mineCount)
        {
            this.width = width;
            this.height = height;
            this.mineCount = mineCount;
        }

        /// <summary>
        ///     Gets the Beginner-sized board, which is 9x9 with 10 mines.
        /// </summary>
        public static BoardSize Beginner
        {
            get
            {
                return BoardSize.beginner;
            }
        }

        /// <summary>
        ///     Gets the Intermediate-sized board, which is 16x16 with 40 mines.
        /// </summary>
        public static BoardSize Intermediate
        {
            get
            {
                return BoardSize.intermediate;
            }
        }

        /// <summary>
        ///     Gets the Expert-sized board, which is 30x24 with 99 mines.
        /// </summary>
        public static BoardSize Expert
        {
            get
            {
                return BoardSize.expert;
            }
        }

        /// <summary>
        ///     Gets a description of the board.
        /// </summary>
        /// <value>The description of the board in the format of WxH, M mines.</value>
        [IgnoreDataMember]
        public string Description
        {
            get
            {
                return string.Format(@"{0}x{1}, {2} mines", this.width, this.height, this.mineCount);
            }
        }

        /// <summary>
        ///     Gets the number of tiles per column.
        /// </summary>
        public int Height
        {
            get
            {
                return this.height;
            }
        }

        /// <summary>
        ///     Gets the total number of mines on the board.
        /// </summary>
        public int MineCount
        {
            get
            {
                return this.mineCount;
            }
        }

        /// <summary>
        ///     Gets the number of tiles per row.
        /// </summary>
        public int Width
        {
            get
            {
                return this.width;
            }
        }

        /// <summary>
        ///     Converts the string representation of a BoardSize into a new BoardSize instance.
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <returns>A new BoardSize instance.</returns>
        /// <exception cref="System.ArgumentNullException">text is <c>null</c>.</exception>
        /// <exception cref="System.FormatException">
        ///     text must either be equal to one of the following:  +
        ///     the value of one of the static BoardSize properties;  +
        ///     a BoardSize.Description string;  +
        ///     a string including the width, height, and mine count delineated by commas between the numbers.
        /// </exception>
        public static BoardSize Parse(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            if (string.Equals(text, BoardSize.beginner.ToString()))
            {
                return BoardSize.beginner;
            }

            if (string.Equals(text, BoardSize.intermediate.ToString()))
            {
                return BoardSize.intermediate;
            }

            if (string.Equals(text, BoardSize.expert.ToString()))
            {
                return BoardSize.expert;
            }

            var parts = text.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2)
            {
                int indexOfFirstDelimiter = parts[0].IndexOf("x", StringComparison.Ordinal);
                int indexOfSecondDelimiter = parts[1].IndexOf(" mines", StringComparison.Ordinal);

                return new BoardSize(int.Parse(parts[0].Substring(0, indexOfFirstDelimiter)), int.Parse(parts[0].Substring(indexOfFirstDelimiter + 1)), int.Parse(parts[1].Substring(0, indexOfSecondDelimiter)));
            }
            if (parts.Length == 3)
            {
                return new BoardSize(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
            }
            throw new FormatException("text must either be equal to one of the following: " + "the value of one of the static BoardSize properties; " + "a BoardSize.Description string; " + "a string including the width, height, and mine count delineated by commas between the numbers.");
        }

        /// <summary>
        ///     Implements the == operator.
        /// </summary>
        /// <param name="bs1">The left-hand BoardSize instance.</param>
        /// <param name="bs2">The right-hand BoardSize instance</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(BoardSize bs1, BoardSize bs2)
        {
            return bs1.width == bs2.width && bs1.height == bs2.height && bs1.mineCount == bs2.mineCount;
        }

        /// <summary>
        ///     Implements the != operator.
        /// </summary>
        /// <param name="bs1">The left-hand BoardSize instance.</param>
        /// <param name="bs2">The right-hand BoardSize instance</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(BoardSize bs1, BoardSize bs2)
        {
            return bs1.width != bs2.width || bs1.height != bs2.height || bs1.mineCount != bs2.mineCount;
        }

        /// <summary>
        ///     Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///     A value that indicates the relative order of the objects being compared. The return value has the following
        ///     meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This
        ///     object is equal to <paramref name="other" />. Greater than zero This object is greater than
        ///     <paramref name="other" />.
        /// </returns>
        public int CompareTo(BoardSize other)
        {
            var thisTotal = this.width * this.height;
            var otherTotal = other.width * other.height;

            if (thisTotal == otherTotal)
            {
                return this.mineCount > other.mineCount ? 1 : this.mineCount < other.mineCount ? -1 : 0;
            }

            return thisTotal > otherTotal ? 1 : -1;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is BoardSize))
            {
                return false;
            }

            var board = (BoardSize)obj;
            return this == board;
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
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
    }

    /// <summary>
    ///     Provides a way of converting values to BoardSize instances.
    /// </summary>
    public class BoardSizeConverter : TypeConverter
    {
        /// <summary>
        ///     Returns whether this converter can convert an object of the given type to the type of this converter, using the
        ///     specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type" /> that represents the type you want to convert from.</param>
        /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        ///     Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="T:System.Type" /> that represents the type you want to convert to.</param>
        /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        ///     Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo" /> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
        /// <returns>An <see cref="T:System.Object" /> that represents the converted value.</returns>
        /// <exception cref="System.ArgumentNullException">value is <c>null</c>.</exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (value.GetType() == typeof(string))
            {
                return BoardSize.Parse(value.ToString());
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        ///     Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="culture">
        ///     A <see cref="T:System.Globalization.CultureInfo" />. If null is passed, the current culture is
        ///     assumed.
        /// </param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
        /// <param name="destinationType">The <see cref="T:System.Type" /> to convert the <paramref name="value" /> parameter to.</param>
        /// <returns>An <see cref="T:System.Object" /> that represents the converted value.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var boardSize = (BoardSize)value;
            if (destinationType == typeof(string))
            {
                return string.Format("{0},{1},{2}", boardSize.Width, boardSize.Height, boardSize.MineCount);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        ///     Returns whether this object supports a standard set of values that can be picked from a list, using the specified
        ///     context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <returns>
        ///     true if <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues" /> should be called to find a
        ///     common set of values the object supports; otherwise, false.
        /// </returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return false;
        }
    }
}
