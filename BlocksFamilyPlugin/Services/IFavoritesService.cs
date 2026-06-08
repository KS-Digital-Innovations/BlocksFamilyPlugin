using System.Collections.Generic;

namespace BlocksFamilyPlugin.Services
{
    /// <summary>
    /// Persists the user's bookmarked / favourite family IDs between sessions.
    /// </summary>
    public interface IFavoritesService
    {
        IReadOnlyCollection<string> FavoriteIds { get; }
        bool IsFavorite(string familyId);
        void Toggle(string familyId);
        void Save();
    }
}
