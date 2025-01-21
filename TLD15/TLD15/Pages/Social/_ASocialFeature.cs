using ACherryPie.Responses;
using MediatR;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using ACherryPie.Incidents;
using TLD15.Infrastructure;
using TLD15.Utils;
using TLD15.Pages.Contacts;

namespace TLD15.Pages.Social;

public sealed class ASocialFeature
{
    public sealed class RequestEdit : IRequest<ResponseId<Guid>>
    {
        public required Guid? Id { get; set; }
        public required string Name { get; set; }
        public required string Url { get; set; }

        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; }
        public required long Version { get; set; }
    }

    public sealed class RequestView : IRequest<List<RequestEdit>>
    {
        public required Guid? Id { get; set; }
    }

    public sealed class HandlerView(IMongoClient client) : IRequestHandler<RequestView, List<RequestEdit>>
    {
        public async Task<List<RequestEdit>> Handle(RequestView request, CancellationToken cancellationToken)
        {
            var database = client.GetDatabase(EntitySocial.Database);
            var collection = database.GetCollection<EntitySocial>(EntitySocial.Collection);

            var filter = request.Id.HasValue ?
                Builders<EntitySocial>.Filter.Eq(x => x.Id, request.Id.Value)
                : FilterDefinition<EntitySocial>.Empty;

            var documents = await collection.FindAsync(filter, cancellationToken: cancellationToken);
            var document = await documents.ToListAsync(cancellationToken);

            return document.ConvertAll(x => new RequestEdit
            {
                Id = x.Id,
                Name = x.Name.ToLowerInvariant(),
                Url = x.Url.ToLowerInvariant(),
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Version = x.Version,
            });
        }
    }

    public sealed class HandlerEdit(IMongoClient client) : IRequestHandler<RequestEdit, ResponseId<Guid>>
    {
        public async Task<ResponseId<Guid>> Handle(RequestEdit request, CancellationToken cancellationToken)
        {
            var database = client.GetDatabase(EntitySocial.Database);
            var collection = database.GetCollection<EntitySocial>(EntitySocial.Collection);

            var item = new EntitySocial { Id = Guid.NewGuid() };
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

            item.Name = request.Name.ToLowerInvariant();
            item.Url = request.Url.ToLowerInvariant();
            item.Bump(request.Version);

            await collection.ReplaceOneAsync(x => x.Id == item.Id, item, cancellationToken: cancellationToken);

            return new ResponseId<Guid> { Id = item.Id };
        }
    }

    public sealed class RequestDelete : IRequest<ResponseId<Guid>>
    {
        public required Guid Id { get; set; }
    }


    public sealed class HandlerDelete(IMongoClient client) : IRequestHandler<RequestDelete, ResponseId<Guid>>
    {
        public async Task<ResponseId<Guid>> Handle(RequestDelete request, CancellationToken cancellationToken)
        {
            var database = client.GetDatabase(EntitySocial.Database);
            var collection = database.GetCollection<EntitySocial>(EntitySocial.Collection);

            await collection.DeleteOneAsync(Builders<EntitySocial>.Filter.Eq(x => x.Id, request.Id), new DeleteOptions(), cancellationToken);

            return new ResponseId<Guid> { Id = request.Id };
        }
    }

    public sealed class ResponsePreview
    {
        public required string Url { get; set; }
        public required string Name { get; set; }

        public string Image
        {
            get
            {
                return IconHelper.GetIcon(Name);
            }
        }
    }

    public sealed class RequestPreview : IRequest<List<ResponsePreview>> { }

    public sealed class HandlerPreview(IMongoClient client) : IRequestHandler<RequestPreview, List<ResponsePreview>>
    {
        public async Task<List<ResponsePreview>> Handle(RequestPreview request, CancellationToken cancellationToken)
        {
            var database = client.GetDatabase(EntitySocial.Database);
            var collection = database.GetCollection<EntitySocial>(EntitySocial.Collection);

            var documents = await collection.Find(FilterDefinition<EntitySocial>.Empty)
                .SortByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);

            return documents.ConvertAll(x => new ResponsePreview
            {
                Name = x.Name,
                Url = x.Url,
            });
        }
    }

}
