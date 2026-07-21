using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using PIMS_MS.Common.Exceptions;
using PIMS_MS.Common.Interfaces;
using PIMS_MS.Modules.Logistics.Database;
using PIMS_MS.Modules.Logistics.Features.EndpointGroup;

namespace PIMS_MS.Modules.Logistics.Features.Replenishments;

public class ApproveReplenishment
{
    public record Command(Guid ReplenishmentId) : IRequest;
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
            if (!_currentService.IsAdmin)
            {
                throw new UnauthorizedAccessException("No tienes permisos de Administrador o Central Logística para aprobar solicitudes de reabastecimiento.");
            }

            var replenishment = await _dbContext.Replenishments
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == request.ReplenishmentId, cancellationToken);

            if (replenishment == null)
                throw new NotFoundException("Solicitud de Reabastecimiento", request.ReplenishmentId);

            replenishment.Approve();

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapLogisticGroup().MapPost("/replenishments/{id:guid}/approve", async (Guid id, ISender sender) =>
            {
                await sender.Send(new Command(id));
                return Results.NoContent();
            })
            .WithName("ApproveReplenishment")
            .WithSummary("Aprueba una solicitud de abastecimiento y notifica para la creación de la guía de traslado.");
        }
    }
}