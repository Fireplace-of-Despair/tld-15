using ApplePie.Incidents;
using ApplePie.Pages;
using Infrastructure;
using Infrastructure.Models.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using TLD15.Composition;

namespace TLD15.Pages.Projects;

[Authorize]
public sealed class EditModel : PageModel, IPagePrivate
{
    private readonly IConfiguration configuration;
    private readonly DataContextBusiness contextBusiness;

    public EditModel(DataContextBusiness contextBusiness, DataContextReference contextReference, IConfiguration configuration)
    {
        this.configuration = configuration;
        this.contextBusiness = contextBusiness;

        var tmp = contextReference.Divisions
            .Include(x => x.Translations)
            .Select(x => new
            {
                x.Id,
                x.Translations
                .First(t => t.LanguageId == Globals.Settings.Locale).Name
            }).ToList();
        Divisions = new SelectList(tmp, "Id", "Name");
    }

    public static MetaPage Meta => new()
    {
        Id = "projects-edit",
        Title = "Edit Project",
        LocalUrl = "/projects/edit",
    };

    public string Host => configuration.GetSection(Globals.Configuration.ApplicationHost).Value!;


    public sealed class EditData
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string SubTitle { get; set; } = string.Empty;
        public string PosterUrl { get; set; } = string.Empty;
        public string PosterAlt { get; set; } = string.Empty;
        public string DivisionCode { get; set; } = string.Empty;
        public string ContentHtml { get; set; } = string.Empty;
        public string Links { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public long Version { get; set; }
    }

    public SelectList? Divisions { get; set; }

    [BindProperty]
    public EditData Data { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(string? id)
    {
        if (id == null)
        {
            Data.Links = Project.LinksDefault;
            return Page();
        }

        Data = await contextBusiness.Projects
            .AsNoTracking()
            .Include(x => x.Translations)
            .Where(x => x.Id == id)
            .Select(x => new EditData
            {
                Id = x.Id,
                Title = x.Translations.First(t => t.LanguageId == Globals.Settings.Locale).Title,
                SubTitle = x.Translations.First(t => t.LanguageId == Globals.Settings.Locale).Subtitle,
                ContentHtml = x.Translations.First(t => t.LanguageId == Globals.Settings.Locale).ContentHtml,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Version = x.Version,
                DivisionCode = x.DivisionId,
                PosterAlt = x.Translations.First(t => t.LanguageId == Globals.Settings.Locale).PosterAlt,
                PosterUrl = x.PosterUrl,
                Links = x.Links
            })
            .FirstAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("Model", IncidentCode.General.GetDescription());
            return Page();
        }

        var item = await contextBusiness.Projects
            .Include(x => x.Translations)
            .FirstOrDefaultAsync();

        if (item == null)
        {
            item = new Project { Id = Data.Id };
            item.Translations =
            [
                new ProjectTranslation
                {
                    Id = Guid.NewGuid(),
                    ProjectId = item.Id,
                    LanguageId = Globals.Settings.Locale
                },
            ];
            await contextBusiness.AddAsync(item);
        }

        item.DivisionId = Data.DivisionCode;
        item.PosterUrl = Data.PosterUrl;
        item.Links = Data.Links;

        var defaultTranslation = item.Translations.First(x => x.LanguageId == Globals.Settings.Locale);

        defaultTranslation.Title = Data.Title;
        defaultTranslation.Subtitle = Data.SubTitle;
        defaultTranslation.PosterAlt = Data.PosterAlt;
        defaultTranslation.ContentHtml = Data.ContentHtml;

        await contextBusiness.SaveChangesAsync();

        return RedirectToPage(@"/Projects/Edit", new { id = Data!.Id });
    }
}
