using ApplePie.Feature;
using ApplePie.Incidents;
using ApplePie.Pages;
using Infrastructure;
using Infrastructure.Models.Business;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TLD15.Composition;

namespace TLD15.Pages.Projects;

public sealed class PreviewModel : PageModel, IPartial
{
    public static MetaPartial Meta => new()
    {
        Id = "preview_projects",
        Title = "Projects",
        Path = "/Pages/Projects/Preview.cshtml",
    };

    public sealed class PreviewData
    {
        public required string? Id { get; set; }
        public string DivisionId { get; set; } = string.Empty;
        public string DivisionName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string PosterUrl { get; set; } = string.Empty;
        public string PosterAlt { get; set; } = string.Empty;
        public required Dictionary<string, string> Links { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }

    public List<PreviewData> Data { get; set; } = [];

    public static async Task<PreviewModel> InitializeAsync(DataContextBusiness contextBusiness)
    {
        var projects = await FeatureRunner.Run(async () =>
        {
            var locale = Globals.Settings.Locale;

            return await contextBusiness
                .Projects
                .AsNoTracking()
                .Include(x => x.Translations)
                .Include(x => x.Division).ThenInclude(x => x.Translations)
                .Where(x => x.DivisionId != Globals.Brand.Divisions.ACD)
                .Select(x => new
                {
                    x.Id,
                    x.DivisionId,
                    DivisionName = x.Division.Translations.First(x => x.LanguageId == locale).Name,
                    x.Translations.First(x => x.LanguageId == locale).Title,
                    x.Translations.First(x => x.LanguageId == locale).Subtitle,
                    x.PosterUrl,
                    x.Translations.First(x => x.LanguageId == locale).PosterAlt,
                    x.CreatedAt,
                    x.Links,
                })
                .ToListAsync();
        });


        var result = new PreviewModel();
        if (projects.Incident != null)
        {
            result.ModelState.AddModelError(Meta.Id, projects.Incident.Value.GetDescription());
        }
        if (projects.Data != null)
        {
            result.Data = projects.Data
                .ConvertAll(x => new PreviewData
                {
                    Id = x.Id,
                    DivisionId = x.DivisionId,
                    DivisionName = x.DivisionName,
                    Title = x.Title,
                    Subtitle = x.Subtitle,
                    PosterUrl = x.PosterUrl,
                    PosterAlt = x.PosterAlt,
                    CreatedAt = x.CreatedAt,
                    Links = Project.LinksToDictionary(x.Links),
                });
        }

        return result;
    }
}
