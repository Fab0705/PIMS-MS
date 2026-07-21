using Microsoft.EntityFrameworkCore;

namespace PIMS_MS.Modules.FieldService.Database;

public sealed class FieldServiceDbContext : DbContext
{
    public FieldServiceDbContext(DbContextOptions<FieldServiceDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //modelBuilder.ApplyConfigurationsFromAssembly(typeof(FieldServiceDbContext).Assembly);
        modelBuilder.HasDefaultSchema("fieldservice");
    }
}