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

namespace TLD15.Pages.Archive;

public class ReadModel(DataContextBusiness contextBusiness, IConfiguration configuration) : PageModel, IPagePublic
{
    public static MetaPage Meta => new()
    {
        Id = "archive",
        Title = "Archive",
        LocalUrl = "/Archive/Read",
    };

    public string Host => configuration.GetSection(Globals.Configuration.ApplicationHost).Value!;

    public sealed class ArchivedProject
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string PosterUrl { get; set; } = string.Empty;
        public string PosterAlt { get; set; } = string.Empty;
        public string DivisionId { get; set; } = string.Empty;
        public string DivisionName { get; set; } = string.Empty;
        public string ContentHtml { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public Dictionary<string, string> Links { get; set; } = [];
        public long Version { get; set; }
    }
    public sealed record ArchivedArticle
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

    public required List<ArchivedProject> Projects { get; set; } = [];
    public required List<ArchivedArticle> Articles { get; set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        var archive = await FeatureRunner.Run(async () =>
        {
            var locale = Globals.Settings.Locale;

            var articles = await contextBusiness
                .Articles
                .AsNoTracking()
                .Include(x => x.Translations)
                .Include(x => x.Division).ThenInclude(x => x.Translations)
                .Where(x => x.DivisionId == Globals.Brand.Divisions.ACD)
                .Select(x => new ArchivedArticle
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

            var projects = await contextBusiness
                .Projects
                .AsNoTracking()
                .Include(x => x.Translations)
                .Include(x => x.Division).ThenInclude(x => x.Translations)
                .Where(x => x.DivisionId == Globals.Brand.Divisions.ACD)
                .Select(x => new ArchivedProject
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
            return new
            {
                Articles = articles,
                Projects = projects,
            };
        });


        if (archive.Incident != null)
        {
            ModelState.AddModelError(Meta.Id, archive.Incident.Value.GetDescription());
        }

        Articles = archive.Data?.Articles ?? [];
        Projects = archive.Data?.Projects ?? [];
        return Page();
    }
}
