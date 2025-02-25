using ACherryPie.Feature;
using ACherryPie.Pages;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TLD15.Pages.Social;

public class PreviewSocialPartialModel : PageModel, IPartial
{
    public static MetaPartialPublic MetaPublic => new()
    {
        Id = "preview_social",
        Title = "Social",
    };

    public List<ASocialFeature.ResponsePreview> Data { get; set; } = [];
    
    public static async Task<PreviewSocialPartialModel> InitializeAsync(IMediator mediator)
    {
        var socials = await FeatureRunner.Run(async () => await mediator.Send(new ASocialFeature.RequestPreview()));

        var model = new PreviewSocialPartialModel();

        if (socials.Incident is not null)
        {
            model.ModelState.AddModelError("Social", socials.Incident.Description);
        }
        else
        {
            model.Data = socials.Data ?? [];
        }

        return model;
    }
}
