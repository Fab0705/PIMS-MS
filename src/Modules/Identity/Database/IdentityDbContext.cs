using Microsoft.EntityFrameworkCore;
using PIMS_MS.Api.Modules.Identity.Domain.Entities;

namespace PIMS_MS.Modules.Identity.Database;

public sealed class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
        modelBuilder.HasDefaultSchema("identity");
    }
}