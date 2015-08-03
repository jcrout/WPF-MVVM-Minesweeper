using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WpfMinesweeper
{
    public class ThicknessBindingConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new Thickness(
                values.Length > 0 ? (double)values[0] : 0d,
                values.Length > 1 ? (double)values[1] : 0d,
                values.Length > 2 ? (double)values[2] : 0d,
                values.Length > 3 ? (double)values[3] : 0d
                );
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
