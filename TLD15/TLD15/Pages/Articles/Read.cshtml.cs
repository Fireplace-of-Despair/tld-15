using ACherryPie.Pages;
using Common.Composition;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace TLD15.Pages.Articles;

public sealed class ReadModel(
    IMediator mediator,
    IConfiguration configuration) : PageModel, IPagePublic
{
    public static MetaPagePublic MetaPublic => new()
    {
        Id = "article",
        Title = "Article",
        LocalUrl = "/Articles/Read",
    };

    public string Host => configuration.GetSection(Globals.Configuration.ApplicationHost).Value!;

    [BindProperty]
    public required AFeatureArticle.ResponseRead Model { get; set; }

    public async Task<IActionResult> OnGetAsync(string idFriendly)
    {
        Model = await mediator.Send(new AFeatureArticle.RequestRead { IdFriendly = idFriendly, Id = null });
        
        return Page();
    }
}
