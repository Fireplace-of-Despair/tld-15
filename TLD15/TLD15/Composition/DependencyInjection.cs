using ACherryPie.Security;
using Common.Composition;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TLD15.Composition;

public static class DependencyInjection
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddWebOptimizer(pipeline =>
        {
            pipeline.AddCssBundle("/css/site.css", "css/**/*css");
            pipeline.AddJavaScriptBundle("/js/site.js", "js/**/*.js");

            pipeline.MinifyCssFiles();
            pipeline.MinifyJsFiles();

            //TODO: add css bundle later
            pipeline.MinifyHtmlFiles();
        });

        builder.Services.AddMemoryCache();
        builder.Services.AddScoped<IHashingService, HashingService>();

        return builder;
    }

    public static async Task<WebApplicationBuilder> ConfigureStorage(this WebApplicationBuilder builder)
    {
        var connectionMongo = builder.Configuration.GetSection(Globals.Storage.Mongo).Get<string>()!;

        builder.Services.AddSingleton<IMongoClient>(new MongoClient(connectionMongo));

        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p =>
                typeof(IEntityStored).IsAssignableFrom(p)
                && p.IsClass);

        using (var client = new MongoClient(connectionMongo))
        {
            foreach (var type in types)
            {
                var method = type.GetMethod("CreateIndexesAsync", BindingFlags.Public | BindingFlags.Static);

                if (method != null)
                {
                    await (Task)method.Invoke(null, [client])!;
                }
                else
                {
                    throw new InvalidOperationException($"The type {type.FullName} does not have a static CreateIndexesAsync method.");
                }
            }
        }

        return builder;
    }

}
