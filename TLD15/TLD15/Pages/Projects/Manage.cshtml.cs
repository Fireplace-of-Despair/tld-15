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

namespace TLD15.Pages.Projects;

[Authorize]
public class ManageModel : PageModel, IPageAdmin
{
    public static MetaPage Meta => new()
    {
        Id = "manage_projects",
        LocalUrl = "/projects/manage",
        Title = "Projects",
    };

    public string Host => configuration.GetSection(Globals.Configuration.ApplicationHost).Value!;

    private readonly IConfiguration configuration;
    private readonly DataContextBusiness contextBusiness;

    public ManageModel(IConfiguration configuration, DataContextBusiness contextBusiness)
    {
        this.configuration = configuration;
        this.contextBusiness = contextBusiness;
    }

    public List<Preview> Data { get; set; } = [];

    public sealed class Preview
    {
        public required string? Id { get; set; }
        public string Division { get; set; } = string.Empty;
        public required string DivisionName { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;

        public string PosterUrl { get; set; } = string.Empty;
        public string PosterAlt { get; set; } = string.Empty;

        public DateTimeOffset CreatedAt { get; set; }
    }

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
                Division = x.DivisionId,
                DivisionName = x.Division!.Translations.First(tr => tr.LanguageId == locale).Name,
                CreatedAt = x.CreatedAt,
            })
            .ToListAsync();

        result.Add(new Preview
        {
            Id = null,
            Title = "NEW",
            Division = "FOD",
            DivisionName = "FOD",
            CreatedAt = DateTime.UtcNow,
        });
        this.Data = result;

        return Page();
    }
}
