using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BlocksFamilyPlugin.Helpers;
using BlocksFamilyPlugin.Models;
using BlocksFamilyPlugin.Services;

namespace BlocksFamilyPlugin.ViewModels
{
    /// <summary>
    /// Root ViewModel for the dockable panel.
    /// Owns category navigation, search, and the visible family card grid.
    /// </summary>
    public sealed class MainViewModel : BaseViewModel
    {
        // ── Dependencies ──────────────────────────────────────────────────────
        private readonly IFamilyLibraryService _library;
        private readonly IFavoritesService _favorites;
        private readonly IRevitFamilyLoader _loader;

        // ── Backing fields ────────────────────────────────────────────────────
        private string _searchText = string.Empty;
        private string _selectedCategory = "All";
        private bool _showFavoritesOnly;
        private bool _isLoading;
        private string _statusMessage = string.Empty;

        // ── Observable collections ────────────────────────────────────────────

        public ObservableCollection<string> Categories { get; } = new();
        public ObservableCollection<FamilyCardViewModel> FamilyCards { get; } = new();

        // ── Bindable properties ───────────────────────────────────────────────

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                    _ = RefreshFamiliesAsync();
            }
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value))
                    _ = RefreshFamiliesAsync();
            }
        }

        public bool ShowFavoritesOnly
        {
            get => _showFavoritesOnly;
            set
            {
                if (SetProperty(ref _showFavoritesOnly, value))
                    _ = RefreshFamiliesAsync();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            private set => SetProperty(ref _isLoading, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            private set => SetProperty(ref _statusMessage, value);
        }

        public int TotalFamilies => FamilyCards.Count;

        // ── Toast notification ────────────────────────────────────────────────

        private string _toastMessage = string.Empty;
        private bool   _toastIsError;
        private bool   _toastVisible;

        public string ToastMessage
        {
            get => _toastMessage;
            private set => SetProperty(ref _toastMessage, value);
        }

        public bool ToastIsError
        {
            get => _toastIsError;
            private set => SetProperty(ref _toastIsError, value);
        }

        public bool ToastVisible
        {
            get => _toastVisible;
            private set => SetProperty(ref _toastVisible, value);
        }

        /// <summary>Shows a toast for 3 seconds then auto-hides it.</summary>
        private async System.Threading.Tasks.Task ShowToastAsync(
            string message, bool isError = false)
        {
            ToastMessage = message;
            ToastIsError = isError;
            ToastVisible = true;
            await System.Threading.Tasks.Task.Delay(3000);
            ToastVisible = false;
        }

        // ── Commands ──────────────────────────────────────────────────────────

        public ICommand LoadCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand SelectCategoryCommand { get; }

        // ─────────────────────────────────────────────────────────────────────

        public MainViewModel(
            IFamilyLibraryService library,
            IFavoritesService favorites,
            IRevitFamilyLoader loader)
        {
            _library = library;
            _favorites = favorites;
            _loader = loader;

            LoadCommand = new AsyncRelayCommand(async _ => await InitialLoadAsync());
            ClearSearchCommand = new RelayCommand(() => SearchText = string.Empty);
            SelectCategoryCommand = new RelayCommand(
                param => SelectedCategory = param as string ?? "All");
        }

        // ── Initialisation ────────────────────────────────────────────────────

        public async Task InitialLoadAsync()
        {
            IsLoading = true;
            StatusMessage = "Loading library…";

            try
            {
                // Populate the category sidebar
                var categories = await _library.GetCategoriesAsync();
                Categories.Clear();
                Categories.Add("All");
                Categories.Add("Favorites");
                foreach (var cat in categories)
                    Categories.Add(cat.Name);

                await RefreshFamiliesAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading library: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        // ── Family refresh ────────────────────────────────────────────────────

        public async Task RefreshFamiliesAsync()
        {
            IsLoading = true;

            try
            {
                IReadOnlyList<FamilyItem> families;

                var isFavoritesTab = string.Equals(
                    _selectedCategory, "Favorites", StringComparison.OrdinalIgnoreCase);

                if (isFavoritesTab || _showFavoritesOnly)
                {
                    families = await _library.GetFamiliesAsync();
                    families = families
                        .Where(f => _favorites.IsFavorite(f.Id))
                        .ToList();
                }
                else if (!string.IsNullOrWhiteSpace(_searchText))
                {
                    families = await _library.SearchAsync(_searchText, _selectedCategory);
                }
                else
                {
                    families = await _library.GetFamiliesAsync(_selectedCategory);
                }

                // Build card view-models
                FamilyCards.Clear();
                foreach (var f in families)
                    FamilyCards.Add(new FamilyCardViewModel(f, _favorites, PlaceFamilyAsync));

                OnPropertyChanged(nameof(TotalFamilies));
                StatusMessage = FamilyCards.Count == 0
                    ? "No families found."
                    : $"{FamilyCards.Count} families";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        // ── Placement ─────────────────────────────────────────────────────────

        private async Task PlaceFamilyAsync(FamilyItem family)
        {
            StatusMessage = $"Placing '{family.Name}'…";
            try
            {
                await _loader.LoadAndPlaceAsync(family);
                StatusMessage = $"'{family.Name}' placed successfully.";
                _ = ShowToastAsync($"✔  '{family.Name}' loaded into project");
            }
            catch (Exception ex)
            {
                StatusMessage = $"Could not place family: {ex.Message}";
                _ = ShowToastAsync($"✘  {ex.Message}", isError: true);
            }
        }
    }
}
