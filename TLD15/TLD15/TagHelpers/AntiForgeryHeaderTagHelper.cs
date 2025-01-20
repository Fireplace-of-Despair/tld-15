using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TLD15.TagHelpers;

[HtmlTargetElement("input", Attributes = AntiForgeryAttributeName)]
[HtmlTargetElement("button", Attributes = AntiForgeryAttributeName)]
public class AntiForgeryHeaderTagHelper(IAntiforgery antiforgery) : TagHelper
{
    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; }

    [HtmlAttributeName(AntiForgeryAttributeName)]
    public bool AntiForgery { get; set; }

    private const string AntiForgeryAttributeName = "antiforgery";

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (AntiForgery)
        {
            //X-Requested-With XMLHttpRequest
            
            var token = antiforgery.GetAndStoreTokens(ViewContext.HttpContext).RequestToken;
            var currentHeaderValue = output.Attributes["hx-headers"]?.Value.ToString();
            var newHeaderValue = $"\"XSRF-TOKEN\": \"{token}\"";

            if (string.IsNullOrEmpty(currentHeaderValue))
            {
                output.Attributes.SetAttribute("hx-headers", newHeaderValue);
            }
            else
            {
                // Append the anti-forgery token to existing hx-headers
                newHeaderValue = $"{currentHeaderValue}, {newHeaderValue}";
                output.Attributes.SetAttribute("hx-headers", newHeaderValue);
            }
        }
    }
}