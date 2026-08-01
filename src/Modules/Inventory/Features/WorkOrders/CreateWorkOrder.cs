using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using PIMS_MS.Common.Exceptions;
using PIMS_MS.Common.Interfaces;
using PIMS_MS.Modules.Inventory.Database;
using PIMS_MS.Modules.Inventory.Domain.Constants;
using PIMS_MS.Modules.Inventory.Domain.Entities;
using PIMS_MS.Modules.Inventory.Domain.Exceptions;
using PIMS_MS.Modules.Inventory.Features._EndpointGroup;

namespace PIMS_MS.Modules.Inventory.Features.WorkOrders;

public static class CreateWorkOrder
{
    public record WorkOrderItemDto(Guid SparePartId, int Quantity);
    public record Command(string WorkOrderNumber, string Description, List<WorkOrderItemDto> Items) : IRequest<Guid>;
    public class Handler : IRequestHandler<Command, Guid>
    {
        private readonly InventoryDbContext _dbContext;
        private readonly ICurrentService _currentService;

        public Handler(InventoryDbContext dbContext, ICurrentService currentService)
        {
            _dbContext = dbContext;
            _currentService = currentService;
        }

        public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
        {
            var userLocationId = _currentService.LocationId;
            
            var locationExists = await _dbContext.Locations
                .AnyAsync(l => l.Id == userLocationId, cancellationToken);
                
            if (!locationExists)
                throw new NotFoundException("Almacén Provincial", userLocationId);
                
            var requestedSparePartIds = request.Items.Select(i => i.SparePartId).Distinct().ToList();
            
            var existingPartsCount = await _dbContext.SpareParts
                .CountAsync(sp => requestedSparePartIds.Contains(sp.Id), cancellationToken);

            if (existingPartsCount != requestedSparePartIds.Count)
                throw new InvalidDomainArgumentException("Uno o más repuestos ingresados no existen en el catálogo maestro.");

            var workOrder = new WorkOrder(Guid.NewGuid(), userLocationId, request.WorkOrderNumber, request.Description);

            foreach (var item in request.Items)
            {
                workOrder.AddItem(item.SparePartId, item.Quantity);
            }

            _dbContext.WorkOrders.Add(workOrder);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return workOrder.Id;
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.WorkOrderNumber).NotEmpty().WithMessage("El número de orden es obligatorio.");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Debe detallar el trabajo a realizar.");
            RuleFor(x => x.Items).NotEmpty().WithMessage("La orden debe crearse con al menos un repuesto solicitado.");
            RuleForEach(x => x.Items).ChildRules(items =>
            {
                items.RuleFor(i => i.SparePartId).NotEmpty().WithMessage("El ID del repuesto no es válido.");
                items.RuleFor(i => i.Quantity).GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0.");
            });
        }
    }

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapInventoryGroup().MapPost("/api/workorders", async (Command command, ISender sender) =>
            {
                var workOrderId = await sender.Send(command);
                return Results.Created($"/api/workorders/{workOrderId}", new { Id = workOrderId });
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = RequiredRoles.OperatorManager })
            .WithName("CreateWorkOrder")
            .WithTags("Inventory - WorkOrder");
        }
    }
}