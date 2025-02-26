using ACherryPie.Feature;
using ACherryPie.Pages;
using Common.Composition;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TLD15.Pages.Projects;

public class ArchiveModel(
      IMediator mediator
    , IConfiguration configuration) : PageModel, IPagePublic
{
    public static MetaPagePublic MetaPublic => new()
    {
        Id = "archive",
        Title = "Archive",
        LocalUrl = "/Projects/Archive",
    };

    public string Host => configuration.GetSection(Globals.Configuration.ApplicationHost).Value!;

    public List<AFeatureProjects.ResponsePreview> Data { get; set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        var articles = await FeatureRunner.Run(async () =>
        {
            return await mediator.Send(new AFeatureProjects.RequestPreview
            {
                ExcludeDivisions = [],
                IncludeDivisions = [.. Globals.Brand.Divisions.Keys.Where(x => x == "ACD")],
            });
        });

        var result = new PreviewPartialModel();

        if (articles.Incident is not null)
        {
            result.ModelState.AddModelError("Projects", articles.Incident.Description);
        }
        else
        {
            Data = articles.Data ?? [];
        }

        return Page();
    }

}
