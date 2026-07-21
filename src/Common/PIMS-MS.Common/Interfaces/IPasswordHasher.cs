namespace PIMS_MS.Common.Interfaces;

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
    string GenerateTemporaryPassword();
}