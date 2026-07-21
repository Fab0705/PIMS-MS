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

        ConfigureUsers(modelBuilder);
    }
    private static void ConfigureUsers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(u => u.Id);

            // ⚠️ CRÍTICO PARA AUTH: El Email es obligatorio, con longitud límite y un ÍNDICE ÚNICO
            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);
            entity.HasIndex(u => u.Email)
                .IsUnique();

            // Hash de contraseña con espacio suficiente para algoritmos seguros (Argon2 / BCrypt / PBKDF2)
            entity.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            // El rol se almacena como texto con un límite controlado por tus constantes
            entity.Property(u => u.Role)
                .IsRequired()
                .HasMaxLength(50);

            // Estado de la cuenta
            entity.Property(u => u.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // 🔐 TENANT SCOPING: Indexamos la provincia/ubicación para acelerar filtros y JOINs,
            // respetando el desacoplamiento de DDD (Sin Foreign Keys físicas hacia otros módulos)
            entity.Property(u => u.LocationId)
                .IsRequired();
            entity.HasIndex(u => u.LocationId);
        });
    }
}