using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using TLD15.Composition;
using TLD15.Pages.Accounts;
using WebMarkupMin.AspNetCoreLatest;

namespace TLD15;

public static class Program
{
    public static async Task Main(string[] args)
    {
        DependencyInjection.ConfigureLogging();

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAntiforgery(o => o.HeaderName = Globals.Security.XSRFTOKEN);
        builder.Services.AddAuthentication
        (
            CookieAuthenticationDefaults.AuthenticationScheme
        ).AddCookie();

        builder.Services.AddAuthorization();

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddControllersWithViews();
        builder.Services.AddRouting(options =>
        {
            options.LowercaseUrls = true;
            options.LowercaseQueryStrings = true;
        });

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.Name = "tld15";

            options.LoginPath = LoginModel.Meta.LocalUrl;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
            options.SlidingExpiration = true;
            options.LogoutPath = "/Logout";
        });
        builder.Services.AddResponseCompression();

        builder.ConfigureStorage();
        builder.ConfigureServices();
        builder.ConfigureOptimizations();

        var app = builder.Build();

        app.UseResponseCompression();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseWebMarkupMin();

        app.UseStaticFiles();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "RssController",
            pattern: "{controller=RssController}/{action=rss}");

        app.MapControllerRoute(
            name: "PingController",
            pattern: "{controller=PingController}/{action=ping}");

        app.MapRazorPages();

        await app.RunAsync();
    }
}
