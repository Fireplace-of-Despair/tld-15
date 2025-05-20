using ApplePie.Incidents;
using ApplePie.Pages;
using ApplePie.Security;
using Infrastructure;
using Infrastructure.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TLD15.Composition;

namespace TLD15.Pages.Accounts;

[Authorize]
public sealed class EditModel(
    DataContextIdentity contextIdentity
    , IHashingService hashingService
    , IConfiguration configuration) : PageModel, IPagePrivate
{
    public List<RequestEdit> Data { get; set; } = [];

    public static MetaPage Meta => new()
    {
        Id = "Accounts",
        LocalUrl = "/accounts/edit",
        Title = "Accounts",
    };

    public string Host => configuration.GetSection(Globals.Configuration.ApplicationHost).Value!;

    public sealed class RequestEdit
    {
        public required Guid? Id { get; set; }
        public required string Login { get; set; }
        public string Password { get; set; } = string.Empty;

        public required DateTimeOffset CreatedAt { get; set; }
        public required DateTimeOffset UpdatedAt { get; set; }
        public required long Version { get; set; }
    }


    public async Task OnGet()
    {
        Data = await contextIdentity.Accounts
            .AsNoTracking()
            .Select(x => new RequestEdit
            {
                Id = x.Id,
                Login = x.Login,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Version = x.Version
            }).ToListAsync();
    }

    public async Task<IActionResult> OnGetItem(Guid id)
    {
        var result = await contextIdentity.Accounts
            .AsNoTracking()
            .Select(x => new RequestEdit
            {
                Id = x.Id,
                Login = x.Login,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Version = x.Version
            })
            .Where(x => x.Id == id)
            .FirstAsync();

        return Partial("_TableRow", result);
    }

    public async Task<IActionResult> OnGetEditItem(Guid id)
    {
        var result = await contextIdentity.Accounts
            .AsNoTracking()
            .Select(x => new RequestEdit
            {
                Id = x.Id,
                Login = x.Login,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Version = x.Version
            })
            .Where(x => x.Id == id)
            .FirstAsync();

        return Partial("_TableRowForm", result);
    }

    public async Task<IActionResult> OnPutItem(RequestEdit request)
    {
        var result = await contextIdentity.Accounts
            .FirstAsync(x => x.Id == request.Id)
            ?? throw new IncidentException(IncidentCode.NotFound);

        result.Login = request.Login;
        if (!string.IsNullOrEmpty(request.Password))
        {
            var hashResult = hashingService.Hash(request.Password);
            result.Password = hashResult.HexHash;
            result.Salt = hashResult.HexSalt;
        }
        await contextIdentity.SaveChangesAsync();

        return Partial("_TableRow", request);
    }

    public async Task<IActionResult> OnDeleteItem(Guid id)
    {
        var employer = new Account { Id = id };
        contextIdentity.Accounts.Attach(employer);
        contextIdentity.Accounts.Remove(employer);
        await contextIdentity.SaveChangesAsync();

        return new OkResult();
    }

    public async Task<IActionResult> OnPostItem(RequestEdit request)
    {
        var item = new Account
        {
            Id = Guid.NewGuid(),
            Login = request.Login
        };
        if (!string.IsNullOrEmpty(request.Password))
        {
            var hashResult = hashingService.Hash(request.Password);
            item.Password = hashResult.HexHash;
            item.Salt = hashResult.HexSalt;
        }

        await contextIdentity.Accounts.AddAsync(item);
        await contextIdentity.SaveChangesAsync();

        return new OkResult();
    }

    public IActionResult OnGetModal()
    {
        return Partial("_ModalPartial", new RequestEdit()
        {
            Id = null,
            Login = "NEW",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            Version = 0
        });
    }
}
