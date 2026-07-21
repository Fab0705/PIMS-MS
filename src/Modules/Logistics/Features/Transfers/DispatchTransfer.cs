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

public class DispatchTransfer
{
    public record Command(Guid TransferId) : IRequest;
    public class Handler : IRequestHandler<Command>
    {
        private readonly LogisticDbContext _dbContext;
        private readonly ICurrentService _currentService;
        public Handler(LogisticDbContext dbContext, ICurrentService currentService)
        {
            _dbContext = dbContext;
            _currentService = currentService;
        }
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var transfer = await _dbContext.Transfers
                .Include(t => t.Items)
                .FirstOrDefaultAsync(t => t.Id == request.TransferId, cancellationToken);

            if (transfer == null)
                throw new NotFoundException("Guía de Traslado", request.TransferId);

            if (!_currentService.IsAdmin && transfer.OriginLocationId != _currentService.LocationId)
                throw new UnauthorizedAccessException("Solo el almacén provincial de origen o un Administrador pueden autorizar la salida de este traslado.");

            transfer.Dispatch();

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapLogisticGroup().MapPost("/transfers/{transferId:guid}/dispatch", async (Guid transferId, ISender sender) =>
            {
                await sender.Send(new Command(transferId));
                return Results.NoContent();
            })
            .WithName("Dispatch Transfer")
            .WithSummary("Marca un traslado como Despachado (InTransit) y gatilla el descuento de stock en origen.");
        }
    }
}