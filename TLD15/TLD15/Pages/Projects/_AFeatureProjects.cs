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

namespace TLD15.Pages.Projects;

public sealed class AFeatureProjects
{
    public sealed class ResponsePreview
    {
        public required Guid? Id { get; set; }
        public string IdFriendly { get; set; } = string.Empty;
        public string Division { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;

        public string PosterUrl { get; set; } = string.Empty;
        public string PosterAlt { get; set; } = string.Empty;

        public required Dictionary<string, string> Links { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public sealed class RequestPreview : IRequest<List<ResponsePreview>>;

    public sealed class HandlerPreview(IMongoClient client)
        : IRequestHandler<RequestPreview, List<ResponsePreview>>
    {
        public async Task<List<ResponsePreview>> Handle(RequestPreview request, CancellationToken cancellationToken)
        {
            var database = client.GetDatabase(EntityProject.Database);
            var collection = database.GetCollection<EntityProject>(EntityProject.Collection);

            var documents = await collection.Find(FilterDefinition<EntityProject>.Empty)
                .SortByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);

            return documents.ConvertAll(x => new ResponsePreview
            {
                Id = x.Id,
                IdFriendly = x.IdFriendly,
                Division = x.DivisionCode,
                Title = x.Title,
                Subtitle = x.SubTitle.TrimByWord(230),
                Links = x.Links,
                PosterUrl = x.PosterUrl,
                PosterAlt = x.PosterAlt,
                CreatedAt = x.CreatedAt
            });
        }
    }



    public sealed class ResponseRead : IRequest<ResponseId<Guid>>
    {
        public Guid? Id { get; set; }
        public string IdFriendly { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string SubTitle { get; set; } = string.Empty;
        public string PosterUrl { get; set; } = string.Empty;
        public string PosterAlt { get; set; } = string.Empty;
        public string DivisionCode { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ContentHtml { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public Dictionary<string, string> Links { get; set; } = [];
        public long Version { get; set; }

        public string[] GetContentHtmlSplitByHR()
        {
            return ContentHtml.Split(["<hr>", "<hr/>", "<hr />"], StringSplitOptions.RemoveEmptyEntries);
        }
    }

    public sealed class RequestRead : IRequest<ResponseRead>
    {
        public required Guid? Id { get; set; }
        public required string? IdFriendly { get; set; }
    }

    public sealed class HandlerRead(IMongoClient client)
        : IRequestHandler<RequestRead, ResponseRead>
    {
        public async Task<ResponseRead> Handle(RequestRead request, CancellationToken cancellationToken)
        {
            var database = client.GetDatabase(EntityProject.Database);
            var collection = database.GetCollection<EntityProject>(EntityProject.Collection);

            var document = await collection.Find(x => x.IdFriendly == request.IdFriendly || x.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            return new ResponseRead
            {
                Id = document.Id,
                IdFriendly = document.IdFriendly,
                Title = document.Title,
                SubTitle = document.SubTitle,
                PosterUrl = document.PosterUrl,
                PosterAlt = document.PosterAlt,
                DivisionCode = document.DivisionCode,
                Content = document.Content,
                ContentHtml = document.ContentHtml,
                CreatedAt = document.CreatedAt,
                Version = document.Version,
                Links = document.Links
            };
        }
    }

    public sealed class HandlerSave(IMongoClient client) : IRequestHandler<ResponseRead, ResponseId<Guid>>
    {
        public async Task<ResponseId<Guid>> Handle(ResponseRead request, CancellationToken cancellationToken)
        {
            var database = client.GetDatabase(EntityProject.Database);
            var collection = database.GetCollection<EntityProject>(EntityProject.Collection);

            var item = new EntityProject { Id = Guid.NewGuid() };
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

            item.IdFriendly = request.IdFriendly.ToLowerInvariant();
            item.Title = request.Title;
            item.SubTitle = request.SubTitle;
            item.PosterUrl = request.PosterUrl;
            item.PosterAlt = request.PosterAlt;
            item.DivisionCode = request.DivisionCode;
            item.Content = request.Content;
            item.ContentHtml = request.ContentHtml;
            item.Links = request.Links;
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
            var database = client.GetDatabase(EntityProject.Database);
            var collection = database.GetCollection<EntityProject>(EntityProject.Collection);
            await collection.DeleteManyAsync(x => x.Id == request.Id, cancellationToken);

            return new ResponseId<Guid>
            {
                Id = request.Id
            };
        }
    }
}
