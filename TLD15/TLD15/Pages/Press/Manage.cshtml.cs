using ACherryPie.Pages;
using Common.Composition;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TLD15.Pages.Press;

[Authorize]
public class ManageModel(
    IMediator mediator,
    IConfiguration configuration) : PageModel, IPageAdmin
{
    public IMediator Mediator => mediator;

    public readonly string ApplicationHost = configuration.GetSection(Globals.Configuration.ApplicationHost).Value!;
    public static MetaData MetaData => new()
    {
        Id = "Press",
        LocalUrl = "/press/manage"
    };

    public List<AFeaturePress.ResponsePreview> Data { get; set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        var result = await mediator.Send(new AFeaturePress.RequestPreview());

        result.Add(new AFeaturePress.ResponsePreview
        {
            PosterAlt = "NEW",
            PosterUrl = string.Empty,
            Subtitle = "NEW",
             
            Id = null,
            Title = "NEW",
            CreatedAt = DateTime.UtcNow,
        });
        Data = [.. result.OrderBy(x => x.CreatedAt)];
        return Page();
    }

    public async Task<IActionResult> OnPostDelete(Guid id)
    {
        await mediator.Send(new AFeaturePress.RequestDelete { Id = id });
        return RedirectToPage();
    }
}
