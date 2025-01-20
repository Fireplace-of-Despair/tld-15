using ACherryPie.Feature;
using ACherryPie.Pages;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TLD15.Pages.Articles;

public class PreviewArticlesPartialModel() : PageModel, IPartial
{
    public static string Id => $"articles_preview";
    public List<AFeatureArticle.ResponsePreview> Data { get; set; } = [];
    public string Title => "Articles";

    public static async Task<PreviewArticlesPartialModel> InitializeAsync(IMediator mediator)
    {
        var articles = await FeatureRunner.Run(async () =>
        {
            return await mediator.Send(new AFeatureArticle.RequestPreview());
        });

        var result = new PreviewArticlesPartialModel();

        if (articles.Incident is not null)
        {
            result.ModelState.AddModelError("Articles", articles.Incident.Description);
        }
        else
        {
            result.Data = articles.Data ?? [];
        }

        return result;
    }

}
