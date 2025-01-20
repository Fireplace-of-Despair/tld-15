using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace TLD15.TagHelpers;

public static class UrlHandlerExtensions
{
    public static string Handler(this IUrlHelper urlHelper, string handler, object? values = null)
    {
        // Convert the values object to a dictionary
        var routeValues = new RouteValueDictionary(values)
        {
            ["handler"] = handler // Add the handler to the dictionary
        };

        return urlHelper.RouteUrl(new UrlRouteContext
        {
            Values = routeValues
        })!;
    }
}