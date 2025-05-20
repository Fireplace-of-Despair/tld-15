using ApplePie.Pages;
using Infrastructure;
using Infrastructure.Models.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TLD15.Composition;

namespace TLD15.Pages.Projects;

public class ReadModel(
    DataContextBusiness contextBusiness,
    IConfiguration configuration) : PageModel, IPagePublic
{
    public static MetaPage Meta => new()
    {
        Id = "project",
        Title = "Projects",
        LocalUrl = "/Projects/Read",
    };

    public string Host => configuration.GetSection(Globals.Configuration.ApplicationHost).Value!;

    public required LocalProject Data { get; set; } = new();

    public sealed class LocalProject
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string PosterUrl { get; set; } = string.Empty;
        public string PosterAlt { get; set; } = string.Empty;
        public string DivisionCode { get; set; } = string.Empty;
        public string ContentHtml { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public Dictionary<string, string> Links { get; set; } = [];
        public long Version { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var locale = Globals.Settings.Locale;

        var item = await contextBusiness.Projects
            .Include(x => x.Translations)
            .Where(x => x.Id == id)
            .Select(x => new
            {
                Id = x.Id,
                Title = x.Translations
                    .Where(t => t.LanguageId == locale)
                    .Select(t => t.Title)
                    .First(),
                Subtitle = x.Translations
                    .Where(t => t.LanguageId == locale)
                    .Select(t => t.Subtitle)
                    .First(),
                PosterUrl = x.PosterUrl,
                PosterAlt = x.Translations
                    .Where(t => t.LanguageId == locale)
                    .Select(t => t.PosterAlt)
                    .First(),
                DivisionCode = x.DivisionId,
                ContentHtml = x.Translations
                    .Where(t => t.LanguageId == locale)
                    .Select(t => t.ContentHtml)
                    .First(),
                CreatedAt = x.CreatedAt,
                Links = x.Links,
                Version = x.Version
            })
            .FirstAsync();

        Data = new LocalProject
        {
            Id = item.Id,
            Title = item.Title,
            Subtitle = item.Subtitle,
            PosterUrl = item.PosterUrl,
            PosterAlt = item.PosterAlt,
            DivisionCode = item.DivisionCode,
            ContentHtml = item.ContentHtml,
            CreatedAt = item.CreatedAt,
            Links = Project.LinksToDictionary(item.Links),
            Version = item.Version
        };

        return Page();
    }
}
