namespace PIMS_MS.Api.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(Guid userId, string role);
}