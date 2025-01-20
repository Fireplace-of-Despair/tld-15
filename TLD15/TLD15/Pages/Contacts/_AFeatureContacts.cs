using MediatR;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TLD15.Utils;
using ACherryPie.Responses;
using System;
using ACherryPie.Incidents;
using TLD15.Infrastructure;

namespace TLD15.Pages.Contacts;

public sealed class AFeatureContacts
{
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
            var database = client.GetDatabase(EntityContact.Database);
            var collection = database.GetCollection<EntityContact>(EntityContact.Collection);

            var documents = await collection.Find(FilterDefinition<EntityContact>.Empty)
                .SortByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);

            return documents.ConvertAll(x => new ResponsePreview
            {
                Name = x.Name,
                Url = x.Url,
            });
        }
    }

    public sealed class EditRequest : IRequest<ResponseId<Guid>>
    {
        public required Guid? Id { get; set; }
        public required string Name { get; set; }
        public required string Url { get; set; }

        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; }
        public required long Version { get; set; }
    }

    public sealed class ViewRequest : IRequest<List<EditRequest>>
    {
        public required Guid? Id { get; set; }
    }

    public sealed class HandlerView(IMongoClient client) : IRequestHandler<ViewRequest, List<EditRequest>>
    {
        public async Task<List<EditRequest>> Handle(ViewRequest request, CancellationToken cancellationToken)
        {
            var database = client.GetDatabase(EntityContact.Database);
            var collection = database.GetCollection<EntityContact>(EntityContact.Collection);

            var filter = request.Id.HasValue ?
                Builders<EntityContact>.Filter.Eq(x => x.Id, request.Id.Value)
                : FilterDefinition<EntityContact>.Empty;

            var documents = await collection.FindAsync(filter, cancellationToken: cancellationToken);
            var document = await documents.ToListAsync(cancellationToken);

            return document.ConvertAll(x => new EditRequest
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

    public sealed class HandlerEdit(IMongoClient client) : IRequestHandler<EditRequest, ResponseId<Guid>>
    {
        public async Task<ResponseId<Guid>> Handle(EditRequest request, CancellationToken cancellationToken)
        {
            var database = client.GetDatabase(EntityContact.Database);
            var collection = database.GetCollection<EntityContact>(EntityContact.Collection);

            var item = new EntityContact { Id = Guid.NewGuid() };
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

    public sealed class DeleteRequest : IRequest<ResponseId<Guid>>
    {
        public required Guid Id { get; set; }
    }


    public sealed class HandlerDelete(IMongoClient client) : IRequestHandler<DeleteRequest, ResponseId<Guid>>
    {
        public async Task<ResponseId<Guid>> Handle(DeleteRequest request, CancellationToken cancellationToken)
        {
            var database = client.GetDatabase(EntityContact.Database);
            var collection = database.GetCollection<EntityContact>(EntityContact.Collection);

            await collection.DeleteOneAsync(Builders<EntityContact>.Filter.Eq(x => x.Id, request.Id), new DeleteOptions(), cancellationToken);

            return new ResponseId<Guid> { Id = request.Id };
        }
    }
}
