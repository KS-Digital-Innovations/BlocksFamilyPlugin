using System.Collections.Generic;

namespace BlocksFamilyPlugin.Models
{
    /// <summary>
    /// Represents a single BIM family available in the library.
    /// </summary>
    public class FamilyItem
    {
        public string Id { get; set; } = string.Empty;

        /// <summary>Display name shown in the panel.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Revit category name, e.g. "Furniture", "Doors", "Lighting Fixtures".</summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>Optional sub-category label.</summary>
        public string SubCategory { get; set; } = string.Empty;

        /// <summary>Short description shown on hover / detail view.</summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>Manufacturer / brand name.</summary>
        public string Manufacturer { get; set; } = string.Empty;

        /// <summary>URI to a thumbnail image (URL or local path).</summary>
        public string ThumbnailUrl { get; set; } = string.Empty;

        /// <summary>
        /// Download URL or local file path to the .rfa file.
        /// Supports http/https for cloud-hosted libraries.
        /// </summary>
        public string RfaPath { get; set; } = string.Empty;

        /// <summary>Tags used for full-text search.</summary>
        public List<string> Tags { get; set; } = new();

        /// <summary>Whether this family requires a Premium subscription.</summary>
        public bool IsPremium { get; set; }

        /// <summary>How many times this family has been downloaded/used globally.</summary>
        public int DownloadCount { get; set; }

        /// <summary>Star rating 0–5.</summary>
        public double Rating { get; set; }

        /// <summary>Revit version compatibility string, e.g. "2022+".</summary>
        public string RevitVersion { get; set; } = "2022+";
    }
}
