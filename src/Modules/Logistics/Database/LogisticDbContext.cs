using Microsoft.EntityFrameworkCore;

namespace PIMS_MS.Modules.Logistics.Database;

public sealed class LogisticDbContext : DbContext
{
    public LogisticDbContext(DbContextOptions<LogisticDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //modelBuilder.ApplyConfigurationsFromAssembly(typeof(LogisticDbContext).Assembly);
        modelBuilder.HasDefaultSchema("logistics");
    }
}