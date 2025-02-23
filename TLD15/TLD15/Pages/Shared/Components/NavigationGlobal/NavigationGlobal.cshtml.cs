using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace TLD15.Pages.Shared.Components.NavigationGlobal;

public class NavigationGlobal : ViewComponent
{
    public Dictionary<string, string> Links { get; set; } = [];

    public IViewComponentResult Invoke(Dictionary<string, string> links)
    {
        return View(GetType().Name, new NavigationGlobal
        {
            Links = links
        });
    }
}
