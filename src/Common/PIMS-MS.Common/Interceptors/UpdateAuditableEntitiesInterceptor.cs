
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PIMS_MS.Common.Interfaces;

namespace PIMS_MS.Common.Interceptors;

public class UpdateAuditableEntitiesInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentService _currentService;

    public UpdateAuditableEntitiesInterceptor(ICurrentService currentService)
    {
        _currentService = currentService;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var dbContext = eventData.Context;

        if (dbContext == null)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        // Obtener la fecha UTC actual de forma centralizada
        var utcNow = DateTime.UtcNow;
        
        // Obtener el usuario actual (o "System" si es un proceso en segundo plano / seeding)
        var userId = _currentService.UserId != Guid.Empty 
            ? _currentService.UserId.ToString() 
            : "System - Auto";

        // Filtramos y modificamos únicamente las entidades que implementan tu IAuditableEntity
        foreach (var entry in dbContext.ChangeTracker.Entries<IAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAtUtc = utcNow;
                entry.Entity.CreatedBy = userId;
            }
            else if (entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
            {
                // Aseguramos que los valores de creación jamás se sobrescriban por error en una edición
                entry.Property(a => a.CreatedAtUtc).IsModified = false;
                entry.Property(a => a.CreatedBy).IsModified = false;

                entry.Entity.LastModifiedAtUtc = utcNow;
                entry.Entity.LastModifiedBy = userId;
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}

// Extensión para detectar si entidades hijas (Owned Entities de DDD) fueron modificadas
public static class Extensions
{
    public static bool HasChangedOwnedEntities(this Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry) =>
        entry.References.Any(r => 
            r.TargetEntry != null && 
            r.TargetEntry.Metadata.IsOwned() && 
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}