using Infrastructure.Composition;
using Infrastructure.Models.Business;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;
/// <summary> Database context for identity-related operations. </summary>
public sealed class DataContextBusiness(DbContextOptions<DataContextBusiness> options) : DbContext(options)
{
    public DbSet<Content> Contents => Set<Content>();
    public DbSet<ContentTranslation> ContentTranslations => Set<ContentTranslation>();

    public DbSet<Article> Articles => Set<Article>();
    public DbSet<ArticleTranslation> ArticleTranslations => Set<ArticleTranslation>();

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectTranslation> ProjectTranslations => Set<ProjectTranslation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName.Business);
    }
}
