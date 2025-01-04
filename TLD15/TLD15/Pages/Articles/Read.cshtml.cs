using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace TLD15.Pages.Articles;

public sealed class ReadFeature(IMongoDatabase database) : PageModel
{
    public sealed class ReadResponse
    {
        public string Title { get; set; } = string.Empty;
        public string SubTitle { get; set; } = string.Empty;
        public string PosterUrl { get; set; } = string.Empty;
        public string PosterAlt { get; set; } = string.Empty;
        public string DivisionCode { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ContentHtml { get; set; } = string.Empty;
    }
    
    public sealed class ReadRequest : IRequest<ReadResponse>
    {
        public required string IdFriendly { get; set; }
    }

    [BindProperty]
    public ReadResponse Model { get; set; } = new ReadResponse();


    public async Task<IActionResult> OnGetAsync(string idFriendly)
    {
        var collection = database.GetCollection<EntityArticle>(EntityArticle.Collection);
        var document = await collection.Find(x => x.IdFriendly == idFriendly)
            .FirstOrDefaultAsync();

        Model = new ReadResponse
        {
            Title = document.Title,
            SubTitle = document.SubTitle,
            PosterUrl = document.PosterUrl,
            PosterAlt = document.PosterAlt,
            DivisionCode = document.DivisionCode,
            Content = document.Content,
            ContentHtml = document.ContentHtml,
        };

        return Page();
    }

    public static string[] SplitHtmlByHR(string html)
    {
        return html.Split(["<hr>", "<hr/>", "<hr />"], StringSplitOptions.RemoveEmptyEntries);
    }
}
