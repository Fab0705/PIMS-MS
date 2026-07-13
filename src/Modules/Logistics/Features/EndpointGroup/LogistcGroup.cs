using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace PIMS_MS.Modules.Logistics.Features.EndpointGroup;

public static class LogistcGroup
{
    public static RouteGroupBuilder MapLogisticGroup(this IEndpointRouteBuilder routes)
    {
        return routes.MapGroup("/api/logistic").WithTags("Logistic");
    }
}