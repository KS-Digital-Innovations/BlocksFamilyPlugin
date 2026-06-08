using System.Collections.Generic;
using System.Threading.Tasks;
using BlocksFamilyPlugin.Models;

namespace BlocksFamilyPlugin.Services
{
    /// <summary>
    /// Provides access to the remote/local family library index.
    /// Swap the implementation for a cloud-backed version without changing ViewModels.
    /// </summary>
    public interface IFamilyLibraryService
    {
        /// <summary>Load all categories from the library index.</summary>
        Task<IReadOnlyList<FamilyCategory>> GetCategoriesAsync();

        /// <summary>Load all family items, optionally filtered by category.</summary>
        Task<IReadOnlyList<FamilyItem>> GetFamiliesAsync(string? category = null);

        /// <summary>Full-text search across name, tags, manufacturer and description.</summary>
        Task<IReadOnlyList<FamilyItem>> SearchAsync(string query, string? category = null);

        /// <summary>Download (or locate) the .rfa file and return its local path.</summary>
        Task<string> EnsureLocalRfaAsync(FamilyItem family);
    }
}
