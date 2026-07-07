using Microsoft.EntityFrameworkCore;

namespace PIMS_MS.Api.Modules.Logistic.Database;

public sealed class LogisticDbContext : DbContext
{
    public LogisticDbContext(DbContextOptions<LogisticDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //modelBuilder.ApplyConfigurationsFromAssembly(typeof(LogisticDbContext).Assembly);
        modelBuilder.HasDefaultSchema("logistic");
    }
}