using FluentMigrator.Runner;
using Infrastructure.Composition;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;

namespace Infrastructure.Migrations;

public static class Migrator
{
    private static readonly Lock _lock = new();
    private static ServiceProvider? _serviceProvider;
    private static string? _currentConnectionString;

    /// <summary> Migrate up to the last version </summary>
    /// <param name="connectionString"> Database connection string </param>
    public static void Up(string connectionString)
    {
        using var scope = GetOrCreateServiceProvider(connectionString).CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }

    /// <summary> Migrate down to the given version </summary>
    /// <param name="connectionString"> Database connection string </param>
    /// <param name="version"> The version to migrate down to </param>
    public static void Down(string connectionString, long version)
    {
        using var scope = GetOrCreateServiceProvider(connectionString).CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateDown(version);
    }

    /// <summary>
    /// Creates or reuses a service provider for the migration runner
    /// </summary>
    /// <param name="connectionString"> Database connection string </param>
    private static ServiceProvider GetOrCreateServiceProvider(string connectionString)
    {
        lock (_lock)
        {
            if (_serviceProvider == null || _currentConnectionString != connectionString)
            {
                _serviceProvider?.Dispose();

                _serviceProvider = new ServiceCollection()
                    .AddFluentMigratorCore()
                    .ConfigureRunner(runner => runner
                        .AddPostgres()
                        .WithGlobalConnectionString(connectionString)
                        .WithVersionTable(new VersionTable())
                        .ScanIn(typeof(Migrator).Assembly).For.Migrations())
                    .AddLogging(lb => lb.AddFluentMigratorConsole())
                    .BuildServiceProvider(validateScopes: false);
                _currentConnectionString = connectionString;
            }

            return _serviceProvider;
        }
    }
}
