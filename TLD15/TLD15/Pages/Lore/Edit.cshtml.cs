using ACherryPie.Feature;
using ACherryPie.Incidents;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace TLD15.Pages.Lore;

[Authorize]
public class EditLoreModel(IMediator mediator) : PageModel
{
    [BindProperty]
    public AFeatureLore.ResponseRead? Model { get; set; }

    public async Task OnGetAsync()
    {
        Model = await mediator.Send(new AFeatureLore.RequestRead());
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
