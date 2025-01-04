using Microsoft.AspNetCore.Mvc;
using System;

namespace TLD15.Pages.Shared.Components.ArticleCard;

public sealed class ArticleCard : ViewComponent
{
    public sealed class Article
    {
        public required string Division { get; set; }
        public required DateTime CreatedAt { get; set; }

        public required Guid Id { get; set; }
        public required string IdFriendly { get; set; }
        public required string Title { get; set; }
        public required string Subtitle { get; set; }

        public required string PosterUrl { get; set; }
        public required string PosterAlt { get; set; }
    }

    public IViewComponentResult Invoke(Article article)
    {
        return View(GetType().Name, article);
    }
}
