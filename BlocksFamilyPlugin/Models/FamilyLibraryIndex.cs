using System.Collections.Generic;

namespace BlocksFamilyPlugin.Models
{
    /// <summary>
    /// Root object that is deserialized from FamilyLibrary.json.
    /// </summary>
    public class FamilyLibraryIndex
    {
        public string Version { get; set; } = "1.0";
        public List<FamilyCategory> Categories { get; set; } = new();
        public List<FamilyItem> Families { get; set; } = new();
    }
}
