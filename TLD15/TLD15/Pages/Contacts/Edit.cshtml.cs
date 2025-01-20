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

namespace TLD15.Pages.Contacts;

[Authorize]
public class EditModel(IMediator mediator) : PageModel, IPageAdmin
{
    public List<AFeatureContacts.EditRequest> Data { get; set; } = [];

    public static MetaData MetaData => new()
    {
        Id = "Contacts",
        LocalUrl = "/Contacts/Edit"
    };

    public async Task OnGet()
    {
        Data = await mediator.Send(new AFeatureContacts.ViewRequest { Id = null });
    }

    public async Task<IActionResult> OnGetItem(Guid id)
    {
        var result = await mediator.Send(new AFeatureContacts.ViewRequest { Id = id });
        var item = result[0];
        
        return Partial("_TableRow", item);
    }
    public async Task<IActionResult> OnGetEditItem(Guid id)
    {
        var result = await mediator.Send(new AFeatureContacts.ViewRequest { Id = id });
        var item = result[0];
        return Partial("_TableRowForm", item);
    }

    public async Task<IActionResult> OnPutItem(AFeatureContacts.EditRequest contact)
    {
        await mediator.Send(contact);

        return Partial("_TableRow", contact);
    }

    public async Task<IActionResult> OnDeleteItem(Guid id)
    {
        await mediator.Send(new AFeatureContacts.DeleteRequest { Id = id } );

        return new OkResult();
    }

    public async Task<IActionResult> OnPostItem(AFeatureContacts.EditRequest contact)
    {
        await mediator.Send(contact);

        return new OkResult();
    }

    public IActionResult OnGetModal()
    {
        return Partial("_ModalPartial", new AFeatureContacts.EditRequest()
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
