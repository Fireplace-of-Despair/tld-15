using ACherryPie.Feature;
using ACherryPie.Pages;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TLD15.Pages.Contacts;

public class PreviewContactsPartialModel : PageModel, IPartial
{
    public static MetaPartialPublic MetaPublic => new()
    {
        Id = "preview_contacts",
        Title = "Contacts",
    };

    public List<AFeatureContacts.ResponsePreview> Data { get; set; } = [];

    public static async Task<PreviewContactsPartialModel> InitializeAsync(IMediator mediator)
    {
        var contacts = await FeatureRunner.Run(async () => await mediator.Send(new AFeatureContacts.RequestPreview()));

        var model = new PreviewContactsPartialModel();

        if (contacts.Incident is not null)
        {
            model.ModelState.AddModelError(string.Empty, contacts.Incident.Description);
        }
        else
        {
            model.Data = contacts.Data ?? [];
        }

        return model;
    }
}
