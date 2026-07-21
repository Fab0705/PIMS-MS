using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using PIMS_MS.Common.Interfaces;
using PIMS_MS.Modules.Logistics.Database;
using PIMS_MS.Modules.Logistics.Domain.Enums;
using PIMS_MS.Modules.Logistics.Features.EndpointGroup;

namespace PIMS_MS.Modules.Logistics.Features.Replenishments;

public class GetReplenishmentsByStatus
{
    public record ReplenishmentItemResponse(Guid SparePartId, int Quantity);
    public record ReplenishmentResponse(
        Guid Id,
        Guid LocationId,
        string Status,
        string? RejectionReason,
        DateTime CreatedAtUtc,
        string? CreatedBy,
        List<ReplenishmentItemResponse> Items
    );
    public record Query(ReplenishmentStatus? Status = null, Guid? LocationId = null) : IRequest<List<ReplenishmentResponse>>;
    public class Handler : IRequestHandler<Query, List<ReplenishmentResponse>>
    {
        private readonly LogisticDbContext _dbContext;
        private readonly ICurrentService _currentService;
        public Handler(LogisticDbContext dbContext, ICurrentService currentService)
        {
            _dbContext = dbContext;
            _currentService = currentService;
        }
        public async Task<List<ReplenishmentResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Replenishments
                .AsNoTracking()
                .AsQueryable();

            if (request.Status.HasValue)
            {
                query = query.Where(r => r.Status == request.Status.Value);
            }

            // 🔐 TENANT SCOPING POR ROLES:
            if (!_currentService.IsAdmin)
            {
                // El almacenero provincial queda restringido en LINQ a ver solo sus pedidos
                var userLocation = _currentService.LocationId;
                query = query.Where(r => r.LocationId == userLocation);
            }
            else if (request.LocationId.HasValue && request.LocationId.Value != Guid.Empty)
            {
                // El Admin central filtró por un almacén en el dropdown del Dashboard
                query = query.Where(r => r.LocationId == request.LocationId.Value);
            }

            var results = await query
                .Select(r => new ReplenishmentResponse(
                    r.Id,
                    r.LocationId,
                    r.Status.ToString(),
                    r.RejectionReason,
                    r.CreatedAtUtc,
                    r.CreatedBy,
                    r.Items.Select(i => new ReplenishmentItemResponse(
                        i.SparePartId,
                        i.Quantity
                    )).ToList()
                ))
                .OrderByDescending(r => r.CreatedAtUtc)
                .ToListAsync(cancellationToken);

            return results;
        }
    }
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapLogisticGroup().MapGet("/replenishments", async ([AsParameters] Query query, ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetReplenishmentsByStatus")
            .WithSummary("Obtiene el listado de solicitudes de reabastecimiento provincial auditadas.");
        }
    }
}