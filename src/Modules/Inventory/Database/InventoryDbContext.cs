using Microsoft.EntityFrameworkCore;
using PIMS_MS.Common.Interfaces;
using PIMS_MS.Modules.Inventory.Domain.Entities;

namespace PIMS_MS.Modules.Inventory.Database;

public sealed class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
    {
    }

    public DbSet<SparePart> SpareParts => Set<SparePart>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Region> Regions => Set<Region>();
    public DbSet<Stock> Stocks => Set<Stock>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<WorkOrderItem> WorkOrderItems => Set<WorkOrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("inventory");

        ConfigureRegions(modelBuilder);
        ConfigureLocations(modelBuilder);
        ConfigureSpareParts(modelBuilder);
        ConfigureStocks(modelBuilder);
        ConfigureWorkOrders(modelBuilder);
    }

    private static void ConfigureRegions(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Region>(entity =>
        {
            entity.ToTable("Regions");
            entity.HasKey(r => r.Id);

            entity.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.HasIndex(r => r.Name)
                .IsUnique();

            entity.Property(r => r.Code)
                .IsRequired()
                .HasMaxLength(10);
            entity.HasIndex(r => r.Code)
                .IsUnique();

            // 🇵🇪 DATA SEEDING: Las 26 regiones administrativas del Perú
            entity.HasData(
                new Region(Guid.Parse("a0000000-0000-0000-0000-000000000001"), "Amazonas", "AMA"),
                new Region(Guid.Parse("a0000000-0000-0000-0000-000000000002"), "Áncash", "ANC"),
                new Region(Guid.Parse("a0000000-0000-0000-0000-000000000003"), "Apurímac", "APU"),
                new Region(Guid.Parse("a0000000-0000-0000-0000-000000000004"), "Arequipa", "ARE"),
                new Region(Guid.Parse("a0000000-0000-0000-0000-000000000005"), "Ayacucho", "AYA"),
                new Region(Guid.Parse("a0000000-0000-0000-0000-000000000006"), "Cajamarca", "CAJ"),
                new Region(Guid.Parse("a0000000-0000-0000-0000-000000000007"), "Callao", "CAL"),
                new Region(Guid.Parse("a0000000-0000-0000-0000-000000000008"), "Cusco", "CUS"),
                new Region(Guid.Parse("a0000000-0000-0000-0000-000000000009"), "Huancavelica", "HUV"),
                new Region(Guid.Parse("a0000000-0000-0000-0000-000000000010"), "Huánuco", "HUC"),
                new Region(Guid.Parse("a0000000-0000-0000-0000-000000000011"), "Ica", "ICA"),
                new Region(Guid.Parse("a0000000-0000-0000-0000-000000000012"), "Junín", "JUN"),
                new Region(Guid.Parse("a0000000-0000-0000-0000-000000000013"), "La Libertad", "LAL"),
                new Region(Guid.Parse("a0000000-0000-0000-0000-000000000014"), "Lambayeque", "LAM"),
                //new Region(Guid.Parse("a0000000-0000-0000-0000-000000000015"), "Lima Metropolitana", "LIM"),
                new Region(Guid.Parse("a0000000-0000-0000-0000-000000000016"), "Lima Provincias", "LIP"),
                new Region(Guid.Parse("a0000000-0000-0000-0000-000000000017"), "Loreto", "LOR"),
                new Region(Guid.Parse("a0000000-0000-0000-0000-000000000018"), "Madre de Dios", "MDD"),
                new Region(Guid.Parse("a0000000-0000-0000-0000-000000000019"), "Moquegua", "MOQ"),
                new Region(Guid.Parse("a0000000-0000-0000-0000-000000000020"), "Pasco", "PAS"),
                new Region(Guid.Parse("a0000000-0000-0000-0000-000000000021"), "Piura", "PIU"),
                new Region(Guid.Parse("a0000000-0000-0000-0000-000000000022"), "Puno", "PUN"),
                new Region(Guid.Parse("a0000000-0000-0000-0000-000000000023"), "San Martín", "SAM"),
                new Region(Guid.Parse("a0000000-0000-0000-0000-000000000024"), "Tacna", "TAC"),
                new Region(Guid.Parse("a0000000-0000-0000-0000-000000000025"), "Tumbes", "TUM"),
                new Region(Guid.Parse("a0000000-0000-0000-0000-000000000026"), "Ucayali", "UCA")
            );
        });
    }

    private static void ConfigureLocations(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Location>(entity =>
        {
            entity.ToTable("Locations");
            entity.HasKey(l => l.Id);

            entity.Property(l => l.Name)
                .IsRequired()
                .HasMaxLength(150);

            entity.HasOne(l => l.Region)
                .WithMany()
                .HasForeignKey(l => l.RegionId)
                .OnDelete(DeleteBehavior.Restrict); // Evita borrar regiones si tienen almacenes
        });
    }

    private static void ConfigureSpareParts(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SparePart>(entity =>
        {
            entity.ToTable("SpareParts");
            entity.HasKey(sp => sp.Id);

            entity.Property(sp => sp.PartNumber)
                .IsRequired()
                .HasMaxLength(50);
            entity.HasIndex(sp => sp.PartNumber)
                .IsUnique();

            entity.Property(sp => sp.Description)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(sp => sp.IsRework)
                .IsRequired()
                .HasDefaultValue(false);

            // 🔐 Auditoría heredada de AggregateRoot
            entity.Property(sp => sp.CreatedAtUtc).IsRequired();
            entity.Property(sp => sp.CreatedBy).HasMaxLength(100).IsRequired(false);
            entity.Property(sp => sp.LastModifiedAtUtc).IsRequired(false);
            entity.Property(sp => sp.LastModifiedBy).HasMaxLength(100).IsRequired(false);
        });
    }

    private static void ConfigureStocks(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Stock>(entity =>
        {
            entity.ToTable("Stocks");
            entity.HasKey(s => s.Id);

            entity.Property(s => s.Quantity).IsRequired();

            // Un repuesto solo puede tener un registro de stock por cada ubicación física
            entity.HasIndex(s => new { s.SparePartId, s.LocationId }).IsUnique();

            entity.HasOne(s => s.SparePart)
                .WithMany()
                .HasForeignKey(s => s.SparePartId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(s => s.Location)
                .WithMany()
                .HasForeignKey(s => s.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Auditoría
            entity.Property(s => s.CreatedAtUtc).IsRequired();
            entity.Property(s => s.CreatedBy).HasMaxLength(100).IsRequired(false);
            entity.Property(s => s.LastModifiedAtUtc).IsRequired(false);
            entity.Property(s => s.LastModifiedBy).HasMaxLength(100).IsRequired(false);
        });
    }

    private static void ConfigureWorkOrders(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkOrder>(entity =>
        {
            entity.ToTable("WorkOrders");
            entity.HasKey(w => w.Id);

            entity.Property(w => w.WorkOrderNumber)
                .IsRequired()
                .HasMaxLength(50);
            entity.HasIndex(w => w.WorkOrderNumber)
                .IsUnique();

            entity.Property(w => w.Description)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(w => w.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            entity.HasOne(w => w.Location)
                .WithMany()
                .HasForeignKey(w => w.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Auditoría
            entity.Property(w => w.CreatedAtUtc).IsRequired();
            entity.Property(w => w.CreatedBy).HasMaxLength(100).IsRequired(false);
            entity.Property(w => w.LastModifiedAtUtc).IsRequired(false);
            entity.Property(w => w.LastModifiedBy).HasMaxLength(100).IsRequired(false);

            // 🧱 Relación DDD Padre-Hijo con acceso al campo privado '_items'
            entity.HasMany(w => w.Items)
                .WithOne()
                .HasForeignKey(wi => wi.WorkOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Metadata.FindNavigation(nameof(WorkOrder.Items))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<WorkOrderItem>(entity =>
        {
            entity.ToTable("WorkOrderItems");
            entity.HasKey(wi => wi.Id);

            entity.Property(wi => wi.Quantity).IsRequired();

            entity.HasOne(wi => wi.SparePart)
                .WithMany()
                .HasForeignKey(wi => wi.SparePartId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}