using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;


namespace PIMS_MS.Modules.Identity.Features.EndpointGroup;

public static class IdentityGroup
{
    public static RouteGroupBuilder MapIdentityGroup(this IEndpointRouteBuilder routes)
    {
        return routes.MapGroup("/api/identity").WithTags("Identity");
    }
}