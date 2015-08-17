namespace WpfMinesweeper.Controls
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;

    public class MessageButtonConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(Button))
            {
                return true;
            }
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(Button))
            {
                return true;
            }
            if (destinationType == typeof(string))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var valueType = value.GetType();
            if (valueType == typeof(Button))
            {
                var button = (Button)value;
                return new MessageButton {Button = button};
            }
            if (valueType == typeof(string))
            {
                var textFragments = value.ToString().Split(';');
                var button = new Button {Content = textFragments[0]};
                var mbutton = new MessageButton {Button = button};

                button.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                button.Width = Math.Max(80, button.DesiredSize.Width);
                button.Height = Math.Max(22, button.DesiredSize.Height);

                if (textFragments.Length > 1)
                {
                    var rightToLeftIndex = 0;
                    if (int.TryParse(textFragments[1], out rightToLeftIndex))
                    {
                        mbutton.RightToLeftIndex = rightToLeftIndex;
                    }
                }

                if (textFragments.Length > 2)
                {
                    var resultFragment = textFragments[2].ToUpper();
                    if (string.Equals(resultFragment, "OK"))
                    {
                        mbutton.Result = MessageResult.OK;
                    }
                    else if (string.Equals(resultFragment, "CANCEL"))
                    {
                        mbutton.Result = MessageResult.Cancel;
                    }
                    else if (string.Equals(resultFragment, "OTHER"))
                    {
                        mbutton.Result = MessageResult.Other;
                    }
                    else if (string.Equals(resultFragment, "NULL"))
                    {
                        mbutton.Result = null;
                    }
                    else if (!string.Equals(resultFragment, string.Empty))
                    {
                        mbutton.Result = textFragments[2];
                    }
                }

                return mbutton;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return false;
        }
    }
}
