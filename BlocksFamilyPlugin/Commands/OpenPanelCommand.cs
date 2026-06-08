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
    /// Dual-mode command:
    ///
    ///   • Add-In Manager / no ribbon  → opens the panel as a standalone Window.
    ///   • Ribbon button (App running) → toggles the registered dockable pane.
    ///
    /// This lets you test the full UI via Add-In Manager without any .addin
    /// registration or ribbon setup.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public sealed class OpenPanelCommand : IExternalCommand
    {
        private static bool _loaderInjected;

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            try
            {
                var uiApp = commandData.Application;

                // ── Mode A: App has registered the dockable pane (ribbon flow) ──
                if (_loaderInjected)
                {
                    ToggleDockablePane(uiApp);
                    return Result.Succeeded;
                }

                // ── Mode B: Add-In Manager / first run — open as a Window ────────
                LoadStyles();

                var libraryService   = new FamilyLibraryService();
                var favoritesService = new FavoritesService();
                var loader           = new RevitFamilyLoader(uiApp, libraryService);

                // Inject so ribbon button works correctly if it runs later
                App.InjectLoader(loader);
                _loaderInjected = true;

                var viewModel = new MainViewModel(libraryService, favoritesService, loader);
                var panel     = new MainPanel { DataContext = viewModel };

                var window = new Window
                {
                    Title                 = "Blocks — Family Library",
                    Content               = panel,
                    Width                 = 520,
                    Height                = 700,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    ResizeMode            = ResizeMode.CanResizeWithGrip,
                };

                window.Loaded += async (_, _) => await viewModel.InitialLoadAsync();

                // Show() = modeless — the window stays open alongside Revit
                // without blocking the viewport. This is required so Revit can
                // receive clicks during PromptForFamilyInstancePlacement.
                // ShowDialog() would block all Revit input until the window closes.
                window.Show();

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

        private static void ToggleDockablePane(UIApplication uiApp)
        {
            var pane = uiApp.GetDockablePane(App.PanelId);
            if (pane.IsShown()) pane.Hide();
            else                pane.Show();
        }

        private static bool _stylesLoaded;

        private static void LoadStyles()
        {
            if (_stylesLoaded) return;
            try
            {
                // Styles.xaml is compiled as BAML into the DLL (Page build action).
                // Use a pack URI so it loads from the assembly — no file path needed.
                var uri  = new Uri(
                    "pack://application:,,,/BlocksFamilyPlugin;component/BlocksFamilyPlugin/Resources/Styles.xaml",
                    UriKind.Absolute);
                var dict = new ResourceDictionary { Source = uri };
                Application.Current.Resources.MergedDictionaries.Add(dict);
                _stylesLoaded = true;
            }
            catch { /* styles are cosmetic — swallow */ }
        }
    }
}
