namespace PIMS_MS.Api.Modules.FieldService.Features.EndpointGroup;

public static class FieldServiceGroup
{
    public static RouteGroupBuilder MapFieldServiceGroup(this IEndpointRouteBuilder routes)
    {
        return routes.MapGroup("/api/field-service").WithTags("Field Service");
    }
}