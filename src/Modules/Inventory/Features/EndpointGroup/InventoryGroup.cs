using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;


namespace PIMS_MS.Modules.Inventory.Features.EndpointGroup;

public static class InventoryGroup
{
    public static RouteGroupBuilder MapInventoryGroup(this IEndpointRouteBuilder routes)
    {
        return routes.MapGroup("/api/inventory").WithTags("Inventory");
    }
}