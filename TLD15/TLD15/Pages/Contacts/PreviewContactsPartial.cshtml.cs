using ACherryPie.Feature;
using ACherryPie.Pages;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TLD15.Pages.Contacts;

public class PreviewContactsPartialModel : PageModel, IPartial
{
    public static string Anchor => "preview_contacts";

    public static async Task<PreviewContactsPartialModel> InitializeAsync(IMediator mediator)
    {
        var contacts = await FeatureRunner.Run(async () =>
        {
            return await mediator.Send(new AFeatureContacts.RequestPreview());
        });

        var result = new PreviewContactsPartialModel();

        if (contacts.Incident is not null)
        {
            result.ModelState.AddModelError("Contacts", contacts.Incident.Description);
        }
        else
        {
            result.Data = contacts.Data ?? [];
        }

        return result;
    }

    public List<AFeatureContacts.ResponsePreview> Data { get; set; } = [];
    public string Title => "Contacts";


    public void OnGet()
    {
        throw new NotSupportedException();
    }
}
