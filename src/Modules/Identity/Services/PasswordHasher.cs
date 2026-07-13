using PIMS_MS.Common.Interfaces;

namespace PIMS_MS.Modules.Identity.Services;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        // Implementation for hashing password
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    }

    public bool VerifyPassword(string password, string hash)
    {
        // Implementation for verifying password
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    public string GenerateTemporaryPassword()
    {
        // Implementation for generating temporary password
        var randomPart = Guid.NewGuid().ToString("N").Substring(0, 8);
        return $"Pims-{randomPart}!";
    }
}