using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using BlocksFamilyPlugin.Services;
using BlocksFamilyPlugin.ViewModels;
using BlocksFamilyPlugin.Views;

namespace BlocksFamilyPlugin
{
    /// <summary>
    /// Entry point registered in the .addin manifest.
    /// Responsible for:
    ///   1. Registering the dockable pane provider
    ///   2. Adding the ribbon tab and button
    ///   3. Wiring up services and the root ViewModel
    /// </summary>
    public sealed class App : IExternalApplication
    {
        // ── Public API used by OpenPanelCommand ───────────────────────────────
        public static DockablePaneId PanelId { get; } =
            new DockablePaneId(new Guid("B1C2D3E4-F5A6-7890-BCDE-F12345678901"));

        // ── Services (singleton for the session) ─────────────────────────────
        private static IFamilyLibraryService? _libraryService;
        private static IFavoritesService?     _favoritesService;
        private static MainViewModel?         _mainViewModel;
        private static MainPanel?             _mainPanel;

        // ─────────────────────────────────────────────────────────────────────

        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                // 0. Load Styles.xaml into Application.Current.Resources so every
                //    UserControl can use DynamicResource keys (BgDeep, Accent, etc.)
                //    without needing a MergedDictionary in each view.
                LoadGlobalStyles();

                // 1. Compose services
                _libraryService   = new FamilyLibraryService();
                _favoritesService = new FavoritesService();

                // RevitFamilyLoader needs a UIApplication — created lazily on
                // first use so it captures the live session (see PanelProvider).

                // 2. Build ViewModel – loader is null until first ribbon click
                //    (UIApplication is unavailable at startup time).
                //    InjectLoader() replaces it once OpenPanelCommand runs.
                _mainViewModel = new MainViewModel(
                    _libraryService,
                    _favoritesService,
                    new NullRevitFamilyLoader());

                // 3. Register dockable pane
                application.RegisterDockablePane(
                    PanelId,
                    "Blocks Family Library",
                    new PanelProvider());

                // 4. Add ribbon UI
                CreateRibbon(application);

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Blocks Plugin Error",
                    $"Failed to initialise Blocks Family Plugin:\n\n{ex.Message}");
                return Result.Failed;
            }
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            _favoritesService?.Save();
            return Result.Succeeded;
        }

        // ── Global styles ─────────────────────────────────────────────────────

        private static void LoadGlobalStyles()
        {
            try
            {
                // Styles.xaml is compiled as BAML — load via pack URI, no file path needed.
                var uri  = new Uri(
                    "pack://application:,,,/BlocksFamilyPlugin;component/BlocksFamilyPlugin/Resources/Styles.xaml",
                    UriKind.Absolute);
                var dict = new ResourceDictionary { Source = uri };
                Application.Current.Resources.MergedDictionaries.Add(dict);
            }
            catch
            {
                // Styles are cosmetic – swallow so the plugin still loads
            }
        }

        // ── Ribbon ────────────────────────────────────────────────────────────

        private static void CreateRibbon(UIControlledApplication app)
        {
            const string tabName    = "Blocks";
            const string panelName  = "Family Library";

            app.CreateRibbonTab(tabName);
            var ribbonPanel = app.CreateRibbonPanel(tabName, panelName);

            var assemblyPath = Assembly.GetExecutingAssembly().Location;

            var buttonData = new PushButtonData(
                name:       "BlocksOpenPanel",
                text:       "Open\nLibrary",
                assemblyName: assemblyPath,
                className:  typeof(Commands.OpenPanelCommand).FullName!)
            {
                ToolTip        = "Open the Blocks Family Library panel.",
                LongDescription = "Browse and place BIM families directly into your Revit project.",
            };

            // Attach icon if the image exists alongside the DLL
            var iconPath = Path.Combine(
                Path.GetDirectoryName(assemblyPath)!, "Resources", "icon_32.png");

            if (File.Exists(iconPath))
                buttonData.LargeImage = new BitmapImage(new Uri(iconPath));

            ribbonPanel.AddItem(buttonData);
        }

        // ── Dockable pane provider ────────────────────────────────────────────

        /// <summary>
        /// Revit calls <see cref="SetupDockablePane"/> when it first needs to
        /// display the panel. At that point we have a live UIApplication and can
        /// finish wiring the <see cref="RevitFamilyLoader"/>.
        /// </summary>
        private sealed class PanelProvider : IDockablePaneProvider
        {
            public void SetupDockablePane(DockablePaneProviderData data)
            {
                // Finish-wire the loader now that UIApplication is available
                // (Revit supplies it indirectly through the Dispatcher context)
                if (_mainViewModel is not null && _libraryService is not null)
                {
                    // We can't get UIApplication here directly; the loader is
                    // re-injected from OpenPanelCommand or via an event the first
                    // time the panel is shown.  For simplicity the panel re-reads
                    // the current application on Execute.
                }

                _mainPanel = new MainPanel
                {
                    DataContext = _mainViewModel
                };

                data.FrameworkElement = _mainPanel;
                data.InitialState = new DockablePaneState
                {
                    DockPosition = DockPosition.Right,
                    MinimumWidth = 380,
                };

                // Trigger initial library load on the UI thread after layout
                _mainPanel.Loaded += async (_, _) =>
                {
                    if (_mainViewModel is not null)
                        await _mainViewModel.InitialLoadAsync();
                };
            }
        }

        // ── Internal accessors used by OpenPanelCommand ───────────────────────

        /// <summary>
        /// Replaces the placeholder NullRevitFamilyLoader with the real one
        /// once a live <see cref="UIApplication"/> is available.
        /// </summary>
        internal static void InjectLoader(IRevitFamilyLoader loader)
        {
            if (_libraryService is null || _favoritesService is null) return;

            _mainViewModel = new MainViewModel(_libraryService, _favoritesService, loader);

            if (_mainPanel is not null)
            {
                _mainPanel.DataContext = _mainViewModel;
                // Re-trigger load so the new VM is populated
                _ = _mainViewModel.InitialLoadAsync();
            }
        }

        // ── Null-object pattern: safe loader before UIApplication exists ─────

        private sealed class NullRevitFamilyLoader : IRevitFamilyLoader
        {
            public System.Threading.Tasks.Task LoadAndPlaceAsync(Models.FamilyItem family)
            {
                TaskDialog.Show("Blocks",
                    "Please close and reopen the panel via the ribbon button to activate placement.");
                return System.Threading.Tasks.Task.CompletedTask;
            }
        }
    }
}
