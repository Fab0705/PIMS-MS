namespace PIMS_MS.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(Guid UserId, string Email, string Role, Guid LocationId);
}