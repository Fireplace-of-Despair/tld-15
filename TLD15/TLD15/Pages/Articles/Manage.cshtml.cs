using ApplePie.Pages;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TLD15.Composition;

namespace TLD15.Pages.Articles;

[Authorize]
public class ManageModel(
    DataContextBusiness contextBusiness
    ,IConfiguration configuration) : PageModel, IPagePrivate
{
    public static MetaPage Meta => new()
    {
        Id = "manage_articles",
        LocalUrl = "/articles/manage",
        Title = "Articles",
    };

    public string Host => configuration.GetSection(Globals.Configuration.ApplicationHost).Value!;


    public sealed class Preview
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

    public List<Preview> Data { get; set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        var locale = Globals.Settings.Locale;
        var result = await contextBusiness.Projects
            .Include(x => x.Translations)
            .Include(x => x.Division).ThenInclude(x => x!.Translations)
            .OrderBy(x => x.CreatedAt)
            .Select(x => new Preview
            {
                Id = x.Id,
                Title = x.Translations.First(tr => tr.LanguageId == locale).Title,
                Subtitle = x.Translations.First(tr => tr.LanguageId == locale).Subtitle,
                PosterUrl = x.PosterUrl,
                PosterAlt = x.Translations.First(tr => tr.LanguageId == locale).PosterAlt,
                DivisionId = x.DivisionId,
                DivisionName = x.Division!.Translations.First(tr => tr.LanguageId == locale).Name,
                CreatedAt = x.CreatedAt,
            })
            .ToListAsync();

        result.Add(new Preview
        {
            Id = null,
            Title = "NEW",
            DivisionId = "FOD",
            DivisionName = "FOD",
            CreatedAt = DateTime.UtcNow,
        });

        Data = result;
        return Page();
    }

    //public async Task<IActionResult> OnPostDelete(string id)
    //{
    //    await mediator.Send(new FeatureArticle.RequestDelete { Id = id });

    //    return RedirectToPage();
    //}
}
