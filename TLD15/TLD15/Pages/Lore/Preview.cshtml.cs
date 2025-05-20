using ApplePie.Feature;
using ApplePie.Incidents;
using ApplePie.Pages;
using Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TLD15.Composition;

namespace TLD15.Pages.Lore;

public class PreviewModel : PageModel, IPartial
{
    public static MetaPartial Meta => new()
    {
        Id = "preview_lore",
        Title = Globals.Content.Lore.Title,
        Path = "/Pages/Lore/Preview.cshtml",
    };

    public string Data { get; set; } = string.Empty;

    public static async Task<PreviewModel> InitializeAsync(DataContextBusiness contextBusiness)
    {
        var lore = await FeatureRunner.Run(async () =>
        {
            var locale = Globals.Settings.Locale;

            var data = await contextBusiness
                .Contents
                .AsNoTracking()
                .Where(contextBusiness => contextBusiness.Id == Globals.Content.Lore.Id)
                .Select(x => new
                {
                    x.Id,
                    x.Translations.First(y => y.LanguageId == Globals.Settings.Locale).Data,
                })
                .FirstAsync();

            return data.Data;
        });


        var result = new PreviewModel();
        if (lore.Incident != null)
        {
            result.ModelState.AddModelError(Meta.Id, lore.Incident.Value.GetDescription());
        }
        if (lore.Data != null)
        {
            result.Data = lore.Data;
        }

        return result;
    }
}
