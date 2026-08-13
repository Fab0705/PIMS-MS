using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using PIMS_MS.Common.Interfaces;
using PIMS_MS.Modules.Inventory.Database;
using PIMS_MS.Modules.Inventory.Domain.Constants;
using PIMS_MS.Modules.Inventory.Features._EndpointGroup;

namespace PIMS_MS.Modules.Inventory.Features.StockManagement;
public static class SearchSparePartAvailability
{
    public record StockAvailabilityResponse(
        Guid SparePartId,
        string PartNumber,
        string Description,
        Guid LocationId,
        string LocationName,
        int AvailableQuantity
    );
    public record Query(string SearchTerm, Guid? LocationId = null) : IRequest<List<StockAvailabilityResponse>>;
    public class Handler : IRequestHandler<Query, List<StockAvailabilityResponse>>
    {
        private readonly InventoryDbContext _dbContext;
        private readonly ICurrentService _currentService;
        public Handler(InventoryDbContext dbContext, ICurrentService currentService)
        {
            _dbContext = dbContext;
            _currentService = currentService;
        }
        public async Task<List<StockAvailabilityResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Stocks
                .AsNoTracking()
                .Include(s => s.SparePart)
                .Include(s => s.Location)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.ToLower().Trim();
                query = query.Where(s =>
                    s.SparePart.PartNumber.ToLower().Contains(term) ||
                    s.SparePart.Description.ToLower().Contains(term));
            }

            var targetLocationId = request.LocationId ?? _currentService.LocationId;

            if (targetLocationId != Guid.Empty)
            {
                query = query.Where(s => s.LocationId == targetLocationId);
            }

            var results = await query
                .OrderBy(s => s.SparePart.Description)
                .ThenByDescending(s => s.Quantity)
                .Select(s => new StockAvailabilityResponse(
                    s.SparePartId,
                    s.SparePart.PartNumber,
                    s.SparePart.Description,
                    s.LocationId,
                    s.Location.Name,
                    s.Quantity
                ))
                .ToListAsync(cancellationToken);

            return results;
        }
    }
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapInventoryGroup().MapGet("/stock/search", async(string searchTerm, Guid? locationId, ISender sender) =>
            {
                var query = new Query(searchTerm, locationId);

                if (string.IsNullOrWhiteSpace(query.SearchTerm) && query.SearchTerm?.Length < 3)
                {
                    return Results.BadRequest("Debe ingresar al menos 3 caracteres para buscar.");
                }

                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = RequiredRoles.Guest })
            .WithName("SearchSparePartAvailability")
            .WithTags("Inventory - Stock Management");
        }
    }
}

