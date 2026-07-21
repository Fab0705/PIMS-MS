namespace PIMS_MS.Common.Interfaces;

public interface ICurrentService
{
    Guid UserId { get; }
    Guid LocationId { get; }
    string Role { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }

    bool IsAdmin { get; }
    bool IsInRole(string role);
}