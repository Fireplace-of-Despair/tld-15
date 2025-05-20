using ApplePie.Pages;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using TLD15.Composition;

namespace TLD15.Pages.Articles;

public sealed class ReadModel(
    DataContextBusiness contextBusiness,
    IConfiguration configuration) : PageModel, IPagePublic
{
    public static MetaPage Meta => new()
    {
        Id = "article",
        Title = "Article",
        LocalUrl = "/Articles/Read",
    };

    public string Host => configuration.GetSection(Globals.Configuration.ApplicationHost).Value!;

    public sealed class ArticleData
    {
        public string? Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string PosterUrl { get; set; } = string.Empty;
        public string PosterAlt { get; set; } = string.Empty;
        public string DivisionCode { get; set; } = string.Empty;
        public string DivisionName { get; set; } = string.Empty;
        public string ContentHtml { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public long Version { get; set; }
    }

    public required ArticleData Data { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var result = await contextBusiness
            .Articles
            .AsNoTracking()
            .Include(x => x.Translations)
            .Include(x => x.Division).ThenInclude(x => x.Translations)
            .Select(x => new ArticleData
            {
                Id = x.Id,
                Title = x.Translations.First(t => t.LanguageId == Globals.Settings.Locale).Title,
                Subtitle = x.Translations.First(t => t.LanguageId == Globals.Settings.Locale).Subtitle,
                PosterUrl = x.PosterUrl,
                PosterAlt = x.Translations.First(t => t.LanguageId == Globals.Settings.Locale).PosterAlt,
                DivisionCode = x.DivisionId,
                DivisionName = x.Division.Translations.First(t => t.LanguageId == Globals.Settings.Locale).Name,
                ContentHtml = x.Translations.First(t => t.LanguageId == Globals.Settings.Locale).ContentHtml,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Version = x.Version
            })
            .FirstAsync(x => x.Id == id);

        Data = result;
        return Page();
    }
}
