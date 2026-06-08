using System;
using System.IO;
using System.Reflection;
using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BlocksFamilyPlugin.Services;
using BlocksFamilyPlugin.ViewModels;
using BlocksFamilyPlugin.Views;

namespace BlocksFamilyPlugin.Commands
{
    /// <summary>
    /// Lightweight test entry point for the Revit Add-In Manager.
    /// Opens the Blocks Family Library panel as a standalone WPF Window —
    /// no ribbon setup, no .addin registration required.
    ///
    /// How to use:
    ///   1. Build the project.
    ///   2. In Revit, open Add-In Manager (AddinManager).
    ///   3. Load BlocksFamilyPlugin.dll.
    ///   4. Select "ShowPanelCommand" and click Run.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public sealed class ShowPanelCommand : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            try
            {
                // 1. Load global styles into Application.Current.Resources
                //    so DynamicResource keys resolve in the panel.
                LoadStyles();

                // 2. Compose services
                var libraryService  = new FamilyLibraryService();
                var favoritesService = new FavoritesService();
                var loader          = new RevitFamilyLoader(
                                            commandData.Application,
                                            libraryService);

                // 3. Build ViewModel
                var viewModel = new MainViewModel(libraryService, favoritesService, loader);

                // 4. Build the UserControl and wrap it in a Window
                var panel = new MainPanel { DataContext = viewModel };

                var window = new Window
                {
                    Title           = "Blocks Family Library",
                    Content         = panel,
                    Width           = 520,
                    Height          = 700,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    ResizeMode      = ResizeMode.CanResizeWithGrip,
                    // Dark background matches the panel theme
                    Background      = System.Windows.Media.Brushes.Transparent,
                };

                // 5. Kick off the async library load once the window is ready
                window.Loaded += async (_, _) => await viewModel.InitialLoadAsync();

                // ShowDialog blocks until the window is closed —
                // fine for manual testing; use Show() if you want modeless.
                window.ShowDialog();

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                TaskDialog.Show("Blocks – Error", ex.ToString());
                return Result.Failed;
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static bool _stylesLoaded;

        private static void LoadStyles()
        {
            if (_stylesLoaded) return;

            try
            {
                var dir        = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
                var stylesPath = Path.Combine(dir, "Resources", "Styles.xaml");

                if (!File.Exists(stylesPath)) return;

                var dict = new System.Windows.ResourceDictionary
                {
                    Source = new Uri(stylesPath, UriKind.Absolute)
                };

                Application.Current.Resources.MergedDictionaries.Add(dict);
                _stylesLoaded = true;
            }
            catch
            {
                // Styles are cosmetic — swallow so the panel still opens
            }
        }
    }
}
