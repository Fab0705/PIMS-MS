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

public class ReceiveTransfer
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

            if (!_currentService.IsAdmin && transfer.DestinationLocationId != _currentService.LocationId)
                throw new UnauthorizedAccessException("Solo el almacén provincial de destino o un Administrador pueden registrar la recepción física de esta mercancía.");

            transfer.Receive();

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapLogisticGroup().MapPost("/Transfers/{transferId:Guid}/recieve", async (Guid transferId, ISender sender) =>
            {
                await sender.Send(new Command(transferId));
                return Results.NoContent();
            })
            .WithName("ReceiveTransfer")
            .WithSummary("Marca un traslado como Recibido (Received) y gatilla el ingreso de stock en el destino.");
        }
    }
}