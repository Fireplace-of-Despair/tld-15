using FluentMigrator.Runner;
using Infrastructure.Composition;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Migrations;

public static class Migrator
{
    /// <summary> Migrate up to the last version </summary>
    /// <param name="connectionString"> Database connection string </param>
    public static void Up(string connectionString)
    {
        using (var scope = CreateServices(connectionString).CreateScope())
        {
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

            runner.ListMigrations();
            runner.MigrateUp();
        }
    }

    /// <summary> Migrate down to the given version </summary>
    /// <param name="connectionString"> Database connection string </param>
    /// <param name="version"> The version to migrate down to </param>
    public static void Down(string connectionString, long version)
    {
        using (var scope = CreateServices(connectionString).CreateScope())
        {
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

            runner.ListMigrations();
            runner.MigrateDown(version);
        }
    }

    /// <summary>
    /// Creates a service provider for the migration runner
    /// </summary>
    /// <param name="connectionString"> Database connection string </param>
    /// <returns> </returns>
    private static ServiceProvider CreateServices(string connectionString)
    {
        return new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(runner => runner
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .WithVersionTable(new VersionTable())
                .ScanIn(typeof(Migrator).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .BuildServiceProvider(false);
    }
}
