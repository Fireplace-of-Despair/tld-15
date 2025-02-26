using ACherryPie.Pages;
using Common.Composition;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TLD15.Pages.Account;

[Authorize]
public sealed class EditModel(
    IMediator mediator,
    IConfiguration configuration) : PageModel, IPageAdmin
{
    public readonly string ApplicationHost = configuration.GetSection(Globals.Configuration.ApplicationHost).Value!;
    public List<AFeatureAccount.RequestEdit> Data { get; set; } = [];

    public static MetaData MetaData => new()
    {
        Id = "Accounts",
        LocalUrl = "/account/edit"
    };

    public async Task OnGet()
    {
        Data = await mediator.Send(new AFeatureAccount.RequestView { Id = null  });
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
