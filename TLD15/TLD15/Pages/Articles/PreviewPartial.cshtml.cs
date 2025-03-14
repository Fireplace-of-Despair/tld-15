using ACherryPie.Feature;
using ACherryPie.Pages;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TLD15.Pages.Articles;

public class PreviewArticlesPartialModel : PageModel, IPartial
{
    public static MetaPartialPublic MetaPublic => new()
    {
        Id = "preview_articles",
        Title = "Articles",
    };

    public List<AFeatureArticle.ResponsePreview> Data { get; set; } = [];

    public static async Task<PreviewArticlesPartialModel> InitializeAsync(IMediator mediator)
    {
        var articles = await FeatureRunner.Run(async () => await mediator.Send(new AFeatureArticle.RequestPreview()));

        var result = new PreviewArticlesPartialModel();

        if (articles.Incident is not null)
        {
            result.ModelState.AddModelError(string.Empty, articles.Incident.Description);
        }
        else
        {
            result.Data = articles.Data ?? [];
        }

        return result;
    }
}
