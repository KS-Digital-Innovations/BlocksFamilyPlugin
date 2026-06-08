using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace BlocksFamilyPlugin.Views.Converters
{
    // ── Icon ─────────────────────────────────────────────────────────────────

    /// <summary>Maps a category name to an emoji icon shown on the card thumbnail.</summary>
    public sealed class CategoryIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (value as string) switch
            {
                "Furniture"          => "🪑",
                "Doors"              => "🚪",
                "Windows"            => "🪟",
                "Lighting"           => "💡",
                "Plumbing"           => "🚿",
                "Structural Framing" => "🏗",
                "Casework"           => "🗄",
                "Electrical"         => "⚡",
                "Mechanical"         => "⚙",
                "Site"               => "🌳",
                "Entourage"          => "👥",
                "Parking"            => "🅿",
                "Favorites"          => "⭐",
                _                    => "📦",
            };

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    // ── Gradient start colour ─────────────────────────────────────────────────

    /// <summary>Maps a category to a gradient start colour for the thumbnail background.</summary>
    public sealed class CategoryColorStartConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (value as string) switch
            {
                "Furniture"          => new SolidColorBrush(Color.FromRgb(0x8B, 0x1A, 0x2E)),
                "Doors"              => new SolidColorBrush(Color.FromRgb(0x1A, 0x52, 0x76)),
                "Windows"            => new SolidColorBrush(Color.FromRgb(0x1E, 0x84, 0x49)),
                "Lighting"           => new SolidColorBrush(Color.FromRgb(0xB7, 0x77, 0x0D)),
                "Plumbing"           => new SolidColorBrush(Color.FromRgb(0x11, 0x7A, 0x65)),
                "Structural Framing" => new SolidColorBrush(Color.FromRgb(0x6E, 0x2F, 0x8E)),
                "Casework"           => new SolidColorBrush(Color.FromRgb(0x78, 0x28, 0x10)),
                "Electrical"         => new SolidColorBrush(Color.FromRgb(0x1C, 0x39, 0x8E)),
                "Mechanical"         => new SolidColorBrush(Color.FromRgb(0x35, 0x35, 0x35)),
                "Site"               => new SolidColorBrush(Color.FromRgb(0x21, 0x61, 0x30)),
                "Favorites"          => new SolidColorBrush(Color.FromRgb(0x9A, 0x6B, 0x00)),
                _                    => new SolidColorBrush(Color.FromRgb(0x6B, 0x0F, 0x1A)),
            };

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    /// <summary>Maps a category to a gradient end colour for the thumbnail background.</summary>
    public sealed class CategoryColorEndConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (value as string) switch
            {
                "Furniture"          => new SolidColorBrush(Color.FromRgb(0x5C, 0x0A, 0x16)),
                "Doors"              => new SolidColorBrush(Color.FromRgb(0x0D, 0x2B, 0x45)),
                "Windows"            => new SolidColorBrush(Color.FromRgb(0x0B, 0x4B, 0x27)),
                "Lighting"           => new SolidColorBrush(Color.FromRgb(0x7A, 0x4F, 0x08)),
                "Plumbing"           => new SolidColorBrush(Color.FromRgb(0x07, 0x4F, 0x42)),
                "Structural Framing" => new SolidColorBrush(Color.FromRgb(0x46, 0x1B, 0x5E)),
                "Casework"           => new SolidColorBrush(Color.FromRgb(0x4A, 0x16, 0x08)),
                "Electrical"         => new SolidColorBrush(Color.FromRgb(0x0C, 0x1F, 0x5C)),
                "Mechanical"         => new SolidColorBrush(Color.FromRgb(0x1A, 0x1A, 0x1A)),
                "Site"               => new SolidColorBrush(Color.FromRgb(0x0E, 0x3B, 0x1C)),
                "Favorites"          => new SolidColorBrush(Color.FromRgb(0x5E, 0x40, 0x00)),
                _                    => new SolidColorBrush(Color.FromRgb(0x3D, 0x06, 0x0E)),
            };

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    // ── Sidebar icon ──────────────────────────────────────────────────────────

    /// <summary>Maps a category name to a small sidebar emoji.</summary>
    public sealed class SidebarIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (value as string) switch
            {
                "All"                => "◈",
                "Favorites"          => "★",
                "Furniture"          => "🪑",
                "Doors"              => "🚪",
                "Windows"            => "🪟",
                "Lighting"           => "💡",
                "Plumbing"           => "🚿",
                "Structural Framing" => "🏗",
                "Casework"           => "🗄",
                "Electrical"         => "⚡",
                "Mechanical"         => "⚙",
                "Site"               => "🌳",
                _                    => "·",
            };

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
