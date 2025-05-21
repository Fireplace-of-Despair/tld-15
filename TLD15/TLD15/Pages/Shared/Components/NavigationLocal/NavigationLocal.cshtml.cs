using ApplePie.Pages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace TLD15.Pages.Shared.Components.NavigationLocal;

public class NavigationLocal : ViewComponent
{
    public List<MetaPartial> Metas { get; set; } = [];

    public IViewComponentResult Invoke(List<MetaPartial> metas)
    {
        return View("NavigationLocal", new NavigationLocal
        {
            Metas = metas,
        });
    }
}
