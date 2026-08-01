using System.ComponentModel.DataAnnotations;
using PIMS_MS.Modules.Identity.Domain.Constants;

namespace PIMS_MS.Api.Modules.Identity.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string Role { get; private set; } = null!;
    public bool IsActive { get; private set; } = true;

    public Guid? LocationId { get; private set; }
    
    private User() { }
    public User(string email, string passwordHash, string role, Guid? locationId)
    {
        if (string.IsNullOrWhiteSpace(role) || !Roles.IsValidRole(role))
        {
            throw new ValidationException($"El rol '{role}' no es un rol válido dentro de la aplicación.");
        }
        // 2. CORREGIDO: Si enviaron un valor (no es null), validamos que no sea un Guid vacío
        if (locationId.HasValue && locationId.Value == Guid.Empty)
        {
            throw new ValidationException("Si se proporciona una locación, no puede ser un identificador vacío (Guid.Empty).");
        }

        bool isGlobalRole = string.Equals(role, "ConsultantLogistic", StringComparison.OrdinalIgnoreCase) 
                 || string.Equals(role, "Administrator", StringComparison.OrdinalIgnoreCase);

        if (!isGlobalRole && !locationId.HasValue)
        {
            throw new ValidationException($"El rol '{role}' requiere obligatoriamente estar asignado a una locación.");
        }

        Id = Guid.NewGuid();
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        LocationId = locationId;
    }
    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
    }
    public void ChangeLocation(Guid newLocationId)
    {
        if (newLocationId == Guid.Empty) throw new ValidationException("La nueva ubicación no es válida.");
        LocationId = newLocationId;
    }
    public void UpdateRole(string newRole)
    {
        if (string.IsNullOrWhiteSpace(newRole) || !Roles.IsValidRole(newRole))
        {
            throw new ValidationException($"El rol '{newRole}' no es un rol válido dentro de la agencia NATRYX.");
        }

        Role = newRole;
    }
}