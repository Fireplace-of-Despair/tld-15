using ACherryPie.Security;
using Common.Composition;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;

namespace TLD15.Composition;

public static class DependencyInjection
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder, string connectionString)
    {
        builder.Services.AddScoped<IHashingService, HashingService>();

        return builder;
    }

    public static async Task<WebApplicationBuilder> ConfigureStorage(this WebApplicationBuilder builder, string connectionString)
    {
        builder.Services.AddSingleton<IMongoClient>(new MongoClient(connectionString));
        builder.Services.AddScoped(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(Globals.Storage.Name));

        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => 
                typeof(IEntityStored).IsAssignableFrom(p)
                && p.IsClass);

        using (var client = new MongoClient(connectionString))
        {
            var database = client.GetDatabase(Globals.Storage.Name);

            foreach (var type in types)
            {
                var method = type.GetMethod("CreateIndexesAsync", BindingFlags.Public | BindingFlags.Static);

                if (method != null)
                {
                    await (Task)method.Invoke(null, [database])!;
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
