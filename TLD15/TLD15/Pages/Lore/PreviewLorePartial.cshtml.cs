using ACherryPie.Feature;
using ACherryPie.Pages;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace TLD15.Pages.Lore;

public sealed class PreviewLoreModel() : PageModel, IPartial
{
    public static string Id => "preview_lore";
    public string Anchor => Id;
    public string Title => "Lore";

    public AFeatureLore.ResponseRead? Data { get; set; }

    public static async Task<PreviewLoreModel> InitializeAsync(IMediator mediator)
    {
        var lore = await FeatureRunner.Run(async () =>
        {
            return await mediator.Send(new AFeatureLore.RequestRead());
        });

        var result = new PreviewLoreModel();

        if (lore.Incident is not null)
        {
            result.ModelState.AddModelError("Lore", lore.Incident.Description);
        }
        else
        {
            result.Data = lore.Data!;
        }

        return result;
    }
}
