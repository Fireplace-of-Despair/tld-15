using ApplePie.Feature;
using ApplePie.Incidents;
using ApplePie.Pages;
using Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TLD15.Composition;

namespace TLD15.Pages.Articles;

public class PreviewModel : PageModel, IPartial
{
    public static MetaPartial Meta => new()
    {
        Id = "preview_articles",
        Title = "Articles",
        Path = "/Pages/Articles/Preview.cshtml",
    };

    public sealed record PreviewData
    {
        public required string? Id { get; set; }
        public string DivisionId { get; set; } = string.Empty;
        public string DivisionName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string PosterUrl { get; set; } = string.Empty;
        public string PosterAlt { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
    }

    public List<PreviewData> Data { get; set; } = [];

    public static async Task<PreviewModel> InitializeAsync(DataContextBusiness contextBusiness)
    {
        var articles = await FeatureRunner.Run(async () =>
        {
            var locale = Globals.Settings.Locale;

            return await contextBusiness
                .Articles
                .AsNoTracking()
                .Include(x => x.Translations)
                .Include(x => x.Division).ThenInclude(x => x.Translations)
                .Where(x => x.DivisionId != Globals.Brand.Divisions.ACD)
                .Select(x => new PreviewData
                {
                    Id = x.Id,
                    DivisionId = x.DivisionId,
                    DivisionName = x.Division.Translations.First(x => x.LanguageId == locale).Name,
                    Title = x.Translations.First(x => x.LanguageId == locale).Title,
                    Subtitle = x.Translations.First(x => x.LanguageId == locale).Subtitle,
                    PosterUrl = x.PosterUrl,
                    PosterAlt = x.Translations.First(x => x.LanguageId == locale).PosterAlt,
                    CreatedAt = x.CreatedAt,
                })
                .ToListAsync();
        });


        var result = new PreviewModel();
        if (articles.Incident != null)
        {
            result.ModelState.AddModelError(Meta.Id, articles.Incident.Value.GetDescription());
        }
        if (articles.Data != null)
        {
            result.Data = articles.Data ?? [];
        }

        return result;
    }
}
