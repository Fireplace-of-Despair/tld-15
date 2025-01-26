using ACherryPie.Feature;
using ACherryPie.Pages;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TLD15.Pages.Social;

public class PreviewSocialPartialModel : PageModel, IPartial
{
    public static string Id => "preview_social";
    public string Anchor => Id;
    public string Title => "Social";

    public List<ASocialFeature.ResponsePreview> Data { get; set; } = [];
    
    public static async Task<PreviewSocialPartialModel> InitializeAsync(IMediator mediator)
    {
        var socials = await FeatureRunner.Run(async () =>
        {
            return await mediator.Send(new ASocialFeature.RequestPreview());
        });

        var result = new PreviewSocialPartialModel();

        if (socials.Incident is not null)
        {
            result.ModelState.AddModelError("Social", socials.Incident.Description);
        }
        else
        {
            result.Data = socials.Data ?? [];
        }

        return result;
    }
}
