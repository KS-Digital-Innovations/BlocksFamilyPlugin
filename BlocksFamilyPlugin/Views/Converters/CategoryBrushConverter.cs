using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace BlocksFamilyPlugin.Views.Converters
{
    /// <summary>
    /// Converts a category name to a LinearGradientBrush for the card thumbnail.
    /// Returns a ready-to-use Brush — no need for a MultiBinding.
    /// </summary>
    public sealed class CategoryGradientConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var (start, end) = (value as string) switch
            {
                "Furniture"          => ("#FF8B1A2E", "#FF5C0A16"),
                "Doors"              => ("#FF1A5276", "#FF0D2B45"),
                "Windows"            => ("#FF1E8449", "#FF0B4B27"),
                "Lighting"           => ("#FFB7770D", "#FF7A4F08"),
                "Plumbing"           => ("#FF117A65", "#FF074F42"),
                "Structural Framing" => ("#FF6E2F8E", "#FF461B5E"),
                "Casework"           => ("#FF782810", "#FF4A1608"),
                "Electrical"         => ("#FF1C398E", "#FF0C1F5C"),
                "Mechanical"         => ("#FF424242", "#FF1A1A1A"),
                "Site"               => ("#FF216130", "#FF0E3B1C"),
                "Favorites"          => ("#FF9A6B00", "#FF5E4000"),
                _                    => ("#FF6B0F1A", "#FF3D060E"),
            };

            var brush = new LinearGradientBrush
            {
                StartPoint = new System.Windows.Point(0, 0),
                EndPoint   = new System.Windows.Point(1, 1),
            };
            brush.GradientStops.Add(new GradientStop(
                (Color)ColorConverter.ConvertFromString(start), 0.0));
            brush.GradientStops.Add(new GradientStop(
                (Color)ColorConverter.ConvertFromString(end), 1.0));
            brush.Freeze();
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
