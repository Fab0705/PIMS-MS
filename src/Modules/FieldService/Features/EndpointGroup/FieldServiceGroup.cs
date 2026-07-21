using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace PIMS_MS.Modules.FieldService.Features.EndpointGroup;

public static class FieldServiceGroup
{
    public static RouteGroupBuilder MapFieldServiceGroup(this IEndpointRouteBuilder routes)
    {
        return routes.MapGroup("/api/field-service").WithTags("Field Service");
    }
}