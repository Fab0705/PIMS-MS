using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PIMS_MS.Common.Interfaces;
using PIMS_MS.Modules.Inventory.Database;
using PIMS_MS.Modules.Inventory.Domain.Constants;
using PIMS_MS.Modules.Inventory.Features._EndpointGroup;

namespace PIMS_MS.Modules.Inventory.Features.StockManagement;

public static class CheckLowStockAlerts
{
    public record LowStockAlert(
        Guid StockId,
        Guid SparePartId,
        string PartNumber,
        string Description,
        Guid LocationId,
        string LocationName,
        int CurrentQuantity,
        int RecommendedThreshold,
        bool IsCritical // Bandera calculada para la UI o el correo (ej. stock cero o casi cero)
    );
    public record Query(int Threshold = 5, Guid? LocationId = null) : IRequest<List<LowStockAlert>>;
    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Threshold)
                .GreaterThanOrEqualTo(0).WithMessage("El umbral de alerta no puede ser un número negativo.")
                .LessThanOrEqualTo(1000).WithMessage("El umbral máximo permitido por consulta es 1000 unidades.");
        }
    }
    public class Handler : IRequestHandler<Query, List<LowStockAlert>>
    {
        private readonly InventoryDbContext _dbContext;
        private readonly ICurrentService _currentService;
        private readonly ILogger<Handler> _logger;
        public Handler(InventoryDbContext dbContext, ICurrentService currentService, ILogger<Handler> logger)
        {
            _dbContext = dbContext;
            _currentService = currentService;
            _logger = logger;
        }
        public async Task<List<LowStockAlert>> Handle(Query request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando escaneo de alertas de stock bajo con umbral <= {Threshold}", request.Threshold);
            
            var query = _dbContext.Stocks.AsNoTracking().Where(s => s.Quantity <= request.Threshold);

            if(!_currentService.Role.Equals(RequiredRoles.ConsultantLogistic))
            {
                var userLocationId = _currentService.LocationId;
                query = query.Where(s => s.LocationId == userLocationId);
            }
            else if (request.LocationId.HasValue && request.LocationId.Value != Guid.Empty)
            {
                query = query.Where(s => s.LocationId == request.LocationId.Value);
            }

            var dbAlerts = await query
                .Select(s => new 
                {
                    StockId = s.Id,
                    s.SparePartId,
                    s.SparePart.PartNumber,
                    s.SparePart.Description,
                    s.LocationId,
                    LocationName = s.Location.Name,
                    s.Quantity
                })
                .OrderBy(s => s.Quantity)
                .ToListAsync(cancellationToken);

            var alerts = dbAlerts
                .Select(s => new LowStockAlert(
                    s.StockId,
                    s.SparePartId,
                    s.PartNumber,
                    s.Description,
                    s.LocationId,
                    s.LocationName,
                    s.Quantity,
                    request.Threshold,
                    s.Quantity == 0
                ))
                .ToList();
            
            _logger.LogInformation("Escaneo completado. Se detectaron {Count} alertas de inventario.", alerts.Count);

            return alerts;
        }
    }

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapInventoryGroup().MapGet("/stock/alert/low-stock", async ([AsParameters] Query query, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(query, cancellationToken);
                return Results.Ok(result);
            })
            .RequireAuthorization(new AuthorizeAttribute { Roles = $"{RequiredRoles.OperatorManager},{RequiredRoles.ConsultantLogistic}" })
            .WithName("CheckLowStockAlerts")
            .WithTags("Inventory - Stock Management");
        }
    }
}