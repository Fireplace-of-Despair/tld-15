using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace TLD15.TagHelpers;

/// <summary> Extension methods for generating URLs with a specific handler </summary>
public static class UrlHandlerExtensions
{
    /// <summary> Generates a URL for a specific handler with optional route values </summary>
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