using ApplePie.Pages;
using Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using TLD15.Composition;

namespace TLD15.Pages;

public class IndexModel(
      IConfiguration configuration
    , DataContextBusiness contextBusiness
    ) : PageModel, IPagePublic
{
    public static MetaPage Meta => new()
    {
        Id = "index",
        Title = Globals.Brand.Company,
        LocalUrl = "/Index",
    };

    public string Host => configuration.GetSection(Globals.Configuration.ApplicationHost).Value!;

    public DataContextBusiness ContextBusiness => contextBusiness;
}
