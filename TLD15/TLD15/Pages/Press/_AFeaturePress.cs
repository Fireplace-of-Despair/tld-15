using ACherryPie.Incidents;
using ACherryPie.Requests;
using ACherryPie.Responses;
using Common.Utils;
using MediatR;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TLD15.Infrastructure;

namespace TLD15.Pages.Press;

public sealed class AFeaturePress
{
    public sealed class ResponsePreview
    {
        public Guid? Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;

        public string PosterUrl { get; set; } = string.Empty;
        public string PosterAlt { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }

    public sealed class RequestPreview : IRequest<List<ResponsePreview>>;

    public sealed class HandlerPreview(IMongoClient client)
        : IRequestHandler<RequestPreview, List<ResponsePreview>>
    {
        public async Task<List<ResponsePreview>> Handle(RequestPreview request, CancellationToken cancellationToken)
        {
            var database = client.GetDatabase(EntityPress.Database);
            var collection = database.GetCollection<EntityPress>(EntityPress.Collection);

            var documents = await collection.Find(FilterDefinition<EntityPress>.Empty)
                .SortByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);

            return documents.ConvertAll(x => new ResponsePreview
            {
                Id = x.Id,
                Title = x.Title,
                Subtitle = x.Subtitle.TrimByWord(230),
                PosterUrl = x.PosterUrl,
                PosterAlt = x.PosterAlt,
                CreatedAt = x.CreatedAt
            });
        }
    }


    public sealed class ResponseRead : IRequest<ResponseId<Guid>>
    {
        public Guid? Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;

        public string PosterUrl { get; set; } = string.Empty;
        public string PosterAlt { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;

        public long Version { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public sealed class RequestRead : IRequest<ResponseRead>
    {
        public required Guid? Id { get; set; }
    }

    public sealed class HandlerRead(IMongoClient client)
        : IRequestHandler<RequestRead, ResponseRead>
    {
        public async Task<ResponseRead> Handle(RequestRead request, CancellationToken cancellationToken)
        {
            var database = client.GetDatabase(EntityPress.Database);
            var collection = database.GetCollection<EntityPress>(EntityPress.Collection);

            var document = await collection.Find(x => x.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            return new ResponseRead
            {
                Id = document.Id,
                Url = document.Url,
                Title = document.Title,
                Subtitle = document.Subtitle,
                PosterUrl = document.PosterUrl,
                PosterAlt = document.PosterAlt,
                Version = document.Version,
                CreatedAt = document.CreatedAt
            };
        }
    }

    public sealed class HandlerSave(IMongoClient client) : IRequestHandler<ResponseRead, ResponseId<Guid>>
    {
        public async Task<ResponseId<Guid>> Handle(ResponseRead request, CancellationToken cancellationToken)
        {
            var database = client.GetDatabase(EntityPress.Database);
            var collection = database.GetCollection<EntityPress>(EntityPress.Collection);

            var item = new EntityPress { Id = Guid.NewGuid() };
            if (request.Id.HasValue)
            {
                item = await collection.Find(x => x.Id == request.Id.Value)
                    .FirstOrDefaultAsync(cancellationToken)
                    ?? throw new IncidentException(IncidentCode.NotFound);
            }
            else
            {
                await collection.InsertOneAsync(item, collection.GetDefaultInsert(), cancellationToken);
            }

            item.Title = request.Title;
            item.Subtitle = request.Subtitle;
            item.PosterUrl = request.PosterUrl;
            item.PosterAlt = request.PosterAlt;
            item.Url = request.Url;

            item.Bump(request.Version);

            await collection.ReplaceOneAsync(x => x.Id == item.Id, item, cancellationToken: cancellationToken);

            return new ResponseId<Guid> { Id = item.Id };
        }
    }


    public sealed class RequestDelete : RequestId<Guid>, IRequest<ResponseId<Guid>>;

    public sealed class HandlerDelete(IMongoClient client) : IRequestHandler<RequestDelete, ResponseId<Guid>>
    {
        public async Task<ResponseId<Guid>> Handle(RequestDelete request, CancellationToken cancellationToken)
        {
            var database = client.GetDatabase(EntityPress.Database);
            var collection = database.GetCollection<EntityPress>(EntityPress.Collection);
            await collection.DeleteManyAsync(x => x.Id == request.Id, cancellationToken);

            return new ResponseId<Guid>
            {
                Id = request.Id
            };
        }
    }

}