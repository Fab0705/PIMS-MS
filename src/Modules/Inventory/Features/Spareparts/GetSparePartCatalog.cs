using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using PIMS_MS.Common.Interfaces;
using PIMS_MS.Modules.Inventory.Database;
using PIMS_MS.Modules.Inventory.Features._EndpointGroup;

namespace PIMS_MS.Modules.Inventory.Features.Spareparts;

public static class GetSparePartCatalog
{
    public record Query : IRequest<List<SparePartResponse>>
    {
        public Guid? LocationId { get; init; }
    }
    public record SparePartResponse(Guid Id, string PartNumber, string Description, bool IsRework, int Quantity);
    public class Handler : IRequestHandler<Query, List<SparePartResponse>>
    {
        private readonly InventoryDbContext _dbContext;
        private readonly ICurrentService _currentService;

        public Handler(InventoryDbContext dbContext, ICurrentService currentService)
        {
            _dbContext = dbContext;
            _currentService = currentService;
        }

        public async Task<List<SparePartResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var targetLocationId = request.LocationId ?? _currentService.LocationId;

            var spareParts = await _dbContext.SpareParts
                .AsNoTracking()
                .Join(_dbContext.Stocks,
                    sp => sp.Id,
                    s => s.SparePartId,
                    (sp, s) => new { SparePart = sp, Stock = s })
                .Where(joined => joined.Stock.LocationId == targetLocationId)
                .Select(joined => new SparePartResponse(
                    joined.SparePart.Id,
                    joined.SparePart.PartNumber,
                    joined.SparePart.Description,
                    joined.SparePart.IsRework,
                    joined.Stock.Quantity))
                .ToListAsync(cancellationToken);

            return spareParts;
        }
    }

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapInventoryGroup().MapGet("/api/spareparts/catalog", async (ISender sender) =>
            {
                var query = new Query();
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetSparePartCatalog");
        }
    }
}