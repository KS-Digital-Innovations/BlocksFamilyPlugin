using System.Threading.Tasks;
using BlocksFamilyPlugin.Models;

namespace BlocksFamilyPlugin.Services
{
    /// <summary>
    /// Loads a family .rfa file into the active Revit document and
    /// activates placement mode so the user can click to place an instance.
    /// </summary>
    public interface IRevitFamilyLoader
    {
        Task LoadAndPlaceAsync(FamilyItem family);
    }
}
