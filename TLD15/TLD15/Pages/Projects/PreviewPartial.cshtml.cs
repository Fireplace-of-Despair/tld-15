using ACherryPie.Feature;
using ACherryPie.Pages;
using Common.Composition;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TLD15.Pages.Projects;

public class PreviewPartialModel : PageModel, IPartial
{
    public static string Id => "preview_projects";
    public string Anchor => Id;
    public string Title => "Projects";

    public List<AFeatureProjects.ResponsePreview> Data { get; set; } = [];

    public static async Task<PreviewPartialModel> InitializeAsync(IMediator mediator)
    {
        var articles = await FeatureRunner.Run(async () =>
        {
            return await mediator.Send(new AFeatureProjects.RequestPreview
            {
                ExcludeDivisions = ["ACD"],
                IncludeDivisions = [.. Globals.Brand.Divisions.Keys.Where(x => x != "ACD")],
            });
        });

        var result = new PreviewPartialModel();

        if (articles.Incident is not null)
        {
            result.ModelState.AddModelError("Projects", articles.Incident.Description);
        }
        else
        {
            result.Data = articles.Data ?? [];
        }

        return result;
    }
}