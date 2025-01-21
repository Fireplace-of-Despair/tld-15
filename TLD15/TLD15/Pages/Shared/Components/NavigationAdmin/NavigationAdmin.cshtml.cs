using ACherryPie.Pages;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TLD15.Pages.Shared.Components.NavigationAdmin;

public class NavigationAdmin : ViewComponent
{
    public Dictionary<string, string> Urls { get; set; } = [];

    public NavigationAdmin()
    {
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => typeof(IPageAdmin).IsAssignableFrom(x) && x.IsClass);

        foreach (var item in types)
        {
            var metaData =
            item.GetProperty(nameof(IPageAdmin.MetaData), BindingFlags.Public | BindingFlags.Static)?
                    .GetValue(null) as MetaData;

            Urls.Add(metaData.Id, metaData.LocalUrl);
        }
    }

    public IViewComponentResult Invoke()
    {
        return View(GetType().Name, new NavigationAdmin());
    }
}
