using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using PIMS_MS.Common.Interfaces;
using PIMS_MS.Modules.Logistics.Database;
using PIMS_MS.Modules.Logistics.Domain.Enums;
using PIMS_MS.Modules.Logistics.Features.EndpointGroup;

namespace PIMS_MS.Modules.Logistics.Features.Transfers;

public class GetTransfersHistory
{
    public record TransferItemResponse(Guid SparePartId, int Quantity);
    public record TransferResponse(
        Guid Id,
        string TrackingCode,
        Guid OriginLocationId,
        Guid DestinationLocationId,
        string Status,
        string? ExceptionNotes,
        DateTime CreatedAtUtc,
        List<TransferItemResponse> Items
    );
    public record Query(TransferStatus? Status = null, Guid? LocationId = null) : IRequest<List<TransferResponse>>;
    public class Handler : IRequestHandler<Query, List<TransferResponse>>
    {
        private readonly LogisticDbContext _dbContext;
        private readonly ICurrentService _currentService;
        public Handler(LogisticDbContext dbContext, ICurrentService currentService)
        {
            _dbContext = dbContext;
            _currentService = currentService;
        }
        public async Task<List<TransferResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Transfers
                .AsNoTracking()
                .AsQueryable();

            if (request.Status.HasValue)
            {
                query = query.Where(t => t.Status == request.Status.Value);
            }

            // 🔐 TENANT SCOPING POR ROLES:
            if (!_currentService.IsAdmin)
            {
                // Si es un Almacenero, le mostramos solo los envíos que salen DE su almacén o llegan A su almacén
                var userLocation = _currentService.LocationId;
                query = query.Where(t => t.OriginLocationId == userLocation || t.DestinationLocationId == userLocation);
            }
            else if (request.LocationId.HasValue && request.LocationId.Value != Guid.Empty)
            {
                // Si es Administrador y seleccionó un almacén en el dropdown de la web
                query = query.Where(t => t.OriginLocationId == request.LocationId.Value || t.DestinationLocationId == request.LocationId.Value);
            }

            var transfers = await query
                .Select(t => new TransferResponse(
                    t.Id,
                    t.TrackingCode,
                    t.OriginLocationId,
                    t.DestinationLocationId,
                    t.Status.ToString(),
                    t.ExceptionNotes,
                    t.CreatedAtUtc,
                    t.Items.Select(i => new TransferItemResponse(
                        i.SparePartId,
                        i.Quantity
                    )).ToList()
                ))
                .OrderByDescending(t => t.CreatedAtUtc)
                .ToListAsync(cancellationToken);

            return transfers;
        }
    }
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapLogisticGroup().MapGet("/transfers", async ([AsParameters] Query query, ISender sender) =>
            {
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetTransfersHistory")
            .WithSummary("Devuelve el historial auditado y filtrable de traslados interprovinciales.");
        }
    }
}