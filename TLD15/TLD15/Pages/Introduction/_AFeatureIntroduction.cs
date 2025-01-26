using ACherryPie.Responses;
using MediatR;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;
using TLD15.Infrastructure;

namespace TLD15.Pages.Introduction;

public sealed class AFeatureIntroduction
{
    public sealed class ResponseRead : IRequest<ResponseId<Guid>>
    {
        public required Guid? Id { get; set; }

        public required string PosterUrl { get; set; } = string.Empty;
        public required string PosterAlt { get; set; } = string.Empty;
        public required string Content { get; set; } = string.Empty;
        public required string ContentHtml { get; set; } = string.Empty;

        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; }
        public required long Version { get; set; }
    }

    public sealed class RequestRead : IRequest<ResponseRead?>;

    public sealed class HandlerRead(IMongoClient client) : IRequestHandler<RequestRead, ResponseRead?>
    {
        public async Task<ResponseRead?> Handle(RequestRead request, CancellationToken cancellationToken)
        {
            var database = client.GetDatabase(EntityIntroduction.Database);
            var collection = database.GetCollection<EntityIntroduction>(EntityIntroduction.Collection);

            var document = await collection.Find(FilterDefinition<EntityIntroduction>.Empty)
                .FirstOrDefaultAsync(cancellationToken);

            return document == null ? null : new ResponseRead
            {
                Id = document.Id,
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

    public sealed class HandlerSave(IMongoClient client) : IRequestHandler<ResponseRead, ResponseId<Guid>>
    {
        public async Task<ResponseId<Guid>> Handle(ResponseRead request, CancellationToken cancellationToken)
        {
            var database = client.GetDatabase(EntityIntroduction.Database);
            var collection = database.GetCollection<EntityIntroduction>(EntityIntroduction.Collection);
            var documents = await collection.FindAsync(FilterDefinition<EntityIntroduction>.Empty, cancellationToken: cancellationToken);
            var document = await documents.FirstOrDefaultAsync(cancellationToken);
            document = document ?? new EntityIntroduction
            {
                Id = Guid.NewGuid(),
            };

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
