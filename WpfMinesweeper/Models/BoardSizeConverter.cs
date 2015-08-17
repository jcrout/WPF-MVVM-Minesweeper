namespace WpfMinesweeper.Models
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

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
                throw new ArgumentNullException(nameof(value));
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
