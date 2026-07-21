using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using PIMS_MS.Common.Exceptions;
using PIMS_MS.Common.Interfaces;
using PIMS_MS.Modules.Logistics.Database;
using PIMS_MS.Modules.Logistics.Features.EndpointGroup;

namespace PIMS_MS.Modules.Logistics.Features.Transfers;

public class GetTransferByTrackingCode
{
    public record TransferItemDetail(Guid SparePartId, int Quantity);
    public record TransferDetail(
        Guid Id,
        string TrackingCode,
        Guid OriginLocationId,
        Guid DestinationLocationId,
        string Status,
        string? ExceptionNotes,
        DateTime CreatedAtUtc,
        string? CreatedBy,
        DateTime? LastModifiedAtUtc,
        List<TransferItemDetail> Items
    );
    public record Query(string TrackingCode) : IRequest<TransferDetail>;
    public class Handler : IRequestHandler<Query, TransferDetail>
    {
        private readonly LogisticDbContext _dbContext;
        private readonly ICurrentService _currentService;
        public Handler(LogisticDbContext dbContext, ICurrentService currentService)
        {
            _dbContext = dbContext;
            _currentService = currentService;
        }
        public async Task<TransferDetail> Handle(Query request, CancellationToken cancellationToken)
        {
            var cleanCode = request.TrackingCode.Trim().ToUpperInvariant();

            var transfer = await _dbContext.Transfers
                .AsNoTracking()
                .Where(t => t.TrackingCode == cleanCode)
                .Select(t => new TransferDetail(
                    t.Id,
                    t.TrackingCode,
                    t.OriginLocationId,
                    t.DestinationLocationId,
                    t.Status.ToString(),
                    t.ExceptionNotes,
                    t.CreatedAtUtc,
                    t.CreatedBy,
                    t.LastModifiedAtUtc,
                    t.Items.Select(i => new TransferItemDetail(
                        i.SparePartId,
                        i.Quantity
                    )).ToList()
                ))
                .FirstOrDefaultAsync(cancellationToken);

            if (transfer == null)
                throw new NotFoundException($"No se encontró ninguna guía de traslado con el código '{cleanCode}'.");

            // 🔐 TENANT SCOPING: Blindaje para evitar que una provincia curiosee envíos que no le competen
            if (!_currentService.IsAdmin && 
                transfer.OriginLocationId != _currentService.LocationId && 
                transfer.DestinationLocationId != _currentService.LocationId)
            {
                throw new UnauthorizedAccessException("No tienes autorización para consultar el estado de este traslado interprovincial.");
            }

            return transfer;
        }
    }
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapLogisticGroup().MapGet("/trasnfer/track/{trackingCode}", async (string trackingCode, ISender sender) =>
            {
                var result = await sender.Send(new Query(trackingCode));
                return Results.Ok(result);
            })
            .WithName("GetTransferByTrackingCode")
            .WithSummary("Consulta el detalle exacto de una guía mediante su código de seguimiento (TrackingCode).");
        }
    }
}