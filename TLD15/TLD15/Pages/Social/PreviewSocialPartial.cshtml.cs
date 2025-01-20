using ACherryPie.Feature;
using ACherryPie.Pages;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TLD15.Utils;

namespace TLD15.Pages.Social;

public sealed class PreviewSocialFeature : IFeature
{
    public sealed class Response
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

    public sealed class Request : IRequest<List<Response>> { }

    public sealed class GetPreviewContactsHandler(IMongoClient client) : IRequestHandler<Request, List<Response>>
    {
        public async Task<List<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var database = client.GetDatabase(EntitySocial.Database);
            var collection = database.GetCollection<EntitySocial>(EntitySocial.Collection);

            var documents = await collection.Find(FilterDefinition<EntitySocial>.Empty)
                .SortByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);

            return documents.ConvertAll(x => new Response
            {
                Name = x.Name,
                Url = x.Url,
            });
        }
    }
}

public class PreviewSocialPartialModel : PageModel, IPartial
{
    public static string Id => "PreviewContactsPartial";

    public static async Task<PreviewSocialPartialModel> InitializeAsync(IMediator mediator)
    {
        var contacts = await FeatureRunner.Run(async () =>
        {
            return await mediator.Send(new PreviewSocialFeature.Request());
        });

        var result = new PreviewSocialPartialModel();

        if (contacts.Incident is not null)
        {
            result.ModelState.AddModelError("Social", contacts.Incident.Description);
        }
        else
        {
            result.Data = contacts.Data ?? [];
        }

        return result;
    }

    public List<PreviewSocialFeature.Response> Data { get; set; } = [];
    public string Title => "Contacts";


    public void OnGet()
    {
        throw new NotSupportedException();
    }
}
