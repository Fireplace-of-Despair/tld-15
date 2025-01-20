using ACherryPie.Feature;
using ACherryPie.Incidents;
using ACherryPie.Pages;
using ACherryPie.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TLD15.Infrastructure;

namespace TLD15.Pages.Social;

public sealed class EditSocialFeature : IFeature
{
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
            var database = client.GetDatabase(EntitySocial.Database);
            var collection = database.GetCollection<EntitySocial>(EntitySocial.Collection);

            var filter = request.Id.HasValue ?
                Builders<EntitySocial>.Filter.Eq(x => x.Id, request.Id.Value)
                : FilterDefinition<EntitySocial>.Empty;

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



    public sealed class DeleteRequest : IRequest<ResponseId<Guid>>
    {
        public required Guid Id { get; set; }
    }


    public sealed class HandlerDelete(IMongoClient client) : IRequestHandler<DeleteRequest, ResponseId<Guid>>
    {
        public async Task<ResponseId<Guid>> Handle(DeleteRequest request, CancellationToken cancellationToken)
        {
            var database = client.GetDatabase(EntitySocial.Database);
            var collection = database.GetCollection<EntitySocial>(EntitySocial.Collection);

            await collection.DeleteOneAsync(Builders<EntitySocial>.Filter.Eq(x => x.Id, request.Id), new DeleteOptions(), cancellationToken);

            return new ResponseId<Guid> { Id = request.Id };
        }
    }


}

[Authorize]
public class EditModel(IMediator mediator) : PageModel, IPageAdmin
{
    public List<EditSocialFeature.EditRequest> Data { get; set; } = [];

    public static MetaData MetaData => new()
    {
        Id = "Social",
        LocalUrl = "/Social/Edit"
    };

    public async Task OnGet()
    {
        Data = await mediator.Send(new EditSocialFeature.ViewRequest { Id = null });
    }

    public async Task<IActionResult> OnGetItem(Guid id)
    {
        var result = await mediator.Send(new EditSocialFeature.ViewRequest { Id = id });
        var item = result[0];
        
        return Partial("_TableRow", item);
    }
    public async Task<IActionResult> OnGetEditItem(Guid id)
    {
        var result = await mediator.Send(new EditSocialFeature.ViewRequest { Id = id });
        var item = result[0];
        return Partial("_TableRowForm", item);
    }

    public async Task<IActionResult> OnPutItem(EditSocialFeature.EditRequest contact)
    {
        await mediator.Send(contact);

        return Partial("_TableRow", contact);
    }

    public async Task<IActionResult> OnDeleteItem(Guid id)
    {
        await mediator.Send(new EditSocialFeature.DeleteRequest { Id = id } );

        return new OkResult();
    }

    public async Task<IActionResult> OnPostItem(EditSocialFeature.EditRequest contact)
    {
        await mediator.Send(contact);

        return new OkResult();
    }

    public IActionResult OnGetModal()
    {
        return Partial("_ModalPartial", new EditSocialFeature.EditRequest()
        {
            Id = null,
            Name = "NEW",
            CreatedAt = DateTime.Now,            
            UpdatedAt = DateTime.Now,
            Url = string.Empty,
            Version = 0
        });
    }
}
