using Common.Composition;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace TLD15.Pages;

public sealed class IndexModel(
    IMediator mediator,
    IConfiguration configuration) : PageModel
{
    public readonly string ApplicationHost = configuration.GetSection(Globals.Settings.ApplicationHost).Value!;
    public IMediator Mediator => mediator;
}
