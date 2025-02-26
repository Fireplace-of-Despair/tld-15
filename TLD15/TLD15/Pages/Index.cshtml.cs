using ACherryPie.Pages;
using Common.Composition;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace TLD15.Pages;

public sealed class IndexModel(
    IMediator mediator,
    IConfiguration configuration) : PageModel, IPagePublic
{
    public static MetaPagePublic MetaPublic => new()
    {
        Id = "index",
        Title = Globals.Brand.Company,
        LocalUrl = "/Index",
    };

    public string Host => configuration.GetSection(Globals.Configuration.ApplicationHost).Value!;
    public IMediator Mediator => mediator;
}
