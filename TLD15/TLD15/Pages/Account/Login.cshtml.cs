using ACherryPie.Feature;
using ACherryPie.Pages;
using Common.Composition;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TLD15.Pages.Account;

public class LoginModel(
      IMediator mediator
    , IConfiguration configuration) : PageModel, IPagePublic
{
    public static MetaPagePublic MetaPublic => new()
    {
        Id = "login",
        Title = "Login",
        LocalUrl = "/Account/Login",
    };

    public string Host => configuration.GetSection(Globals.Configuration.ApplicationHost).Value!;

    [BindProperty]
    public AFeatureAccount.RequestLogin Model { get; set; } = new AFeatureAccount.RequestLogin();

    public IActionResult OnGet(string? returnUrl = null)
    {
        if(User?.Identity?.IsAuthenticated == true)
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
        returnUrl ??= Url.Content("~/");

        if (ModelState.IsValid)
        {
            var result = await FeatureRunner.Run(async() =>
            {
                await mediator.Send(new AFeatureAccount.RequestEmptyLogin
                {
                    Login = Model.Login,
                    Password = Model.Password
                });

                await mediator.Send(Model);

                return true;
            });

            if (result.Incident != null)
            {
                ModelState.AddModelError("Model.Login", result.Incident.Description);
                return Page();
            }
            else
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, Model.Login)
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                return LocalRedirect(returnUrl);
            }
        }

        return Page();
    }
}
