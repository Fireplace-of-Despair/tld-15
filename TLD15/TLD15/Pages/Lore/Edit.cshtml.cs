using ACherryPie.Feature;
using ACherryPie.Incidents;
using ACherryPie.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;
using TLD15.Infrastructure;
using TLD15.Pages.Articles;

namespace TLD15.Pages.Lore;

[Authorize]
public class EditLoreModel(IMediator mediator) : PageModel
{
    [BindProperty]
    public AFeatureLore.Response? Model { get; set; }

    public async Task OnGetAsync()
    {
        Model = await mediator.Send(new AFeatureLore.Request());
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("Model", IncidentCode.General.GetDescription());
            return Page();
        }

        var result = await FeatureRunner.Run(async () => await mediator.Send(Model!));
        if (result.Incident != null)
        {
            ModelState.AddModelError("Model", result.Incident.Description);
            return Page();
        }

        return RedirectToPage(@"/Lore/Edit");
    }
}
