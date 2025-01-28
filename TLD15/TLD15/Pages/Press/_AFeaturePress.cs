using Common.Utils;
using MediatR;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TLD15.Pages.Press;

public sealed class AFeaturePress
{
    public sealed class ResponsePreview
    {
        public required Guid? Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;

        public string PosterUrl { get; set; } = string.Empty;
        public string PosterAlt { get; set; } = string.Empty;

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
                Subtitle = x.SubTitle.TrimByWord(230),
                PosterUrl = x.PosterUrl,
                PosterAlt = x.PosterAlt,
                CreatedAt = x.CreatedAt
            });
        }
    }
}
