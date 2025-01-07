using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace TLD15.Pages.Shared.Components.Contacts;

public sealed class Contacts : ViewComponent
{
    public sealed class Model
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public Dictionary<string, string> Contacts { get; set; } = [];
    }

    public IViewComponentResult Invoke(Model model)
    {
        return View(GetType().Name, model);
    }
}
