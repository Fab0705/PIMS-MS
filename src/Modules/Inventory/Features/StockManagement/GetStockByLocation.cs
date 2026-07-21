using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using PIMS_MS.Common.Interfaces;
using PIMS_MS.Modules.Inventory.Database;
using PIMS_MS.Modules.Inventory.Features._EndpointGroup;

namespace PIMS_MS.Modules.Inventory.Features.StockManagement;

public static class GetStockByLocation
{
    public record Query(Guid LocationId) : IRequest<List<StockResponse>>;
    public record StockResponse(Guid SparePartId, string PartNumber, int Quantity);
    public class Handler : IRequestHandler<Query, List<StockResponse>>
    {
        private readonly InventoryDbContext _dbContext;

        public Handler(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<StockResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var stockList = await _dbContext.Stocks
                .AsNoTracking()
                .Where(s => s.LocationId == request.LocationId)
                .Select(s => new StockResponse(s.SparePartId, s.SparePart.PartNumber, s.Quantity))
                .ToListAsync(cancellationToken);

            return stockList;
        }
    }

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapInventoryGroup().MapGet("/stock/location/{locationId:guid}", async (Guid locationId, ISender sender) =>
            {
                var query = new Query(locationId);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetStockByLocation");
        }
    }
}