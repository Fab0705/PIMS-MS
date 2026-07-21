using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using PIMS_MS.Common.Exceptions;
using PIMS_MS.Common.Interfaces;
using PIMS_MS.Modules.Inventory.Database;
using PIMS_MS.Modules.Inventory.Domain.Exceptions;
using PIMS_MS.Modules.Inventory.Features._EndpointGroup;

namespace PIMS_MS.Modules.Inventory.Features.StockManagement;
public static class AdjustStockLevel
{
    public record Command(Guid SparePartId, int Quantity, string Reason) : IRequest
    {
        public Guid? LocationId { get; init; }
    }
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
            var targetLocationId = request.LocationId ?? _currentService.LocationId;

            if (targetLocationId == Guid.Empty)
            {
                throw new InvalidDomainArgumentException("No se ha podido determinar una ubicación (LocationId) válida para realizar el ajuste.");
            }

            // Buscamos el stock físico actual
            var stock = await _dbContext.Stocks
                .FirstOrDefaultAsync(s => s.SparePartId == request.SparePartId 
                                       && s.LocationId == targetLocationId, 
                                     cancellationToken);

            // Si no existe, usamos nuestra excepción personalizada NotFoundException (del Common)
            if (stock == null)
            {
                throw new NotFoundException($"Stock para el repuesto {request.SparePartId} en la ubicación {targetLocationId}");
            }

            // Aplicamos el método del dominio puro (DDD). 
            // Si hay un error de regla interna, lanzará una excepción que hereda de DomainException
            stock.AdjustToPhysicalCount(request.Quantity, request.Reason);

            // Persistimos los cambios y despachamos eventos en memoria
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapInventoryGroup().MapPost("/stock/adjust", async (ISender sender, Command command) =>
            {
                await sender.Send(command);
                return Results.NoContent();
            })
            .WithName("AdjustStockLevel");
        }
    }
}