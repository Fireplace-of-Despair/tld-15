using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace TLD15.Pages.Articles;

public sealed class ReadModel(IMediator mediator) : PageModel
{
    [BindProperty]
    public required AFeatureArticle.ResponseRead Model { get; set; }

    public async Task<IActionResult> OnGetAsync(string idFriendly)
    {
        Model = await mediator.Send(new AFeatureArticle.RequestRead { IdFriendly = idFriendly, Id = null });
        
        return Page();
    }
}
