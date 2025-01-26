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

namespace TLD15.Pages.Lore;

[Authorize]
public class EditLoreModel(IMediator mediator,
    IConfiguration configuration,
    IWebHostEnvironment webHostEnvironment,
    IHashingService hashingService) : PageModel, IPageAdmin
{
    public readonly string ApplicationHost = configuration.GetSection(Globals.Settings.ApplicationHost).Value!;

    public static MetaData MetaData => new()
    {
        Id = "Lore",
        LocalUrl = "/Lore/Edit"
    };

    [BindProperty]
    public AFeatureLore.ResponseRead? Model { get; set; }

    public async Task OnGetAsync()
    {
        var model = await mediator.Send(new AFeatureLore.RequestRead());
        Model = model ?? new AFeatureLore.ResponseRead
            {
                Content = string.Empty,
                Id = null,
                ContentHtml = string.Empty,
                CreatedAt = DateTime.UtcNow,
                PosterAlt = string.Empty,
                PosterUrl = string.Empty,
                UpdatedAt = DateTime.Now,
                Version = 0
            };
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("Model", IncidentCode.General.GetDescription());
            return Page();
        }

        var result = await FeatureRunner.Run(async () => await mediator.Send(Model!));
        if (result.Incident != null)
        {
            ModelState.AddModelError("Model", result.Incident.Description);
            return Page();
        }

        return RedirectToPage("/Lore/Edit");
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
