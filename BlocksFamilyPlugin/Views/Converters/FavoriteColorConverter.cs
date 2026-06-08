using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace BlocksFamilyPlugin.Views.Converters
{
    /// <summary>True → gold star, False → grey star.</summary>
    [ValueConversion(typeof(bool), typeof(Brush))]
    public sealed class FavoriteColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is true
                ? new SolidColorBrush(Color.FromRgb(0xFF, 0xD7, 0x00))   // gold
                : new SolidColorBrush(Color.FromRgb(0x66, 0x66, 0x88));  // grey

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
