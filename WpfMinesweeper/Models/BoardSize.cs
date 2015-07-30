namespace WpfMinesweeper.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;

    [Serializable, DataContract(Name="BoardSize", IsReference=false), TypeConverter(typeof(BoardSizeConverter))]
    public struct BoardSize
    {
        private static BoardSize beginner = new BoardSize(9, 9, 10);
        private static BoardSize intermediate = new BoardSize(16, 16, 40);
        private static BoardSize expert = new BoardSize(30, 16, 99);
        private int height;
        private int mineCount;
        private int width;

        public BoardSize(int width, int height, int mineCount)
        {
            this.width = width;
            this.height = height;
            this.mineCount = mineCount;
        }

        /// <summary>
        /// Gets Height.
        /// </summary>
        public int Height
        {
            get
            {
                return this.height;
            }
        }

        /// <summary>
        /// Gets MineCount.
        /// </summary>
        public int MineCount
        {
            get
            {
                return this.mineCount;
            }
        }

        /// <summary>
        /// Gets Width.
        /// </summary>
        public int Width
        {
            get
            {
                return this.width;
            }
        }

        public override string ToString()
        {
            if (this == beginner)
            {
                return "Beginner";
            }
            else if (this == intermediate)
            {
                return "Intermediate";
            }
            else if (this == expert)
            {
                return "Expert";
            }
            else
            {
                return this.Description;
            }
        }

        [IgnoreDataMember]
        public string Description
        {
            get
            {
                return string.Format(@"{0}x{1}, {2} mines", this.width, this.height, this.mineCount);
            }
        }

        public static BoardSize Beginner
        {
            get
            {
                return beginner;
            }
        }

        public static BoardSize Intermediate
        {
            get
            {
                return intermediate;
            }
        }

        public static BoardSize Expert
        {
            get
            {
                return expert;
            }
        }

        public static BoardSize Parse(string text)
        {
            if (string.Equals(text, beginner.ToString()))
            {
                return beginner;
            }
            else if (string.Equals(text, intermediate.ToString()))
            {
                return intermediate;
            }
            else if (string.Equals(text, expert.ToString()))
            {
                return expert;
            }
            else
            {
                var parts = text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                return new BoardSize(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
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
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(string))
            {
                return BoardSize.Parse(value.ToString());
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            var boardSize = (BoardSize)value;
            if (destinationType == typeof(string))
            {
                return string.Format("{0},{1},{2}", boardSize.Width, boardSize.Height, boardSize.MineCount);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }


        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return false;
        }
    }
}
