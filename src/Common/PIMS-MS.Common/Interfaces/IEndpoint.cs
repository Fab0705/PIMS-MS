using Microsoft.AspNetCore.Routing;

namespace PIMS_MS.Common.Interfaces;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}