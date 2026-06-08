using System.Windows.Input;
using BlocksFamilyPlugin.Helpers;
using BlocksFamilyPlugin.Models;
using BlocksFamilyPlugin.Services;

namespace BlocksFamilyPlugin.ViewModels
{
    /// <summary>
    /// ViewModel for a single family card in the grid.
    /// Exposes favorite-toggle and place commands back to the parent.
    /// </summary>
    public sealed class FamilyCardViewModel : BaseViewModel
    {
        private readonly IFavoritesService _favorites;

        // ── Bindable properties ───────────────────────────────────────────────

        public FamilyItem Family { get; }

        public string Name => Family.Name;
        public string Category => Family.Category;
        public string ThumbnailUrl => Family.ThumbnailUrl;
        public bool IsPremium => Family.IsPremium;
        public double Rating => Family.Rating;
        public int DownloadCount => Family.DownloadCount;

        private bool _isFavorite;
        public bool IsFavorite
        {
            get => _isFavorite;
            private set => SetProperty(ref _isFavorite, value);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            private set => SetProperty(ref _isBusy, value);
        }

        // ── Commands ──────────────────────────────────────────────────────────

        public ICommand ToggleFavoriteCommand { get; }

        /// <summary>Raised by the parent MainViewModel when this card is clicked.</summary>
        public ICommand PlaceCommand { get; }

        // ─────────────────────────────────────────────────────────────────────

        public FamilyCardViewModel(
            FamilyItem family,
            IFavoritesService favorites,
            System.Func<FamilyItem, System.Threading.Tasks.Task> onPlace)
        {
            Family = family;
            _favorites = favorites;
            _isFavorite = favorites.IsFavorite(family.Id);

            ToggleFavoriteCommand = new RelayCommand(OnToggleFavorite);
            PlaceCommand = new AsyncRelayCommand(
                async _ =>
                {
                    IsBusy = true;
                    try { await onPlace(Family); }
                    finally { IsBusy = false; }
                });
        }

        private void OnToggleFavorite(object? _)
        {
            _favorites.Toggle(Family.Id);
            IsFavorite = _favorites.IsFavorite(Family.Id);
        }
    }
}
