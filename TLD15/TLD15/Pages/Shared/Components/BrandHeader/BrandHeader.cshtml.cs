using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TLD15.Pages.Shared.Components.BrandHeader;

public class BrandHeader : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View(GetType().Name);
    }
}
