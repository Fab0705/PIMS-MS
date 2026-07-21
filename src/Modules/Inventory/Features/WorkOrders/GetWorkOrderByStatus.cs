using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using PIMS_MS.Common.Interfaces;
using PIMS_MS.Modules.Inventory.Database;
using PIMS_MS.Modules.Inventory.Domain.Enums;
using PIMS_MS.Modules.Inventory.Features._EndpointGroup;

namespace PIMS_MS.Modules.Inventory.Features.WorkOrders;

public static class GetWorkOrderByStatus
{
    public record WorkOrderItemResponse(Guid SparePartId, string PartNumber, string Description, int Quantity);
    public record WorkOrderResponse(
        Guid Id, 
        Guid LocationId, 
        string LocationName, 
        string WorkOrderNumber, 
        string Description, 
        string Status, 
        DateTime CreatedAtUtc,
        List<WorkOrderItemResponse> Items
    );
    public record Query(WorkOrderStatus Status, Guid? LocationId) : IRequest<List<WorkOrderResponse>>;
    public class Handler : IRequestHandler<Query, List<WorkOrderResponse>>
    {
        private readonly InventoryDbContext _dbContext;
        private readonly ICurrentService _currentService;

        public Handler(InventoryDbContext dbContext, ICurrentService currentService)
        {
            _dbContext = dbContext;
            _currentService = currentService;
        }

        public async Task<List<WorkOrderResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = _dbContext.WorkOrders
                .AsNoTracking()
                .Where(w => w.Status == request.Status);

            // 🔐 LÓGICA DE AISLAMIENTO POR ROL (TENANT SCOPING):
            // Si el usuario NO es Administrador (ej. es Almacenero u Operador), 
            // ignoramos el LocationId de su petición y lo FORZAMOS a ver solo su almacén asignado.
            if (!_currentService.Role.Equals("Administrator", StringComparison.OrdinalIgnoreCase)) 
            {
                var userLocationId = _currentService.LocationId;
                query = query.Where(w => w.LocationId == userLocationId);
            }
            else if (request.LocationId.HasValue && request.LocationId.Value != Guid.Empty)
            {
                // Si ES Administrador, le permitimos filtrar libremente por la provincia que solicite
                query = query.Where(w => w.LocationId == request.LocationId.Value);
            }
            // Si es Administrador y no envió LocationId, la consulta no aplica filtro de ubicación y le devuelve TODO el país.

            var workOrders = await query
                .Select(w => new WorkOrderResponse(
                    w.Id,
                    w.LocationId,
                    w.Location.Name,
                    w.WorkOrderNumber,
                    w.Description,
                    w.Status.ToString(),
                    w.CreatedAtUtc,
                    w.Items.Select(i => new WorkOrderItemResponse(
                        i.SparePartId,
                        i.SparePart.PartNumber,
                        i.SparePart.Description,
                        i.Quantity
                    )).ToList()
                ))
                .OrderByDescending(w => w.CreatedAtUtc)
                .ToListAsync(cancellationToken);

            return workOrders;
        }
    }

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapInventoryGroup().MapGet("/api/workorders/status/{status:regex(^Pending$|^InProgress$|^Completed$)}", async (string status, Guid? locationId, ISender sender) =>
            {
                if (!Enum.TryParse<WorkOrderStatus>(status, true, out var parsedStatus))
                {
                    return Results.BadRequest($"Estado de orden de trabajo inválido: {status}. Los valores válidos son: Pending, InProgress, Completed.");
                }

                var query = new Query(parsedStatus, locationId);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetWorkOrdersByStatus");
        }
    }
}