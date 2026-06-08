using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace BlocksFamilyPlugin.Services
{
    /// <summary>
    /// JSON-backed favorites store persisted in LocalApplicationData.
    /// </summary>
    public sealed class FavoritesService : IFavoritesService
    {
        private static readonly string StorePath =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "BlocksFamilyPlugin", "favorites.json");

        private readonly HashSet<string> _ids;

        public FavoritesService()
        {
            _ids = Load();
        }

        public IReadOnlyCollection<string> FavoriteIds => _ids;

        public bool IsFavorite(string familyId) => _ids.Contains(familyId);

        public void Toggle(string familyId)
        {
            if (!_ids.Remove(familyId))
                _ids.Add(familyId);
            Save();
        }

        public void Save()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(StorePath)!);
            File.WriteAllText(StorePath, JsonConvert.SerializeObject(_ids));
        }

        // ─────────────────────────────────────────────────────────────────────

        private static HashSet<string> Load()
        {
            if (!File.Exists(StorePath))
                return new HashSet<string>();

            try
            {
                var json = File.ReadAllText(StorePath);
                return JsonConvert.DeserializeObject<HashSet<string>>(json)
                       ?? new HashSet<string>();
            }
            catch
            {
                return new HashSet<string>();
            }
        }
    }
}
