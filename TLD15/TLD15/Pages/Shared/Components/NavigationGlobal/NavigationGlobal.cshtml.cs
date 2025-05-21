using Microsoft.AspNetCore.Mvc;

namespace TLD15.Pages.Shared.Components.NavigationGlobal;

public class NavigationGlobal : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View("NavigationGlobal", new NavigationGlobal());
    }
}
