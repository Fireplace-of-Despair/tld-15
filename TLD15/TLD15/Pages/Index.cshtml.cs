using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace TLD15.Pages;

public class IndexModel(IMediator mediator) : PageModel
{
    //public List<GetPublicationsLatest.Response> Publications { get; set; } = [];

    //public async Task OnGet()
    //{
    //    var ss = await mediator.Send(new GetPublicationsLatest.Request());
    //    Publications = ss;
    //}
}
