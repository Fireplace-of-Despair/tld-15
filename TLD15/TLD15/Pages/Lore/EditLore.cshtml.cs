using ACherryPie.Feature;
using ACherryPie.Requests;
using ACherryPie.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace TLD15.Pages.Lore;

public sealed class SaveLoreFeature : IFeature
{
    public sealed class Response : ResponseId<Guid>
    {
    }

    public sealed class Request : IRequest<Response>
    {
        public required Guid? Id { get; set; }

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

    public sealed class Handler(IMongoDatabase database) : IRequestHandler<Request, Response>
    {
        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var collection = database.GetCollection<EntityLore>(EntityLore.Collection);

            EntityLore entity;
            if (request.Id.HasValue)
            {
                
                entity = await collection.Find(x => x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
                entity.Bump(request.Version);
                await collection.DeleteOneAsync(Builders<EntityLore>.Filter.Eq("_id", entity.Id), cancellationToken);
            }
            else
            {
                entity = new EntityLore
                { 
                    Id = Guid.NewGuid()
                };
            }

            entity.Title = request.Title;
            entity.SubTitle = request.SubTitle;
            entity.PosterUrl = request.PosterUrl;
            entity.PosterAlt = request.PosterAlt;
            entity.Content = request.Content;
            entity.ContentHtml = request.ContentHtml;
            entity.Language = request.Language;

            await collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
            return new Response { Id = entity.Id };
        }
    }
}


public class EditLoreModel(IMediator mediator) : PageModel
{
    public SaveLoreFeature.Request Data { get; set; }

    public async Task OnGetAsync()
    {
        var result = await FeatureRunner.Run(async () => { return await mediator.Send(new GetLoreFeature.Request()); });
        if (result.Incident != null)
        {
            Data = new SaveLoreFeature.Request
            {
                Id = result.Data!.Id,
                Title = result.Data!.Title,
                SubTitle = result.Data!.SubTitle,
                PosterUrl = result.Data!.PosterUrl,
                PosterAlt = result.Data!.PosterAlt,
                Content = result.Data!.Content,
                ContentHtml = result.Data!.ContentHtml,
                Language = result.Data!.Language,
                CreatedAt = result.Data!.CreatedAt,
                UpdatedAt = result.Data!.UpdatedAt,
                Version = result.Data!.Version,
            };
        }
    }
}
