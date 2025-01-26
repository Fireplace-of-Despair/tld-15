using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Threading.Tasks;
using TLD15.Composition;

namespace TLD15;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
        builder.Services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");
        builder.Services.AddAuthentication(
            CookieAuthenticationDefaults.AuthenticationScheme
            ).AddCookie();

        builder.Services.AddAuthorization();

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddRouting(options =>
        {
            options.LowercaseUrls = true;
        });
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Login";
            options.LogoutPath = "/Logout";
        });


        builder.ConfigureServices();
        await builder.ConfigureStorage();

        var app = builder.Build();
        app.UseWebOptimizer();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();

        await app.RunAsync();
    }
}
