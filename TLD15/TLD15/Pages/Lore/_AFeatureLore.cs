using MediatR;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.Threading;
using System;
using ACherryPie.Responses;
using TLD15.Infrastructure;

namespace TLD15.Pages.Lore;

public sealed class AFeatureLore
{
    public sealed class Response
    {
        public required Guid Id { get; set; }

        public required string Title { get; set; } = string.Empty;
        public required string PosterUrl { get; set; } = string.Empty;
        public required string PosterAlt { get; set; } = string.Empty;
        public required string Content { get; set; } = string.Empty;
        public required string ContentHtml { get; set; } = string.Empty;

        public required string Language { get; set; } = string.Empty;
        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; }
        public required long Version { get; set; }
    }

    public sealed class Request : IRequest<Response> { }

    public sealed class Handler(IMongoClient client) : IRequestHandler<Request, Response>
    {
        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var database = client.GetDatabase(EntityLore.Database);
            var collection = database.GetCollection<EntityLore>(EntityLore.Collection);

            var document = await collection.Find(FilterDefinition<EntityLore>.Empty)
                .FirstOrDefaultAsync(cancellationToken)
                ??
                new EntityLore
                {
                    Id = Guid.Empty,
                    Language = "eng",
                    Title = "Lore",
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

    public sealed class EditRequest : IRequest<ResponseId<Guid>>
    {
        public required Guid? Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string PosterUrl { get; set; } = string.Empty;
        public required string PosterAlt { get; set; } = string.Empty;
        public required string Content { get; set; } = string.Empty;
        public required string ContentHtml { get; set; } = string.Empty;

        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; }
        public required long Version { get; set; }
    }

    public sealed class HandlerSave(IMongoClient client) : IRequestHandler<EditRequest, ResponseId<Guid>>
    {
        public async Task<ResponseId<Guid>> Handle(EditRequest request, CancellationToken cancellationToken)
        {
            var database = client.GetDatabase(EntityLore.Database);
            var collection = database.GetCollection<EntityLore>(EntityLore.Collection);
            var documents = await collection.FindAsync(FilterDefinition<EntityLore>.Empty, cancellationToken: cancellationToken);
            var document = await documents.FirstOrDefaultAsync(cancellationToken);
            document = document ?? new EntityLore
            {
                Id = Guid.NewGuid(),
            };

            document.Title = request.Title;
            document.PosterUrl = request.PosterUrl;
            document.PosterAlt = request.PosterAlt;
            document.Content = request.Content;
            document.ContentHtml = request.ContentHtml;
            document.Bump(request.Version);

            if (request.Id == null)
            {
                await collection.InsertOneAsync(document, collection.GetDefaultInsert(), cancellationToken);
            }
            else
            {
                await collection.ReplaceOneAsync(x => x.Id == document.Id, document, cancellationToken: cancellationToken);
            }
            return new ResponseId<Guid> { Id = document.Id };
        }
    }
}
