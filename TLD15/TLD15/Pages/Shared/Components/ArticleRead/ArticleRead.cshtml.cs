using Microsoft.AspNetCore.Mvc;
using System;

namespace TLD15.Pages.Shared.Components.ArticleRead;

public sealed class ArticleRead : ViewComponent
{
    public sealed class Model
    {
        public required string Title { get; set; } = string.Empty;
        public required string SubTitle { get; set; } = string.Empty;
        public required string PosterUrl { get; set; } = string.Empty;
        public required string PosterAlt { get; set; } = string.Empty;
        public required string DivisionCode { get; set; } = string.Empty;
        public required string Content { get; set; } = string.Empty;
        public required string ContentHtml { get; set; } = string.Empty;
        public required long Version { get; set; }
        public required DateTime CreatedAt { get; set; }

        public string[] GetContentHtmlSplitByHR()
        {
            return ContentHtml.Split(["<hr>", "<hr/>", "<hr />"], StringSplitOptions.RemoveEmptyEntries);
        }
    }

    public IViewComponentResult Invoke(Model model)
    {
        return View(GetType().Name, model);
    }
}
