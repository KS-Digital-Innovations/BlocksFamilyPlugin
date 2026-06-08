namespace BlocksFamilyPlugin.Models
{
    /// <summary>
    /// A top-level category grouping in the family library (e.g. Furniture, Doors, Lighting).
    /// </summary>
    public class FamilyCategory
    {
        public string Name { get; set; } = string.Empty;

        /// <summary>Icon identifier (maps to a resource key in Styles.xaml).</summary>
        public string IconKey { get; set; } = string.Empty;

        public int FamilyCount { get; set; }
    }
}
