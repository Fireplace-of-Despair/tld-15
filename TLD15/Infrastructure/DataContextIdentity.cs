using Infrastructure.Composition;
using Infrastructure.Models.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace Infrastructure;
/// <summary> Database context for identity-related operations. </summary>
public sealed class DataContextIdentity(DbContextOptions<DataContextIdentity> options) : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName.Identity);
    }
}
