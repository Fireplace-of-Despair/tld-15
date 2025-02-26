using ACherryPie.Feature;
using ACherryPie.Pages;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace TLD15.Pages.Lore;

public sealed class PreviewLoreModel : PageModel, IPartial
{
    public static MetaPartialPublic MetaPublic => new()
    {
        Id = "preview_lore",
        Title = "Lore",
    };

    public AFeatureLore.ResponseRead? Data { get; set; } = new();

    public static async Task<PreviewLoreModel> InitializeAsync(IMediator mediator)
    {
        var lore = await FeatureRunner.Run(async () => await mediator.Send(new AFeatureLore.RequestRead()));
        var model = new PreviewLoreModel();

        if (lore.Incident is not null)
        {
            model.ModelState.AddModelError(string.Empty, lore.Incident.Description);
        }
        else
        {
            model.Data = lore.Data!;
        }

        return model;
    }
}
