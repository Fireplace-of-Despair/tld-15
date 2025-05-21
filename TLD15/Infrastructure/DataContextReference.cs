using Infrastructure.Composition;
using Infrastructure.Models.Reference;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;
/// <summary> Data context for the reference data </summary>
public sealed class DataContextReference(DbContextOptions<DataContextReference> options) : DbContext(options)
{
    public DbSet<Language> Languages => Set<Language>();

    public DbSet<Division> Divisions => Set<Division>();
    public DbSet<DivisionTranslation> DivisionTranslations => Set<DivisionTranslation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName.Reference);

        modelBuilder.Entity<DivisionTranslation>()
            .HasIndex(at => new { at.DivisionId, at.LanguageId });
    }
}
