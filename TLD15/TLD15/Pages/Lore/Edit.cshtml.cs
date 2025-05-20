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

namespace TLD15.Pages.Lore;

[Authorize]
public class EditModel(DataContextBusiness contextBusiness, IConfiguration configuration) : PageModel, IPagePrivate
{
    public static MetaPage Meta => new()
    {
        Id = "lore-edit",
        Title = "Lore",
        LocalUrl = "/lore/edit",
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
        var lore = await contextBusiness.Contents
            .AsNoTracking()
            .Include(x => x.Translations)
            .Select(x => new
            {
                Id = x.Id,
                x.Translations.First(x => x.LanguageId == Globals.Settings.Locale).Data,
                x.Version,
            })
            .FirstOrDefaultAsync(x => x.Id == Globals.Content.Lore.Id);

        if (string.IsNullOrEmpty(lore?.Data))
        {
            Data = new EditData
            {
                Id = Globals.Content.Lore.Id,
                Version = 0,
                Data = string.Empty
            };
        }
        else
        {
            Data = new EditData
            {
                Id = lore.Id,
                Version = lore.Version,
                Data = lore.Data
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
            .FirstAsync(x => x.Id == Globals.Content.Lore.Id);

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
