using ApplePie.Pages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace TLD15.Pages.Shared.Components.NavigationLocal;

public class NavigationLocal : ViewComponent
{
    public List<MetaPage> Metas { get; set; } = [];

    public IViewComponentResult Invoke(List<MetaPage> metas)
    {
        return View(GetType().Name, new NavigationLocal
        {
            Metas = metas,
        });
    }
}
