namespace PIMS_MS.Api.Modules.Identity.Features.EndpointGroup;

public static class IdentityGroup
{
    public static RouteGroupBuilder MapIdentityGroup(this IEndpointRouteBuilder routes)
    {
        return routes.MapGroup("/api/identity").WithTags("Identity");
    }
}