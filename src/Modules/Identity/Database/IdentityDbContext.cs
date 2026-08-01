using Microsoft.EntityFrameworkCore;
using PIMS_MS.Api.Modules.Identity.Domain.Entities;
using PIMS_MS.Modules.Identity.Domain.Entities;

namespace PIMS_MS.Modules.Identity.Database;

public sealed class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        //modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
        modelBuilder.HasDefaultSchema("identity");

        ConfigureUsers(modelBuilder);
        ConfigureRefreshTokens(modelBuilder);
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
                .IsRequired(false);
            entity.HasIndex(u => u.LocationId);
        });
    }
    private static void ConfigureRefreshTokens(ModelBuilder modelBuilder)
    {
        // Dentro de tu OnModelCreating / Configure:
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshTokens");
            entity.HasKey(x => x.Id);
            
            entity.Property(x => x.Token).IsRequired().HasMaxLength(200);
            
            // ⚡ VITAL: Creamos un índice para buscar rapidísimo por Token cuando el Frontend intente refrescar
            entity.HasIndex(x => x.Token).IsUnique();
            
            // Relación con el usuario
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Si borras al usuario, se borran sus sesiones
        });
    }
}