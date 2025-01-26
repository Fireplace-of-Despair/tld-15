using Common.Composition;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace TLD15.Pages.Projects;

public class ReadModel(
    IMediator mediator,
    IConfiguration configuration) : PageModel
{
    public readonly string ApplicationHost = configuration.GetSection(Globals.Settings.ApplicationHost).Value!;

    public required AFeatureProjects.ResponseRead Model { get; set; }

    public async Task<IActionResult> OnGetAsync(string idFriendly)
    {
        Model = await mediator.Send(new AFeatureProjects.RequestRead { IdFriendly = idFriendly, Id = null });

        return Page();
    }
}
