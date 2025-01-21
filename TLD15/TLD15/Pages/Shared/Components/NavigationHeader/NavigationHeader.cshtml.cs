using Microsoft.AspNetCore.Mvc;

namespace TLD15.Pages.Shared.Components.NavigationHeader;

public class NavigationHeader : ViewComponent
{
    public IViewComponentResult Invoke() => View(GetType().Name);
}
