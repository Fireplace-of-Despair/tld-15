using Microsoft.AspNetCore.Mvc;

namespace TLD15.Pages.Shared.Components.CommonScrollToTop;

public sealed class CommonScrollToTop() : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View(GetType().Name);
    }
}