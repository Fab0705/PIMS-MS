namespace PIMS_MS.Api.Modules.Inventory.Features.EndpointGroup;

public static class InventoryGroup
{
    public static RouteGroupBuilder MapInventoryGroup(this IEndpointRouteBuilder routes)
    {
        return routes.MapGroup("/api/inventory").WithTags("Inventory");
    }
}