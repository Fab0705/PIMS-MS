namespace PIMS_MS.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(Guid userId, string role);
}