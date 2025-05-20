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

namespace TLD15.Pages.Contacts;

public class PreviewModel : PageModel, IPartial
{
    public static MetaPartial Meta => new()
    {
        Id = "preview_contacts",
        Title = Globals.Content.Contacts.Title,
        Path = "/Pages/Contacts/Preview.cshtml",
    };

    public Dictionary<string, string> Data { get; set; } = [];

    public static async Task<PreviewModel> InitializeAsync(DataContextBusiness contextBusiness)
    {
        var contacts = await FeatureRunner.Run(async () =>
        {
            var locale = Globals.Settings.Locale;
            var item = await contextBusiness
                .Contents
                .AsNoTracking()
                .Where(x => x.Id == Globals.Content.Contacts.Id)
                .Select(x => new
                {
                    x.Translations.First(y => y.LanguageId == locale).Data,
                })
                .FirstAsync();

            return Globals.Content.Contacts.Deserialize(item.Data) ?? [];
        });


        var result = new PreviewModel();
        if (contacts.Incident != null)
        {
            result.ModelState.AddModelError(Meta.Id, contacts.Incident.Value.GetDescription());
        }
        if (contacts.Data != null)
        {
            result.Data = contacts.Data;
        }

        return result;
    }
}
