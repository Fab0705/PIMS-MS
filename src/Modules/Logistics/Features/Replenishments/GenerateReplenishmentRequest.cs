using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PIMS_MS.Common.Interfaces;
using PIMS_MS.Modules.Logistics.Database;
using PIMS_MS.Modules.Logistics.Domain.Entities;
using PIMS_MS.Modules.Logistics.Domain.Exceptions;
using PIMS_MS.Modules.Logistics.Features.EndpointGroup;

namespace PIMS_MS.Modules.Logistics.Features.Replenishments;

public class GenerateReplenishmentRequest
{
    public record ReplenishmentItemDto(Guid SparePartId, int Quantity);
    public record Command(Guid? LocationId, List<ReplenishmentItemDto> Items) : IRequest<Guid>;
    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("La solicitud debe contener al menos un repuesto para reabastecer.");
                
            RuleForEach(x => x.Items).ChildRules(items =>
            {
                items.RuleFor(i => i.SparePartId).NotEmpty().WithMessage("El ID del repuesto es inválido.");
                items.RuleFor(i => i.Quantity).GreaterThan(0).WithMessage("La cantidad solicitada debe ser mayor a 0.");
            });
        }
    }
    public class Handler : IRequestHandler<Command, Guid>
    {
        private readonly LogisticDbContext _dbContext;
        private readonly ICurrentService _currentService;
        public Handler(LogisticDbContext dbContext, ICurrentService currentService)
        {
            _dbContext = dbContext;
            _currentService = currentService;
        }
        public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
        {
            var targetLocationId = !_currentService.IsAdmin 
                ? _currentService.LocationId 
                : (request.LocationId ?? _currentService.LocationId);

            if (targetLocationId == Guid.Empty)
            {
                throw new InvalidLogisticsArgumentException("No se pudo determinar el almacén provincial que solicita el reabastecimiento.");
            }

            // Instanciamos la Raíz de Agregado
            var replenishment = new Replenishment(Guid.NewGuid(), targetLocationId);

            foreach (var item in request.Items)
            {
                replenishment.AddItem(item.SparePartId, item.Quantity);
            }

            _dbContext.Replenishments.Add(replenishment);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return replenishment.Id;
        }
    }
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapLogisticGroup().MapPost("/replenishments", async (Command command, ISender sender) =>
            {
                var id = await sender.Send(command);
                return Results.Created($"/api/logistics/replenishments/{id}", new { id });
            })
            .WithName("GenerateReplenishmentRequest")
            .WithSummary("Genera una nueva solicitud de abastecimiento de repuestos para una provincia.");
        }
    }
}