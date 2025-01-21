using ACherryPie.Pages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TLD15.Pages.Articles;

[Authorize]
public class ManageArticlesModel(IMediator mediator) : PageModel, IPageAdmin
{
    public static MetaData MetaData => new()
    {
        Id = "ManageArticles",
        LocalUrl = "/Articles/Manage"
    };

    public IMediator Mediator => mediator;

    public List<AFeatureArticle.ResponsePreview> Data { get; set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        var result = await mediator.Send(new AFeatureArticle.RequestPreview());

        result.Add(new AFeatureArticle.ResponsePreview
        {
            Id = null,
            Title = "NEW",
            Division = "FOD",
            CreatedAt = DateTime.UtcNow,
        });

        Data = result;
        return Page();
    }

    public async Task<IActionResult> OnPostDelete(Guid id)
    {
        await mediator.Send(new AFeatureArticle.RequestDelete { Id = id });

        return RedirectToPage();
    }
}
