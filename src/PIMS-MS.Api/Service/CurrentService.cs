using System.Security.Claims;
using PIMS_MS.Common.Interfaces;
using PIMS_MS.Modules.Identity.Domain.Constants;

namespace PIMS_MS.Api.Service;

public class CurrentService : ICurrentService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                           ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;
                           
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }
    }

    public Guid LocationId
    {
        get
        {
            // Extraemos exactamente el claim "location_id" que configuraste en tu JwtTokenGenerator
            var locationClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("location_id")?.Value;
            return Guid.TryParse(locationClaim, out var locationId) ? locationId : Guid.Empty;
        }
    }

    public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public string Role
    {
        get
        {
            var roleClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value
                         ?? _httpContextAccessor.HttpContext?.User?.FindFirst("role")?.Value;

            return roleClaim ?? string.Empty;
        }
    }

    public bool IsAdmin => IsInRole(Roles.Administrator);

    public bool IsInRole(string role)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null || !user.Identity?.IsAuthenticated == true)
            return false;

        return user.IsInRole(role) || string.Equals(Role, role, StringComparison.OrdinalIgnoreCase);
    }
}