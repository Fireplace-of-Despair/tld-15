using ACherryPie.Feature;
using ACherryPie.Pages;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace TLD15.Pages.Introduction;

public class PreviewIntroductionPartialModel : PageModel, IPartial
{
    public static string Anchor => "preview_introduction";
    public string Title => "Introduction";

    public AFeatureIntroduction.ResponseRead? Data { get; set; }

    public static async Task<PreviewIntroductionPartialModel> InitializeAsync(IMediator mediator)
    {
        var lore = await FeatureRunner.Run(async () =>
        {
            return await mediator.Send(new AFeatureIntroduction.RequestRead());
        });

        var result = new PreviewIntroductionPartialModel();

        if (lore.Incident is not null)
        {
            result.ModelState.AddModelError("Introduction", lore.Incident.Description);
        }
        else
        {
            result.Data = lore.Data!;
        }

        return result;
    }
}
