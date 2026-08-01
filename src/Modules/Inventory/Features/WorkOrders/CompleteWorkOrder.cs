using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using PIMS_MS.Common.Contracts;
using PIMS_MS.Common.Exceptions;
using PIMS_MS.Common.Interfaces;
using PIMS_MS.Modules.Inventory.Database;
using PIMS_MS.Modules.Inventory.Domain.Constants;
using PIMS_MS.Modules.Inventory.Features._EndpointGroup;

namespace PIMS_MS.Modules.Inventory.Features.WorkOrders;

public static class CompleteWorkOrder
{
    public record Command(Guid WorkOrderId) : IRequest;
    public class Handler : IRequestHandler<Command>
    {
        private readonly InventoryDbContext _dbContext;
        private readonly ICurrentService _currentService;
        public Handler(InventoryDbContext dbContext, ICurrentService currentService)
        {
            _dbContext = dbContext;
            _currentService = currentService;
        }
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var workOrder = await _dbContext.WorkOrders
                .Include(t => t.Items)
                .FirstOrDefaultAsync(t => t.Id == request.WorkOrderId, cancellationToken);

            if (workOrder == null)
                throw new NotFoundException("Guía de Traslado", request.WorkOrderId);

            if (!_currentService.IsAdmin && workOrder.LocationId != _currentService.LocationId)
                throw new UnauthorizedAccessException("Solo el almacén provincial de origen o un Administrador pueden autorizar la salida de este traslado.");
            
            workOrder.CompleteWork();
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapInventoryGroup().MapPost("/workOrder/{workOrderId:guid}/complete-work", async (Guid workOrderId, ISender sender) =>
            {
                await sender.Send(new Command(workOrderId));
                return Results.NoContent();
            }).
            RequireAuthorization(new AuthorizeAttribute { Roles = RequiredRoles.OperatorManager })
            .WithName("Complete WorkOrder")
            .WithTags("Inventory - WorkOrder");
        }
    }
}