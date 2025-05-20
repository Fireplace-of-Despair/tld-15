using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace TLD15.Pages.Shared.Components.NavigationGlobal;

public class NavigationGlobal : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View(GetType().Name, new NavigationGlobal
        {
        });
    }
}
