using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace BlocksFamilyPlugin.Views.Converters
{
    /// <summary>
    /// Converts a category name to a WPF PathGeometry for the card icon.
    /// All icons are drawn in a 24×24 coordinate space using stroke style —
    /// clean, modern, and crisp at any DPI.
    /// </summary>
    public sealed class FamilyIconGeometryConverter : IValueConverter
    {
        // ── Icon path data (24×24 viewbox, stroke style) ──────────────────────

        private const string Chair =
            "M 7 13 L 7 9 C 7 7.34 8.34 6 10 6 L 14 6 C 15.66 6 17 7.34 17 9 L 17 13 " +
            "M 5 13 L 19 13 M 5 13 C 3.9 13 3 13.9 3 15 L 3 17 L 21 17 L 21 15 " +
            "C 21 13.9 20.1 13 19 13 M 6 17 L 6 21 M 18 17 L 18 21";

        private const string Door =
            "M 6 21 L 18 21 M 8 21 L 8 3 L 16 3 L 16 21 " +
            "M 14 12 C 14 11.45 13.55 11 13 11 A 1 1 0 1 0 13 13 C 13.55 13 14 12.55 14 12 Z";

        private const string Window =
            "M 3 4 L 21 4 L 21 20 L 3 20 Z M 12 4 L 12 20 M 3 12 L 21 12 " +
            "M 3 4 C 3 4 5 2 12 2 C 19 2 21 4 21 4";

        private const string Lightbulb =
            "M 9 21 L 15 21 M 10 18 L 14 18 " +
            "M 12 2 A 6 6 0 0 0 6 8 C 6 11.5 8.5 13.5 9.5 15.5 L 9.5 18 " +
            "L 14.5 18 L 14.5 15.5 C 15.5 13.5 18 11.5 18 8 A 6 6 0 0 0 12 2 Z";

        private const string WaterDrop =
            "M 12 2 L 5.5 12.5 A 6.5 6.5 0 1 0 18.5 12.5 Z " +
            "M 9.5 16 A 3 3 0 0 0 12 19";

        private const string Beam =
            "M 3 5 L 21 5 L 21 8 L 14 8 L 14 16 L 21 16 L 21 19 " +
            "L 3 19 L 3 16 L 10 16 L 10 8 L 3 8 Z";

        private const string Cabinet =
            "M 3 4 L 21 4 L 21 20 L 3 20 Z M 3 12 L 21 12 " +
            "M 12 4 L 12 20 M 10 8 L 8 8 M 14 8 L 16 8 " +
            "M 10 16 L 8 16 M 14 16 L 16 16";

        private const string Bolt =
            "M 13 2 L 4 14 L 11 14 L 11 22 L 20 10 L 13 10 Z";

        private const string Gear =
            "M 12 8 A 4 4 0 1 0 12 16 A 4 4 0 0 0 12 8 Z " +
            "M 12 2 L 12 4 M 12 20 L 12 22 M 4.22 4.22 L 5.64 5.64 " +
            "M 18.36 18.36 L 19.78 19.78 M 2 12 L 4 12 M 20 12 L 22 12 " +
            "M 4.22 19.78 L 5.64 18.36 M 18.36 5.64 L 19.78 4.22";

        private const string Tree =
            "M 12 2 L 5 10 L 9 10 L 4 18 L 10 18 L 10 22 L 14 22 " +
            "L 14 18 L 20 18 L 15 10 L 19 10 Z";

        private const string Star =
            "M 12 2 L 15.09 8.26 L 22 9.27 L 17 14.14 L 18.18 21.02 " +
            "L 12 17.77 L 5.82 21.02 L 7 14.14 L 2 9.27 L 8.91 8.26 Z";

        private const string AllIcon =
            "M 3 5 L 21 5 M 3 10 L 15 10 M 3 15 L 18 15 M 3 20 L 12 20";

        // ─────────────────────────────────────────────────────────────────────

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var path = (value as string) switch
            {
                "Furniture"          => Chair,
                "Doors"              => Door,
                "Windows"            => Window,
                "Lighting"           => Lightbulb,
                "Plumbing"           => WaterDrop,
                "Structural Framing" => Beam,
                "Casework"           => Cabinet,
                "Electrical"         => Bolt,
                "Mechanical"         => Gear,
                "Site"               => Tree,
                "Favorites"          => Star,
                "All"                => AllIcon,
                _                    => AllIcon,
            };

            try { return Geometry.Parse(path); }
            catch { return Geometry.Empty; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
