using ACherryPie.Feature;
using ACherryPie.Incidents;
using ACherryPie.Responses;
using Common.Composition;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;
using TLD15.Infrastructure;

namespace TLD15.Pages.Articles;

[Authorize]
public class EditFeature(IMediator mediator) : PageModel
{
    public static string FeatureName => "Edit Article";

    public sealed class EditRequest : IRequest<ResponseCreated<Guid>>
    {
        [BindProperty] public Guid? Id { get; set; } = null;
        [BindProperty] public string IdFriendly { get; set; } = string.Empty;
        [BindProperty] public string Title { get; set; } = string.Empty;
        [BindProperty] public string SubTitle { get; set; } = string.Empty;
        [BindProperty] public string PosterUrl { get; set; } = string.Empty;
        [BindProperty] public string PosterAlt { get; set; } = string.Empty;
        [BindProperty] public string DivisionCode { get; set; } = string.Empty;
        [BindProperty] public string Content { get; set; } = string.Empty;
        [BindProperty] public string ContentHtml { get; set; } = string.Empty;
        [BindProperty] public long Version { get; set; }
    }

    public sealed class ViewRequest : IRequest<EditRequest>
    {
        public Guid Id { get; set; }
    }

    public sealed class EditHandler(IMongoDatabase database) : IRequestHandler<EditRequest, ResponseCreated<Guid>>
    {
        public async Task<ResponseCreated<Guid>> Handle(EditRequest request, CancellationToken cancellationToken)
        {
            var collection = database.GetCollection<EntityArticle>(EntityArticle.Collection);

            var item = new EntityArticle { Id = Guid.NewGuid() };
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

            item.IdFriendly = request.IdFriendly;
            item.Title = request.Title;
            item.SubTitle = request.SubTitle;
            item.PosterUrl = request.PosterUrl;
            item.PosterAlt = request.PosterAlt;
            item.DivisionCode = request.DivisionCode;
            item.Content = request.Content;
            item.ContentHtml = request.ContentHtml;
            item.Bump(request.Version);

            await collection.ReplaceOneAsync(x => x.Id == item.Id, item, cancellationToken: cancellationToken);

            return new ResponseCreated<Guid> { Id = item.Id };
        }
    }

    public sealed class ViewHandler(IMongoDatabase database) : IRequestHandler<ViewRequest, EditRequest>
    {
        public async Task<EditRequest> Handle(ViewRequest request, CancellationToken cancellationToken)
        {
            var collection = database.GetCollection<EntityArticle>(EntityArticle.Collection);
            var document = await collection.Find(x => x.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            return new EditRequest
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
                Version = document.Version
            };
        }
    }

    [BindProperty]
    public EditRequest Model { get; set; } = new EditRequest();

    public SelectList Divisions { get; set; } = new SelectList(Globals.Brand.Divisions, "Key", "Value");

    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        if (id == null)
        {
            return Page();
        }

        var result = await FeatureRunner.Run(async () => await mediator.Send(new ViewRequest { Id = id.Value }));
        if (result.Incident != null)
        {
            ModelState.AddModelError("Model", result.Incident.Description);
            return Page();
        }

        Model = result.Data!;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("Model", IncidentCode.General.GetDescription());
            return Page();
        }

        var result = await FeatureRunner.Run(async () => await mediator.Send(Model));
        if (result.Incident != null)
        {
            ModelState.AddModelError("Model", result.Incident.Description);
            return Page();
        }

        return RedirectToPage(@"/Articles/Edit", new { id = result.Data!.Id });
    }
}
