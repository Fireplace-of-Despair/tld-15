using Microsoft.AspNetCore.Mvc;

namespace TLD15.Pages.Shared.Components.BrandHeader;

public sealed class BrandHeader : ViewComponent
{
    public IViewComponentResult Invoke() => View(GetType().Name);
}
