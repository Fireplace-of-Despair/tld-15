using ApplePie.Feature;
using ApplePie.Incidents;
using ApplePie.Pages;
using ApplePie.Security;
using Infrastructure;
using Infrastructure.Models.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using TLD15.Composition;

namespace TLD15.Pages.Accounts;

public class LoginModel(
      DataContextIdentity contextIdentity
    , IHashingService hashingService
    , IConfiguration configuration
    ) : PageModel, IPagePublic
{
    public static MetaPage Meta => new()
    {
        Id = "login",
        Title = "Login",
        LocalUrl = "/Accounts/Login",
    };

    public string Host => configuration.GetSection(Globals.Configuration.ApplicationHost).Value!;

    public sealed class RequestLogin
    {
        [BindProperty, MaxLength(280)]
        public string Login { get; set; } = string.Empty;

        [BindProperty, MaxLength(280), DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }

    [BindProperty]
    public RequestLogin Model { get; set; } = new RequestLogin();

    public IActionResult OnGet(string? returnUrl = null)
    {
        if (User?.Identity?.IsAuthenticated == true)
        {
            return LocalRedirect("~/");
        }

        if (!string.IsNullOrEmpty(Model.ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, Model.ErrorMessage);
        }

        returnUrl ??= Url.Content("~/");
        Model.ReturnUrl = returnUrl;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        Log.Warning("Login attempt for {Login}", Model.Login);

        returnUrl ??= Url.Content("~/");

        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("Model", ModelState.ErrorCount.ToString());
            return Page();
        }
        var result = await FeatureRunner.Run(async () =>
        {
            await OnEmptyDatabase(Model);
            await TryLogin(Model);

            return Task.CompletedTask;
        });

        if (result.Incident != null)
        {
            ModelState.AddModelError("Model", result.Incident.Value.GetDescription());
            return Page();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, Model.Login)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        return LocalRedirect(returnUrl);
    }


    //public void OnGetLogout()
    //{
    //    HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    //    HttpContext.Response.Cookies.Delete(Globals.Security.XSRFTOKEN);
    //}

    public async Task OnEmptyDatabase(RequestLogin request)
    {
        var saId = configuration.GetSection(Globals.Configuration.IdAdministrator).Get<Guid>();

        var isEmpty = !await contextIdentity.Accounts
            .AsNoTracking()
            .AnyAsync(x => x.Id == saId);

        if (isEmpty)
        {
            var hashResult = hashingService.Hash(request.Password);

            await contextIdentity.Accounts.AddAsync(new Account
            {
                Id = saId,
                Login = request.Login,
                Password = hashResult.HexHash,
                Salt = hashResult.HexSalt
            });
            await contextIdentity.SaveChangesAsync();
        }
    }

    public async Task TryLogin(RequestLogin request)
    {
        var account = await contextIdentity.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Login == request.Login)
            ?? throw new IncidentException(IncidentCode.LoginFailed);

        var incode = hashingService.Hash(request.Password, account.Salt);
        if (incode.HexHash != account.Password)
        {
            throw new IncidentException(IncidentCode.LoginFailed);
        }
    }

}
