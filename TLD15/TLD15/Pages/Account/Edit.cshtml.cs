using ACherryPie.Pages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TLD15.Pages.Account;

[Authorize]
public sealed class EditModel(IMediator mediator) : PageModel, IPageAdmin
{
    public List<AFeatureAccount.ResponsePreview> Data { get; set; } = [];

    public static MetaData MetaData => new()
    {
        Id = "Accounts",
        LocalUrl = "/Account/Edit"
    };

    public async Task OnGet()
    {
        Data = await mediator.Send(new AFeatureAccount.RequestPreview {  });
    }

    public async Task<IActionResult> OnGetItem(Guid id)
    {
        var result = await mediator.Send(new AFeatureAccount.RequestView { Id = id });
        var item = result[0];

        return Partial("_TableRow", item);
    }
    public async Task<IActionResult> OnGetEditItem(Guid id)
    {
        var result = await mediator.Send(new AFeatureAccount.RequestView { Id = id });
        var item = result[0];
        return Partial("_TableRowForm", item);
    }

    public async Task<IActionResult> OnPutItem(AFeatureAccount.RequestEdit contact)
    {
        await mediator.Send(contact);

        return Partial("_TableRow", contact);
    }

    public async Task<IActionResult> OnDeleteItem(Guid id)
    {
        await mediator.Send(new AFeatureAccount.RequestDelete { Id = id });

        return new OkResult();
    }

    public async Task<IActionResult> OnPostItem(AFeatureAccount.RequestEdit contact)
    {
        await mediator.Send(contact);

        return new OkResult();
    }

    public IActionResult OnGetModal()
    {
        return Partial("_ModalPartial", new AFeatureAccount.RequestEdit()
        {
            Id = null,
            Login = "NEW",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            Version = 0
        });
    }
}
