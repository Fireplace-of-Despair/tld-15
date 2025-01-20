using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace TLD15.Pages.Shared.Components.NavigationLocal;

public class NavigationLocal : ViewComponent
{
    public Dictionary<string, string> Anchors { get; set; } = [];

    public IViewComponentResult Invoke(Dictionary<string, string> anchors)
    {
        Anchors = anchors;

        return View(GetType().Name, new NavigationLocal
        {
            Anchors = anchors
        });
    }
}
