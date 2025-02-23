using ACherryPie.Feature;
using ACherryPie.Incidents;
using ACherryPie.Pages;
using ACherryPie.Security;
using Common.Composition;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TLD15.Pages.Press;

[Authorize]
public class EditModel(IMediator mediator,
    IWebHostEnvironment webHostEnvironment,
    IHashingService hashingService,
    IConfiguration configuration) : PageModel
{
    public static string FeatureName => "Edit Press";
    public readonly string ApplicationHost = configuration.GetSection(Globals.Settings.ApplicationHost).Value!;

    public static MetaData MetaData => new()
    {
        Id = "Press",
        LocalUrl = "/press/edit"
    };

    [BindProperty]
    public AFeaturePress.ResponseRead Model { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        if (id == null)
        {
            return Page();
        }

        var result = await FeatureRunner.Run(async () => await mediator.Send(new AFeaturePress.RequestRead { Id = id.Value }));
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

        return RedirectToPage(@"/Press/Edit", new { id = result.Data!.Id });
    }

    public async Task<IActionResult> OnPostImages(IList<IFormFile> files)
    {
        List<string> urls = [];

        foreach (var file in files)
        {
            var hash = hashingService.Hash(file);
            var extension = Path.GetExtension(file.FileName);
            var filePath = Path.Combine(webHostEnvironment.WebRootPath, "files", hash + extension);
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            urls.Add($"{ApplicationHost}files/{hash}{extension}");
        }

        return new ObjectResult(urls);
    }
}
