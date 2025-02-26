using ACherryPie.Feature;
using ACherryPie.Pages;
using Common.Composition;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace TLD15.Pages.Projects;

public class ReadModel(
    IMediator mediator,
    IConfiguration configuration) : PageModel, IPagePublic
{
    public static MetaPagePublic MetaPublic => new()
    {
        Id = "project",
        Title = "Projects",
        LocalUrl = "/Projects/Read",
    };

    public string Host => configuration.GetSection(Globals.Configuration.ApplicationHost).Value!;

    public required AFeatureProjects.ResponseRead Model { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(string idFriendly)
    {
        var result = await FeatureRunner.Run(async () =>
            await mediator.Send(new AFeatureProjects.RequestRead { IdFriendly = idFriendly, Id = null })
        );

        if (result.Incident != null)
        {
            ModelState.AddModelError(string.Empty, result.Incident.Description);
            return Page();
        }

        Model = result.Data!;
        return Page();
    }
}
