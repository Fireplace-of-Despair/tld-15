using ACherryPie.Feature;
using ACherryPie.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TLD15.Pages.Lore;

public sealed class GetLoreFeature : IFeature
{
    public sealed class Response
    {
        public required Guid Id { get; set; }

        public required string Title { get; set; } = string.Empty;
        public required string SubTitle { get; set; } = string.Empty;
        public required string PosterUrl { get; set; } = string.Empty;
        public required string PosterAlt { get; set; } = string.Empty;
        public required string Content { get; set; } = string.Empty;
        public required string ContentHtml { get; set; } = string.Empty;

        public required string Language { get; set; } = string.Empty;
        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; }
        public required long Version { get; set; }
    }

    public sealed class Request : IRequest<Response>, IRequestLanguage
    {
        public string Language { get; set; } = "eng";
    }

    public sealed class Handler(IMongoDatabase database) : IRequestHandler<Request, Response>
    {
        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var collection = database.GetCollection<EntityLore>(EntityLore.Collection);

            var document = await collection.Find(x => x.Language == request.Language)
                .FirstOrDefaultAsync(cancellationToken)
                ??
                new EntityLore
                {                 
                    Id = Guid.Empty,
                    Language = "eng",
                    Title = "Lore",
                    SubTitle = "You want",
                    PosterUrl = "https://fireplace-of-despair.org/images/chief.jpg",
                    PosterAlt = "lol",
                    Content = @"The lore and history of the Fireplace of Despair had slipped away into the shadows of time, like smoke from a half-forgotten cigarette. The Fireplace of Despair had always been here, lurking in the corners, barely noticed by most.

But behind it all, there is one man, a constant presence from the beginning to the very end.

A seasoned individual with a mind as sharp as his tailored suits, he'd seen it all. Nothing escaped his notice. At the head of the Fireplace of Despair stands a figure cloaked in mystery, known as Shevtsov Stan. Or simply, the Chief. A ghost from a distant land, his past shrouded in layers of intrigue and rumor. To some, he is a myth, a legend whispered among the people.

Yet beneath the cloak of enigma, there is a man with a soft smile, a soul steeped in shadows, and too many skills for one person alone. The Chief's true motives remain hidden in the murk, his loyalty a secret he keeps close to the chest. To those who seek his help, he makes no promises but always delivers results. ",
                    ContentHtml = @"The lore and history of the Fireplace of Despair had slipped away into the shadows of time, like smoke from a half-forgotten cigarette. The Fireplace of Despair had always been here, lurking in the corners, barely noticed by most.

But behind it all, there is one man, a constant presence from the beginning to the very end.

A seasoned individual with a mind as sharp as his tailored suits, he'd seen it all. Nothing escaped his notice. At the head of the Fireplace of Despair stands a figure cloaked in mystery, known as Shevtsov Stan. Or simply, the Chief. A ghost from a distant land, his past shrouded in layers of intrigue and rumor. To some, he is a myth, a legend whispered among the people.

Yet beneath the cloak of enigma, there is a man with a soft smile, a soul steeped in shadows, and too many skills for one person alone. The Chief's true motives remain hidden in the murk, his loyalty a secret he keeps close to the chest. To those who seek his help, he makes no promises but always delivers results. ",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Version = 0,
                };

            return new Response
            {
                Id = document.Id,
                Language = document.Language,
                Title = document.Title,
                SubTitle = document.SubTitle,
                PosterUrl = document.PosterUrl,
                PosterAlt = document.PosterAlt,
                Content = document.Content,
                ContentHtml = document.ContentHtml,
                CreatedAt = document.CreatedAt,
                UpdatedAt = document.UpdatedAt,
                Version = document.Version,
            };
        }
    }
}

public sealed class ReadLoreModel() : PageModel
{
    public GetLoreFeature.Response? Data;

    public IActionResult OnGetPartial()
    {
        return new PartialViewResult
        {
            ViewName = "ReadLore",
            ViewData = ViewData,
        };
    }
}
