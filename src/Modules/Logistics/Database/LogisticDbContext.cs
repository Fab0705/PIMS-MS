using Microsoft.EntityFrameworkCore;
using PIMS_MS.Modules.Logistics.Domain.Entities;

namespace PIMS_MS.Modules.Logistics.Database;

public sealed class LogisticDbContext : DbContext
{
    public LogisticDbContext(DbContextOptions<LogisticDbContext> options) : base(options)
    {
    }

    public DbSet<Transfer> Transfers => Set<Transfer>();
    public DbSet<Replenishment> Replenishments => Set<Replenishment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //modelBuilder.ApplyConfigurationsFromAssembly(typeof(LogisticDbContext).Assembly);
        modelBuilder.HasDefaultSchema("logistics");

        ConfigureTransfers(modelBuilder);
        ConfigureReplenishments(modelBuilder);
    }
    private static void ConfigureTransfers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transfer>(entity =>
        {
            entity.ToTable("Transfers");
            entity.HasKey(t => t.Id);

            // Regla de negocio: El TrackingCode es una clave natural, obligatoria y absolutamente ÚNICA
            entity.Property(t => t.TrackingCode)
                .IsRequired()
                .HasMaxLength(50);
            entity.HasIndex(t => t.TrackingCode)
                .IsUnique();

            // Referencias externas a otros módulos: Solo indexamos para velocidad, ¡SIN Foreign Keys cruzadas!
            entity.Property(t => t.OriginLocationId).IsRequired();
            entity.Property(t => t.DestinationLocationId).IsRequired();
            entity.HasIndex(t => t.OriginLocationId);
            entity.HasIndex(t => t.DestinationLocationId);

            // Convertimos el Enum a String en la base de datos
            entity.Property(t => t.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(t => t.ExceptionNotes)
                .HasMaxLength(1000)
                .IsRequired(false);

            // 🔐 Auditoría: Mapeo de las propiedades heredadas de IAuditableEntity / AggregateRoot
            entity.Property(t => t.CreatedAtUtc).IsRequired();
            entity.Property(t => t.CreatedBy).HasMaxLength(100).IsRequired(false);
            entity.Property(t => t.LastModifiedAtUtc).IsRequired(false);
            entity.Property(t => t.LastModifiedBy).HasMaxLength(100).IsRequired(false);

            // 🧱 Relación DDD Padre-Hijo (Transfer -> TransferItems)
            entity.HasMany(t => t.Items)
                .WithOne()
                .HasForeignKey(i => i.TransferId)
                .OnDelete(DeleteBehavior.Cascade); // Si se elimina la guía, se eliminan sus ítems

            // ⚠️ MAGIA DDD: Le decimos a EF Core que use el campo privado '_items' para hidratar la lista
            entity.Metadata.FindNavigation(nameof(Transfer.Items))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<TransferItem>(entity =>
        {
            entity.ToTable("TransferItems");
            entity.HasKey(i => i.Id);

            entity.Property(i => i.TransferId).IsRequired();
            
            // Referencia externa a la entidad SparePart del módulo Inventory (Solo Index)
            entity.Property(i => i.SparePartId).IsRequired();
            entity.HasIndex(i => i.SparePartId);

            entity.Property(i => i.Quantity).IsRequired();
        });
    }

    private static void ConfigureReplenishments(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Replenishment>(entity =>
        {
            entity.ToTable("Replenishments");
            entity.HasKey(r => r.Id);

            // Referencia externa al almacén provincial que solicita el pedido
            entity.Property(r => r.LocationId).IsRequired();
            entity.HasIndex(r => r.LocationId);

            entity.Property(r => r.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(r => r.RejectionReason)
                .HasMaxLength(500)
                .IsRequired(false);

            // Auditoría
            entity.Property(r => r.CreatedAtUtc).IsRequired();
            entity.Property(r => r.CreatedBy).HasMaxLength(100).IsRequired(false);
            entity.Property(r => r.LastModifiedAtUtc).IsRequired(false);
            entity.Property(r => r.LastModifiedBy).HasMaxLength(100).IsRequired(false);

            // 🧱 Relación DDD Padre-Hijo (Replenishment -> ReplenishmentItems)
            entity.HasMany(r => r.Items)
                .WithOne()
                .HasForeignKey(i => i.ReplenishmentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Metadata.FindNavigation(nameof(Replenishment.Items))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<ReplenishmentItem>(entity =>
        {
            entity.ToTable("ReplenishmentItems");
            entity.HasKey(i => i.Id);

            entity.Property(i => i.ReplenishmentId).IsRequired();
            
            // Referencia externa al repuesto (Inventory Module)
            entity.Property(i => i.SparePartId).IsRequired();
            entity.HasIndex(i => i.SparePartId);

            entity.Property(i => i.Quantity).IsRequired();
        });
    }
}