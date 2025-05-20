using ApplePie.Feature;
using ApplePie.Incidents;
using ApplePie.Pages;
using Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TLD15.Composition;

namespace TLD15.Pages.Social;

public class PreviewModel : PageModel, IPartial
{
    public static MetaPartial Meta => new()
    {
        Id = "preview_social",
        Title = Globals.Content.Social.Title,
        Path = "/Pages/Social/Preview.cshtml",
    };

    public Dictionary<string, string> Data { get; set; } = [];

    public static async Task<PreviewModel> InitializeAsync(DataContextBusiness contextBusiness)
    {
        var social = await FeatureRunner.Run(async () =>
        {
            var locale = Globals.Settings.Locale;

            var item = await contextBusiness
                .Contents
                .AsNoTracking()
                .Where(contextBusiness => contextBusiness.Id == Globals.Content.Social.Id)
                .Select(x => new
                {
                    x.Id,
                    x.Translations.First(y => y.LanguageId == Globals.Settings.Locale).Data,
                })
                .FirstAsync();

            return Globals.Content.Social.Deserialize(item.Data) ?? [];
        });


        var result = new PreviewModel();
        if (social.Incident != null)
        {
            result.ModelState.AddModelError(Meta.Id, social.Incident.Value.GetDescription());
        }
        if (social.Data != null)
        {
            result.Data = social.Data;
        }

        return result;
    }
}
