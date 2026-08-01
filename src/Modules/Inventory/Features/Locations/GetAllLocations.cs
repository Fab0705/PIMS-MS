using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using PIMS_MS.Common.Interfaces;
using PIMS_MS.Modules.Inventory.Database;
using PIMS_MS.Modules.Inventory.Features._EndpointGroup;

namespace PIMS_MS.Modules.Inventory.Features.Locations;

public static class GetAllLocations
{
    public record LocationsResponse(Guid Id, string Name, string Location);
    public record Query : IRequest<List<LocationsResponse>>;
    public class Handler : IRequestHandler<Query, List<LocationsResponse>>
    {
        private readonly InventoryDbContext _dbContext;
        public Handler(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<LocationsResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var locations = await (from l in _dbContext.Locations.AsNoTracking()
                                   join r in _dbContext.Regions.AsNoTracking() on l.RegionId equals r.Id
                                   orderby r.Name, l.Name // 3. Ordenamos alfabéticamente para que el Combobox se vea ordenado
                                   select new LocationsResponse(l.Id, l.Name, r.Name))
                                  .ToListAsync(cancellationToken);

            return locations;
        }
        public class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapInventoryGroup().MapGet("/locations", async (ISender sender) =>
                {
                    var result = await sender.Send(new Query());
                    return Results.Ok(result);
                })
                .WithName("GetAllLocations")
                .WithTags("Inventory - Locations")
                .AllowAnonymous();
            }
        }
    }
}