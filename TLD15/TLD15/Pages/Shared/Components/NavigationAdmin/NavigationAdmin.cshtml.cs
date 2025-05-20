using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace TLD15.Pages.Shared.Components.NavigationAdmin;

public class NavigationAdmin : ViewComponent
{
    public Dictionary<string, string> Urls { get; set; } = [];

    public IViewComponentResult Invoke()
    {
        return View(GetType().Name, new NavigationAdmin());
    }
}