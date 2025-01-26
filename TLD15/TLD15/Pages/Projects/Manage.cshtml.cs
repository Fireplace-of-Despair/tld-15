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

namespace TLD15.Pages.Projects;

[Authorize]
public class ManageModel(
    IMediator mediator,
    IConfiguration configuration) : PageModel, IPageAdmin
{
    public IMediator Mediator => mediator;

    public readonly string ApplicationHost = configuration.GetSection(Globals.Settings.ApplicationHost).Value!;
    public static MetaData MetaData => new()
    {
        Id = "Projects",
        LocalUrl = "/projects/manage"
    };
    
    public List<AFeatureProjects.ResponsePreview> Data { get; set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        var result = await mediator.Send(new AFeatureProjects.RequestPreview());
        result.Add(new AFeatureProjects.ResponsePreview
        {
            Id = null,
            Title = "NEW",
            Division = "FOD",
            Links = [],
            CreatedAt = DateTime.UtcNow,
        });
        Data = [.. result.OrderBy(x => x.CreatedAt)];
        return Page();
    }

    public async Task<IActionResult> OnPostDelete(Guid id)
    {
        await mediator.Send(new AFeatureProjects.RequestDelete { Id = id });
        return RedirectToPage();
    }
}