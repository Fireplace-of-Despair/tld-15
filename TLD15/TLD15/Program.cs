using Common.Composition;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using TLD15.Composition;

namespace TLD15;

public static class Program
{
    const string StorageName = "Storage:Files:Path";
    const string StorageRequest = "Application:HostFiles";

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

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Login";
            options.LogoutPath = "/Logout";
        });

        var connectionString = builder.Configuration.GetSection(Globals.Storage.ConnectionString).Get<string>()!;
        builder.ConfigureServices(connectionString);
        await builder.ConfigureStorage(connectionString);

        var app = builder.Build();

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


        //Migrator.Up(connectionString);

        await app.RunAsync();
    }
}
