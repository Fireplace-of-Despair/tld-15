using ACherryPie.Feature;
using ACherryPie.Pages;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TLD15.Pages.Press;

public class PartialPreviewPressModel : PageModel, IPartial
{
    public static string Id => "preview_press";
    public string Anchor => Id;
    public string Title => "Press";

    public List<AFeaturePress.ResponsePreview> Data { get; set; } = [];

    public static async Task<PartialPreviewPressModel> InitializeAsync(IMediator mediator)
    {
        var articles = await FeatureRunner.Run(async () =>
        {
            return await mediator.Send(new AFeaturePress.RequestPreview());
        });

        var result = new PartialPreviewPressModel();

        if (articles.Incident is not null)
        {
            result.ModelState.AddModelError("Data", articles.Incident.Description);
        }
        else
        {
            result.Data = articles.Data ?? [];
        }

        return result;
    }
}
