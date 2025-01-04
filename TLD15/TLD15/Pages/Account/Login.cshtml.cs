using ACherryPie.Feature;
using ACherryPie.Incidents;
using ACherryPie.Responses;
using ACherryPie.Security;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TLD15.Features.Accounts;

namespace TLD15.Pages.Account;

public class LoginFeature(IMediator mediator): PageModel
{
    public static string PageName => "Login";

    public sealed class LoginRequest : IRequest<ResponseCreated<Guid>>
    {
        [BindProperty]
        public string Login { get; set; } = string.Empty;

        [BindProperty, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [TempData]
        public string ErrorMessage { get; set; } = string.Empty;

        public string ReturnUrl { get; set; } = string.Empty;
    }

    public sealed class LoginHandler(
        IHashingService hashingService,
        IMongoDatabase database) : IRequestHandler<LoginRequest, ResponseCreated<Guid>>
    {
        public async Task<ResponseCreated<Guid>> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            await Task.Delay(new Random().Next(0, 500), cancellationToken);

            var collection = database.GetCollection<EntityAccount>(EntityAccount.Collection, new MongoCollectionSettings
            {
                ReadEncoding = new UTF8Encoding(),
            });
            var document = await collection.Find(x => x.Login == request.Login)
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new IncidentException(IncidentCode.LoginFailed);

            var incode = hashingService.Hash(request.Password, document.Salt);
            if (incode.HexHash != document.Password)
            {
                throw new IncidentException(IncidentCode.LoginFailed);
            }

            return new ResponseCreated<Guid>
            {
                Id = document.Id,
            };
        }
    }

    [BindProperty]
    public LoginRequest Model { get; set; } = new LoginRequest();

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
                await mediator.Send(new AccountInit.Request { Login = Model.Login, Password = Model.Password });
                await mediator.Send(new LoginRequest { Login = Model.Login, Password = Model.Password });

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
