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

public class RegisterTransferException
{
    public record Command(Guid TransferId, string Notes) : IRequest;
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
                .FirstOrDefaultAsync(t => t.Id == request.TransferId, cancellationToken);

            if (transfer == null)
                throw new NotFoundException("Guía de Traslado", request.TransferId);

            transfer.RegisterException(request.Notes);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapLogisticGroup().MapPost("/transfers/{transferId:Guid}/exception", async (Guid transferId, [AsParameters] Command body, ISender sender) =>
            {
                var command = new Command(transferId, body.Notes);
                await sender.Send(command);
                return Results.NoContent();
            })
            .WithName("RegisterTransferException")
            .WithSummary("Registra una incidencia, daño o pérdida en una guía de traslado.");
        }
    }
}