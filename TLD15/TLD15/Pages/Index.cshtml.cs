using ACherryPie.Feature;
using Common.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TLD15.Pages.Articles;
using TLD15.Pages.Shared.Components.ArticleCard;

namespace TLD15.Pages;

public class IndexModel(IMongoDatabase database, IMediator mediator) : PageModel
{
    public IMediator Mediator => mediator;
    public Lore.GetLoreFeature.Response? Lore { get; set; }

    public List<ArticleCard.Article> Articles { get; set; } = [];

    public async Task OnGet()
    {
        var collection = database.GetCollection<EntityArticle>(EntityArticle.Collection, new MongoCollectionSettings
        {
            ReadEncoding = new UTF8Encoding(),
        });
        var documents = await collection
            .Find(Builders<EntityArticle>.Filter.Empty)
            .SortByDescending(x => x.CreatedAt)
            .ToListAsync();

        Articles = documents.ConvertAll(x => new ArticleCard.Article
        {
            Division = x.DivisionCode,
            CreatedAt = x.CreatedAt,
            Id = x.Id,
            IdFriendly = x.IdFriendly,
            Title = x.Title,
            Subtitle = x.SubTitle.TrimByWord(230),
            PosterUrl = x.PosterUrl,
            PosterAlt = x.PosterAlt,
        });


        var lore = await FeatureRunner.Run(async () =>
        {
            return await mediator.Send(new Lore.GetLoreFeature.Request());
        });
        if (lore.Incident is not null)
        {
            ModelState.AddModelError("Lore", lore.Incident.Description);
        }
        else
        {
            Lore = lore.Data;
        }
    }
}
