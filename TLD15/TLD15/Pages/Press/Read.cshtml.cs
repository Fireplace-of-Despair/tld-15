using ApplePie.Feature;
using ApplePie.Incidents;
using ApplePie.Pages;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TLD15.Composition;

namespace TLD15.Pages.Press;

public class ReadModel(DataContextBusiness contextBusiness,
    IConfiguration configuration) : PageModel, IPagePublic
{
    public static MetaPage Meta => new()
    {
        Id = "press",
        Title = "Press",
        LocalUrl = "/Press/Read",
    };

    public string Host => configuration.GetSection(Globals.Configuration.ApplicationHost).Value!;

    public sealed record LocalPress
    {
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;

        public string PosterUrl { get; set; } = string.Empty;
        public string PosterAlt { get; set; } = string.Empty;
        public DateTimeOffset PublishedAt { get; set; }
    }

    public List<LocalPress> Data { get; set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        var pressData = await FeatureRunner.Run(async () =>
        {
            var locale = Globals.Settings.Locale;
            var data = await contextBusiness
                .Contents
                .AsNoTracking()
                .Where(x => x.Id == Globals.Content.Press.Id)
                .Select(x => new
                {
                    x.Id,
                    x.Translations.First(y => y.LanguageId == Globals.Settings.Locale).Data,
                })
                .FirstAsync();

            return Globals.Content.Press.Deserialize(data.Data);
        });

        if (pressData.Incident != null)
        {
            ModelState.AddModelError(Meta.Id, pressData.Incident.Value.GetDescription());
        }
        if (pressData.Data != null)
        {
            Data = pressData.Data.ConvertAll(x => new LocalPress
            {
                Title = x.Title,
                Subtitle = x.Subtitle,
                PosterUrl = x.PosterUrl,
                PosterAlt = x.PosterAlt,
                PublishedAt = x.PublishedAt,
            });
        }

        return Page();
    }
}
