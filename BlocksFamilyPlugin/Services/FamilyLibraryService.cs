using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using BlocksFamilyPlugin.Models;
using Newtonsoft.Json;

namespace BlocksFamilyPlugin.Services
{
    /// <summary>
    /// Reads the family library from an embedded resource (FamilyLibrary.json
    /// compiled into the DLL), so no file-system path resolution is needed.
    /// Downloads remote .rfa files into the user's local cache folder.
    /// </summary>
    public sealed class FamilyLibraryService : IFamilyLibraryService
    {
        // ── Embedded resource name (matches <LogicalName> in .csproj) ────────
        private const string EmbeddedJsonName =
            "BlocksFamilyPlugin.Resources.FamilyLibrary.json";

        // ── Cache folder for downloaded .rfa files ────────────────────────────
        private static readonly string CacheRoot =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "BlocksFamilyPlugin", "FamilyCache");

        // ── Lazy-loaded index ─────────────────────────────────────────────────
        private FamilyLibraryIndex? _index;
        private readonly HttpClient _http = new();

        // ── Public API ────────────────────────────────────────────────────────

        public async Task<IReadOnlyList<FamilyCategory>> GetCategoriesAsync()
        {
            var index = await LoadIndexAsync();
            return index.Categories.AsReadOnly();
        }

        public async Task<IReadOnlyList<FamilyItem>> GetFamiliesAsync(string? category = null)
        {
            var index = await LoadIndexAsync();
            var list  = index.Families.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(category) &&
                !string.Equals(category, "All", StringComparison.OrdinalIgnoreCase))
            {
                list = list.Where(f =>
                    string.Equals(f.Category, category, StringComparison.OrdinalIgnoreCase));
            }

            return list.ToList().AsReadOnly();
        }

        public async Task<IReadOnlyList<FamilyItem>> SearchAsync(
            string query, string? category = null)
        {
            var families = await GetFamiliesAsync(category);
            if (string.IsNullOrWhiteSpace(query)) return families;

            var lower = query.ToLowerInvariant();
            return families
                .Where(f =>
                    f.Name.ToLowerInvariant().Contains(lower)         ||
                    f.Description.ToLowerInvariant().Contains(lower)  ||
                    f.Manufacturer.ToLowerInvariant().Contains(lower) ||
                    f.Tags.Any(t => t.ToLowerInvariant().Contains(lower)))
                .ToList()
                .AsReadOnly();
        }

        public async Task<string> EnsureLocalRfaAsync(FamilyItem family)
        {
            // Already a local file?
            if (File.Exists(family.RfaPath))
                return family.RfaPath;

            // Cached from a previous download?
            var cachedPath = Path.Combine(CacheRoot, family.Id + ".rfa");
            if (File.Exists(cachedPath))
                return cachedPath;

            // Download from URL
            if (!Uri.TryCreate(family.RfaPath, UriKind.Absolute, out var uri))
                throw new InvalidOperationException(
                    $"RfaPath '{family.RfaPath}' is neither a local file nor a valid URL.");

            Directory.CreateDirectory(CacheRoot);
            var bytes = await _http.GetByteArrayAsync(uri);
            await File.WriteAllBytesAsync(cachedPath, bytes);
            return cachedPath;
        }

        // ── Index loading ─────────────────────────────────────────────────────

        private async Task<FamilyLibraryIndex> LoadIndexAsync()
        {
            if (_index is not null) return _index;

            var json = await ReadEmbeddedJsonAsync();
            _index   = JsonConvert.DeserializeObject<FamilyLibraryIndex>(json)
                       ?? throw new InvalidDataException(
                           "FamilyLibrary.json is empty or invalid.");
            return _index;
        }

        /// <summary>
        /// Reads FamilyLibrary.json from the embedded manifest resource stream.
        /// This works regardless of where or how the DLL is loaded by Revit.
        /// </summary>
        private static async Task<string> ReadEmbeddedJsonAsync()
        {
            var assembly = typeof(FamilyLibraryService).Assembly;
            using var stream = assembly.GetManifestResourceStream(EmbeddedJsonName)
                ?? throw new InvalidOperationException(
                    $"Embedded resource '{EmbeddedJsonName}' not found in assembly. "  +
                    $"Available: {string.Join(", ", assembly.GetManifestResourceNames())}");

            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }
    }
}
