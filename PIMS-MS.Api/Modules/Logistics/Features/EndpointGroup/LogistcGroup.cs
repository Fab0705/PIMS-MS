namespace PIMS_MS.Api.Modules.Logistic.Features.EndpointGroup;

public static class LogisticGroup
{
    public static RouteGroupBuilder MapLogisticGroup(this IEndpointRouteBuilder routes)
    {
        return routes.MapGroup("/api/logistic").WithTags("Logistic");
    }
}