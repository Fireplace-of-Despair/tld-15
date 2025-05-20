using ApplePie.Security;
using Infrastructure;
using Infrastructure.Migrations;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.IO;

namespace TLD15.Composition;

public static class DependencyInjection
{
    public static void ConfigureLogging()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
    }

    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        //builder.Services.AddWebOptimizer(pipeline =>
        //{
        //    pipeline.MinifyCssFiles();
        //    pipeline.MinifyJsFiles();
        //    pipeline.MinifyHtmlFiles();
        //});

        builder.Services.AddMemoryCache();
        builder.Services.AddScoped<IHashingService, HashingService>();

        return builder;
    }

    public static WebApplicationBuilder ConfigureStorage(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetSection(Globals.Configuration.Database).Get<string>()!;

        builder.Services.AddDbContextPool<DataContextIdentity>(options => options.UseNpgsql(connectionString + ";MaxPoolSize=1"));
        builder.Services.AddDbContextPool<DataContextReference>(options => options.UseNpgsql(connectionString + ";MaxPoolSize=1"));
        builder.Services.AddDbContextPool<DataContextBusiness>(options => options.UseNpgsql(connectionString + ";MaxPoolSize=1"));

        Migrator.Up(connectionString);
        return builder;
    }
}
