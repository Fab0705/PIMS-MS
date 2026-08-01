using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using PIMS_MS.Common.Contracts;
using PIMS_MS.Common.Exceptions;
using PIMS_MS.Common.Interfaces;
using PIMS_MS.Modules.Logistics.Database;
using PIMS_MS.Modules.Logistics.Domain.Constants;
using PIMS_MS.Modules.Logistics.Features.EndpointGroup;

namespace PIMS_MS.Modules.Logistics.Features.Transfers;

public class DispatchTransfer
{
    public record Command(Guid TransferId) : IRequest;
    public class Handler : IRequestHandler<Command>
    {
        private readonly LogisticDbContext _dbContext;
        private readonly ICurrentService _currentService;
        private readonly IPublisher _publisher;
        public Handler(LogisticDbContext dbContext, ICurrentService currentService, IPublisher publisher)
        {
            _dbContext = dbContext;
            _currentService = currentService;
            _publisher = publisher;
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

            var contractItems = transfer.Items
                        .Select(i => new TransferItemContractDto(i.SparePartId, i.Quantity))
                        .ToList();

            // Publicamos el Integration Event
            await _publisher.Publish(new TransferDispatchedIntegrationEvent(
                transfer.Id, 
                transfer.TrackingCode, // Asegúrate de tener esta propiedad en tu entidad
                transfer.OriginLocationId, 
                contractItems
            ), cancellationToken);
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
            .RequireAuthorization(new AuthorizeAttribute { Roles = RequiredRoles.OperatorManager })
            .WithName("Dispatch Transfer")
            .WithTags("Logistics - Transfers");
        }
    }
}