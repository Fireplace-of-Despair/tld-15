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
        public required string Title { get; set; } = string.Empty;
        public required string SubTitle { get; set; } = string.Empty;
        public required string PosterUrl { get; set; } = string.Empty;
        public required string PosterAlt { get; set; } = string.Empty;
        public required string DivisionCode { get; set; } = string.Empty;
        public required string Content { get; set; } = string.Empty;
        public required string ContentHtml { get; set; } = string.Empty;
        public required DateTime CreatedAt { get; set; }
        public required long Version { get; set; }
    }
    
    public sealed class ReadRequest : IRequest<ReadResponse>
    {
        public required string IdFriendly { get; set; }
    }

    [BindProperty]
    public required ReadResponse Model { get; set; }


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
            CreatedAt = document.CreatedAt,
            Version = document.Version
        };

        return Page();
    }
}
