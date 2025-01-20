using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TLD15.Pages;

public sealed class IndexModel(IMediator mediator) : PageModel
{
    public IMediator Mediator => mediator;
}
