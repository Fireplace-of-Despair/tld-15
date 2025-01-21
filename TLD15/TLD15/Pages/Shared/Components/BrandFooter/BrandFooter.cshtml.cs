using Microsoft.AspNetCore.Mvc;

namespace TLD15.Pages.Shared.Components.BrandFooter;

public class BrandFooter : ViewComponent
{
    public IViewComponentResult Invoke() => View(GetType().Name);
}
