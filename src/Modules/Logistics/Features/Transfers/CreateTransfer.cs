using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using PIMS_MS.Common.Interfaces;
using PIMS_MS.Modules.Logistics.Database;
using PIMS_MS.Modules.Logistics.Domain.Entities;
using PIMS_MS.Modules.Logistics.Domain.Exceptions;
using PIMS_MS.Modules.Logistics.Features.EndpointGroup;

namespace PIMS_MS.Modules.Logistics.Features.Transfers;

public class CreateTransfer
{
    public record TransferItemDto(Guid SparePartId, int Quantity);
    public record Command(Guid? OriginLocationId, Guid DestinationLocationId, string TrackingCode, List<TransferItemDto> Items) : IRequest<Guid>;
    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.DestinationLocationId).NotEmpty().WithMessage("El almacén de destino es obligatorio.");
            RuleFor(x => x.TrackingCode).NotEmpty().WithMessage("El código de rastreo (TrackingCode) es obligatorio.");
            RuleFor(x => x.Items).NotEmpty().WithMessage("Un traslado debe contener al menos un repuesto en la guía.");
            RuleForEach(x => x.Items).ChildRules(items =>
            {
                items.RuleFor(i => i.SparePartId).NotEmpty().WithMessage("El ID del repuesto es inválido.");
                items.RuleFor(i => i.Quantity).GreaterThan(0).WithMessage("La cantidad a trasladar debe ser mayor a 0.");
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
            var originId = !_currentService.IsAdmin 
                ? _currentService.LocationId 
                : (request.OriginLocationId ?? _currentService.LocationId);

            if (originId == Guid.Empty)
                throw new InvalidLogisticsArgumentException("No se ha podido determinar el almacén de origen para el traslado.");
            
            var trackingExists = await _dbContext.Transfers
                .AnyAsync(t => t.TrackingCode == request.TrackingCode.Trim().ToUpperInvariant(), cancellationToken);

            if (trackingExists)
                throw new InvalidLogisticsArgumentException($"El código de rastreo '{request.TrackingCode}' ya está registrado en otra guía.");

            // Instanciamos la Raíz de Agregado (DDD puro)
            var transfer = new Transfer(Guid.NewGuid(), request.TrackingCode, originId, request.DestinationLocationId);

            foreach (var item in request.Items)
            {
                transfer.AddItem(item.SparePartId, item.Quantity);
            }

            // Opcional: Lo dejamos en ReadyForDispatch si los ítems ya están empacados
            transfer.MarkAsReadyForDispatch();

            _dbContext.Transfers.Add(transfer);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return transfer.Id;
        }
    }
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapLogisticGroup().MapPost("/transfers", async (Command command, ISender sender) =>
            {
                var id = await sender.Send(command);
                return Results.Created($"/api/logistics/transfers/{id}", new { id });
            })
            .WithName("Create Transfer")
            .WithSummary("Crea una nueva guía de traslado interprovincial de repuestos.");
        }
    }
}