using ACherryPie.Incidents;
using Common.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TLD15.Pages.Account;
using TLD15.Pages.Articles;
using TLD15.Pages.Shared.Components.ArticleCard;


namespace TLD15.Pages;

public class IndexModel(IMongoDatabase database) : PageModel
{
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
    }
}
