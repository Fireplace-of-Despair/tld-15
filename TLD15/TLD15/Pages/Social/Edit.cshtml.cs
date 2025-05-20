using ApplePie.Incidents;
using ApplePie.Pages;
using Infrastructure;
using Infrastructure.Models.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using TLD15.Composition;

namespace TLD15.Pages.Social;

[Authorize]
public class EditModel(DataContextBusiness contextBusiness, IConfiguration configuration) : PageModel, IPagePrivate
{
    public static MetaPage Meta => new()
    {
        Id = "social-edit",
        Title = "Social",
        LocalUrl = "/social/edit",
    };

    public string Host => configuration.GetSection(Globals.Configuration.ApplicationHost).Value!;

    public sealed class EditData
    {
        public string Id { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
        public long Version { get; set; }
    }

    [BindProperty]
    public EditData Data { get; set; } = new EditData();

    public async Task<IActionResult> OnGetAsync()
    {
        var social = await contextBusiness.Contents
            .AsNoTracking()
            .Include(x => x.Translations)
            .Select(x => new
            {
                Id = x.Id,
                x.Translations.First(x => x.LanguageId == Globals.Settings.Locale).Data,
                x.Version,
            })
            .FirstOrDefaultAsync(x => x.Id == Globals.Content.Social.Id);

        if (string.IsNullOrEmpty(social?.Data))
        {
            Data = new EditData
            {
                Id = Globals.Content.Social.Id,
                Version = 0,
                Data = Globals.Content.Social.Serialize(Globals.Content.Social.GetDefault())
            };
        }
        else
        {
            Data = new EditData
            {
                Id = social.Id,
                Version = social.Version,
                Data = social.Data
            };
        }

        return Page();
    }


    public async Task<IActionResult> OnPostAsync()
    {
        var locale = Globals.Settings.Locale;

        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("Model", IncidentCode.General.GetDescription());
            return Page();
        }

        var item = await contextBusiness.Contents
            .Include(x => x.Translations)
            .FirstAsync(x => x.Id == Globals.Content.Social.Id);

        var translation = item.Translations.FirstOrDefault(x => x.LanguageId == locale);
        if (translation == null)
        {
            translation = new ContentTranslation
            {
                Id = Guid.NewGuid(),
                ContentId = item.Id,
                LanguageId = locale,
            };
            await contextBusiness.AddAsync(translation);
        }
        translation.Data = Data.Data;

        await contextBusiness.SaveChangesAsync();

        return Page();
    }
}
