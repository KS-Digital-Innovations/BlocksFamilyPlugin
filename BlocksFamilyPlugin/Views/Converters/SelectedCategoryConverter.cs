using System;
using System.Globalization;
using System.Windows.Data;

namespace BlocksFamilyPlugin.Views.Converters
{
    /// <summary>
    /// Returns true when the bound SelectedCategory equals the converter parameter.
    /// Used to switch between CategoryButton / CategoryButtonActive styles in the sidebar.
    /// </summary>
    public sealed class SelectedCategoryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => string.Equals(value as string, parameter as string, StringComparison.OrdinalIgnoreCase);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
