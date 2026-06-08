using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace BlocksFamilyPlugin.Views.Converters
{
    /// <summary>
    /// Multi-value converter used by the category sidebar.
    /// values[0] = the category string for this item
    /// values[1] = MainViewModel.SelectedCategory
    /// Returns a Brush: highlighted when equal, transparent when not.
    /// </summary>
    public sealed class CategorySelectedBrushConverter : IMultiValueConverter
    {
        // These are set as static resources so XAML doesn't need to instantiate new brushes
        // Selected: white-tinted overlay on maroon; unselected: transparent
        private static readonly Brush ActiveBg    = new SolidColorBrush(Color.FromArgb(0x30, 0xFF, 0xFF, 0xFF));
        private static readonly Brush HoverBg     = new SolidColorBrush(Color.FromArgb(0x18, 0xFF, 0xFF, 0xFF));
        private static readonly Brush Transparent = Brushes.Transparent;

        static CategorySelectedBrushConverter()
        {
            ActiveBg.Freeze();
            HoverBg.Freeze();
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) return Transparent;
            var item     = values[0] as string ?? string.Empty;
            var selected = values[1] as string ?? string.Empty;
            return string.Equals(item, selected, StringComparison.OrdinalIgnoreCase)
                ? ActiveBg
                : Transparent;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    /// <summary>
    /// Returns SemiBold when values[0] == values[1], Normal otherwise.
    /// </summary>
    public sealed class CategorySelectedWeightConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) return System.Windows.FontWeights.Normal;
            return string.Equals(values[0] as string, values[1] as string,
                StringComparison.OrdinalIgnoreCase)
                ? System.Windows.FontWeights.SemiBold
                : System.Windows.FontWeights.Normal;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    /// <summary>
    /// Returns TextPrimary brush when equal, TextSecondary otherwise.
    /// </summary>
    public sealed class CategorySelectedForeConverter : IMultiValueConverter
    {
        // White for selected, muted pink-white for unselected (both on maroon)
        private static readonly Brush Primary   = new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF));
        private static readonly Brush Secondary = new SolidColorBrush(Color.FromRgb(0xEA, 0xC0, 0xC8));

        static CategorySelectedForeConverter()
        {
            Primary.Freeze();
            Secondary.Freeze();
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) return Secondary;
            return string.Equals(values[0] as string, values[1] as string,
                StringComparison.OrdinalIgnoreCase)
                ? Primary
                : Secondary;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
