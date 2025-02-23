using Common.Composition;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TLD15.Pages.Press;

public class ReadModel(
    IMediator mediator,
    IConfiguration configuration) : PageModel
{
    public static string TitleStatic => "Press";
    public string Title => TitleStatic;

    public readonly string ApplicationHost = configuration.GetSection(Globals.Settings.ApplicationHost).Value!;

    public required List<AFeaturePress.ResponsePreview> Data { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        Data = await mediator.Send(new AFeaturePress.RequestPreview());

        return Page();
    }
}
