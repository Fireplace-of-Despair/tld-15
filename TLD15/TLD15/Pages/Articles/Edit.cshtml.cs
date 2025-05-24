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

namespace TLD15.Pages.Articles;

[Authorize]
public class EditModel : PageModel, IPagePrivate
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
        Id = "articles-edit",
        Title = "Edit Article",
        LocalUrl = "/articles/edit",
    };

    public string Host => configuration.GetSection(Globals.Configuration.ApplicationHost).Value!;

    public sealed class ResponseRead
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string SubTitle { get; set; } = string.Empty;
        public string PosterUrl { get; set; } = string.Empty;
        public string PosterAlt { get; set; } = string.Empty;
        public string DivisionCode { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ContentHtml { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public long Version { get; set; }
    }

    [BindProperty]
    public ResponseRead Model { get; set; } = new();

    public SelectList? Divisions { get; set; }

    public async Task<IActionResult> OnGetAsync(string? id)
    {
        if (id == null)
        {
            return Page();
        }

        var result = await contextBusiness
            .Articles
            .Include(x => x.Translations)
            .Where(x => x.Id == id)
            .Select(x => new ResponseRead
            {
                Id = x.Id,
                Title = x.Translations
                    .First(t => t.LanguageId == Globals.Settings.Locale).Title,
                SubTitle = x.Translations
                    .First(t => t.LanguageId == Globals.Settings.Locale).Subtitle,
                PosterUrl = x.PosterUrl,
                PosterAlt = x.Translations
                    .First(t => t.LanguageId == Globals.Settings.Locale).PosterAlt,
                DivisionCode = x.DivisionId,
                ContentHtml = x.Translations
                    .First(t => t.LanguageId == Globals.Settings.Locale).ContentHtml,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Version = x.Version
            })
            .FirstAsync();

        Model = result;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("Model", IncidentCode.General.GetDescription());
            return Page();
        }

        var item = await contextBusiness.Articles
            .Include(x => x.Translations)
            .FirstOrDefaultAsync(x => x.Id == Model.Id);

        if (item == null)
        {
            item = new Article { Id = Model.Id.ToLower() };
            item.Translations =
            [
                new ArticleTranslation
                {
                    Id = Guid.NewGuid(),
                    ArticleId = item.Id,
                    LanguageId = Globals.Settings.Locale
                },
            ];
            await contextBusiness.AddAsync(item);
        }

        item.DivisionId = Model.DivisionCode;
        item.PosterUrl = Model.PosterUrl;

        var defaultTranslation = item.Translations.First(x => x.LanguageId == Globals.Settings.Locale);

        defaultTranslation.Title = Model.Title;
        defaultTranslation.Subtitle = Model.SubTitle;
        defaultTranslation.PosterAlt = Model.PosterAlt;
        defaultTranslation.ContentHtml = Model.ContentHtml;

        await contextBusiness.SaveChangesAsync();

        return RedirectToPage(@"/Articles/Edit", new { id = item.Id });
    }
}
