using System;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BlocksFamilyPlugin.Models;

namespace BlocksFamilyPlugin.Services
{
    /// <summary>
    /// Loads a .rfa family into the active Revit document and activates
    /// placement mode so the user can click to place an instance.
    ///
    /// Threading rules:
    ///   • ExternalEvent.Create() MUST be called inside a valid Revit API
    ///     context (IExternalCommand.Execute / IExternalApplication.OnStartup).
    ///     We do this once in the constructor.
    ///   • ExternalEvent.Raise() can be called from any thread (WPF, async, etc.)
    ///     and Revit will dispatch Execute() on its main thread when ready.
    /// </summary>
    public sealed class RevitFamilyLoader : IRevitFamilyLoader
    {
        private readonly IFamilyLibraryService  _libraryService;
        private readonly LoadFamilyEventHandler _handler;
        private readonly ExternalEvent          _externalEvent;

        /// <param name="uiApp">
        ///   Must be passed from inside IExternalCommand.Execute so that
        ///   ExternalEvent.Create is called in a valid API context.
        /// </param>
        public RevitFamilyLoader(UIApplication uiApp, IFamilyLibraryService libraryService)
        {
            _libraryService = libraryService;
            _handler        = new LoadFamilyEventHandler(uiApp);

            // ← This line must run inside a valid Revit API context.
            //   OpenPanelCommand.Execute() satisfies that requirement.
            _externalEvent  = ExternalEvent.Create(_handler);
        }

        // ─────────────────────────────────────────────────────────────────────

        public async Task LoadAndPlaceAsync(FamilyItem family)
        {
            // 1. Ensure the .rfa is available locally (download if needed)
            var rfaPath = await _libraryService.EnsureLocalRfaAsync(family);

            // 2. Prime the handler with this specific family, reset its TCS
            _handler.Prepare(rfaPath, family.Name);

            // 3. Signal Revit to call Execute() on its main thread
            //    This is safe to call from the WPF / async thread.
            var result = _externalEvent.Raise();
            if (result == ExternalEventRequest.Denied)
                throw new InvalidOperationException(
                    "Revit denied the ExternalEvent request. " +
                    "Make sure a document is open and Revit is not in a command.");

            // 4. Await until Execute() finishes
            await _handler.CompletionTask;

            if (_handler.Error is not null)
                throw new InvalidOperationException(_handler.Error);
        }

        // ── Inner handler — executed on Revit's main thread ──────────────────

        private sealed class LoadFamilyEventHandler : IExternalEventHandler
        {
            private readonly UIApplication _uiApp;

            // These are set by Prepare() before each Raise()
            private string _rfaPath    = string.Empty;
            private string _familyName = string.Empty;

            // Reset per-call so the handler is reusable
            private TaskCompletionSource<bool> _tcs = new();

            public Task    CompletionTask => _tcs.Task;
            public string? Error          { get; private set; }

            public LoadFamilyEventHandler(UIApplication uiApp)
                => _uiApp = uiApp;

            /// <summary>
            /// Call this before every Raise() to set the family to load
            /// and reset the completion signal.
            /// </summary>
            public void Prepare(string rfaPath, string familyName)
            {
                _rfaPath    = rfaPath;
                _familyName = familyName;
                Error       = null;
                _tcs        = new TaskCompletionSource<bool>();
            }

            // Called by Revit on its main thread
            public void Execute(UIApplication app)
            {
                try
                {
                    var doc   = app.ActiveUIDocument.Document;
                    var uidoc = app.ActiveUIDocument;

                    // ── Load the family file into the document ────────────────
                    Family? loadedFamily = null;
                    using (var tx = new Transaction(doc, $"Load: {_familyName}"))
                    {
                        tx.Start();

                        if (!doc.LoadFamily(_rfaPath, out loadedFamily))
                        {
                            // Already in the document — find it by name
                            loadedFamily = new FilteredElementCollector(doc)
                                .OfClass(typeof(Family))
                                .Cast<Family>()
                                .FirstOrDefault(f => f.Name == _familyName);
                        }

                        tx.Commit();
                    }

                    if (loadedFamily is null)
                    {
                        Error = "Family could not be loaded or found in the document.";
                        _tcs.SetResult(false);
                        return;
                    }

                    // ── Activate the first symbol ─────────────────────────────
                    var symbolId = loadedFamily.GetFamilySymbolIds().FirstOrDefault();
                    if (symbolId == ElementId.InvalidElementId)
                    {
                        Error = "No family types (symbols) found in this family.";
                        _tcs.SetResult(false);
                        return;
                    }

                    var symbol = (FamilySymbol)doc.GetElement(symbolId);
                    using (var tx = new Transaction(doc, "Activate Symbol"))
                    {
                        tx.Start();
                        if (!symbol.IsActive) symbol.Activate();
                        tx.Commit();
                    }

                    // ── Hand off to the user to click a placement point ───────
                    // PromptForFamilyInstancePlacement blocks until the user
                    // presses Esc or places an instance.
                    uidoc.PromptForFamilyInstancePlacement(symbol);

                    _tcs.SetResult(true);
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                {
                    // User pressed Esc — not an error
                    _tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    Error = ex.Message;
                    _tcs.SetResult(false);
                }
            }

            public string GetName() => "BlocksFamilyPlugin_LoadFamily";
        }
    }
}
